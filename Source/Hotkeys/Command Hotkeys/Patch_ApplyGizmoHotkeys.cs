using Harmony;
using Verse;
using System.Collections;
using System.Collections.Generic;


namespace Hotkeys
{
    [HarmonyPatch(typeof(Command), nameof(Command.GizmoOnGUI))]
    class Patch_ApplyGizmoHotkeys
    {
        public static List<Command> newCommands = new List<Command>();
        static Dictionary<string, KeyBindFlagged> keyCache = new Dictionary<string, KeyBindFlagged>();

        static void Prefix()
        {
            foreach (Command command in newCommands)
            {
                UpdateCommandHotkey(command);
            }
            newCommands.Clear();
        }

        private static KeyBindFlagged FullKeySearch(Command command)
        {
            Log.Message("Full Search");
            List<string> keys = command.KeyList();
            KeyBindFlagged keyCache = new KeyBindFlagged();

            foreach (string key in keys)
            {
                DirectKeyData direct = DirectKeys.GetKey(key);
                if (direct != null)
                {
                    keyCache.keyDef = direct.keyDef;
                    keyCache.isUpdated = true;
                    return keyCache;
                }

                GizmoKeyData gizmo = GizmoKeys.GetKey(key);
                if (gizmo != null)
                {
                    keyCache.keyDef = gizmo.keyDef;
                    keyCache.isUpdated = true;
                    return keyCache;
                }
            }

            keyCache.keyDef = null;
            keyCache.isUpdated = true;
            return keyCache;
        }

        private static void UpdateCommandHotkey(Command command)
        {
            string keyName = command.Key(true, true, true);
            bool isCached = keyCache.TryGetValue(keyName, out KeyBindFlagged keyData);

            if (!isCached || !keyData.isUpdated)
            {
                keyData = FullKeySearch(command);
                keyCache[keyName] = keyData;
            }

            if (keyData.keyDef != null)
            {
                command.hotKey = keyData.keyDef;
            }
        }

        public static void UpdateCache()
        {
            foreach (KeyBindFlagged key in keyCache.Values)
            {
                key.isUpdated = false;
            }
        }
    }

    [HarmonyPatch(typeof(Command), MethodType.Constructor)]
    class Patch_CommandConstructor
    {
        static void Postfix(Command __instance)
        {
            Patch_ApplyGizmoHotkeys.newCommands.Add(__instance);
        }

        public static void CheckStaticCommands()
        {
            foreach (DesignationCategoryDef defCat in DefDatabase<DesignationCategoryDef>.AllDefsListForReading)
            {
                foreach (Designator des in defCat.AllResolvedDesignators)
                {
                    Patch_ApplyGizmoHotkeys.newCommands.Add(des);
                }
            }
        }
    }
}

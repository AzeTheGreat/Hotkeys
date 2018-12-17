using Harmony;
using Verse;
using System.Collections;
using System.Collections.Generic;


namespace Hotkeys
{
    [HarmonyPatch(typeof(Command), nameof(Command.GizmoOnGUI))]
    class Patch_ApplyGizmoHotkeys
    {
        static Dictionary<string, KeyBindFlagged> keyCache = new Dictionary<string, KeyBindFlagged>();

        static void Prefix(Command __instance)
        {
            string keyName = __instance.Key(true, true, true);
            bool isCached = keyCache.TryGetValue(keyName, out KeyBindFlagged keyData);

            if (!isCached || !keyData.isUpdated)
            {
                keyData = FullKeySearch(__instance);
                keyCache[keyName] = keyData;
            }

            if (keyData.keyDef != null)
            {
                __instance.hotKey = keyData.keyDef;
            }
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
    }

    [HarmonyPatch(typeof(Command), MethodType.Constructor)]
    class Patch_CommandConstructor
    {
        static void Postfix(Command __instance)
        {
            
        }
    }
}

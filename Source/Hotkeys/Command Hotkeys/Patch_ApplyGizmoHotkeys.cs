using HarmonyLib;
using System.Collections.Generic;
using Verse;


namespace Hotkeys
{
    [HarmonyPatch(typeof(Command), "GizmoOnGUIInt")]
    class Patch_ApplyGizmoHotkeys
    {
        public static List<Command> newCommands = new List<Command>();

        static Dictionary<string, KeyBindFlagged> keyCache = new Dictionary<string, KeyBindFlagged>();
        static Dictionary<string, KeyBindingDef> oldKeyCache = new Dictionary<string, KeyBindingDef>();

        static void Prefix()
        {
            if (!Hotkeys.settings.useCommandHotkeys)
                return;

            foreach (Command command in newCommands)
                UpdateCommandHotkey(command);

            newCommands.Clear();
        }

        private static KeyBindFlagged FullKeySearch(Command command)
        {
            foreach (string key in command.KeyList())
            {
                DirectKeyData direct = DirectKeys.GetKey(key);
                if (direct != null)
                    return new KeyBindFlagged() { keyDef = direct.keyDef };

                GizmoKeyData gizmo = GizmoKeys.GetKey(key);
                if (gizmo != null)
                    return new KeyBindFlagged() { keyDef = gizmo.keyDef };
            }

            return new KeyBindFlagged();
        }

        private static void UpdateCommandHotkey(Command command)
        {
            string keyName = command.Key(true, true, true);

            if (!oldKeyCache.TryGetValue(keyName, out KeyBindingDef oldKeyDef))
                oldKeyCache[keyName] = oldKeyDef = command.hotKey;

            if (!keyCache.TryGetValue(keyName, out KeyBindFlagged keyData) || !keyData.isUpdated)
            {
                keyData = FullKeySearch(command);
                keyCache[keyName] = keyData;
                keyData.isUpdated = true;
            }

            if (keyData.keyDef != null)
                command.hotKey = keyData.keyDef;
            else
                command.hotKey = oldKeyDef;
        }

        public static void UpdateCache()
        {
            foreach (KeyBindFlagged key in keyCache.Values)
                key.isUpdated = false;
        }
    }
}

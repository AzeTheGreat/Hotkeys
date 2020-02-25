﻿using HarmonyLib;
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
            if(!Hotkeys.settings.useCommandHotkeys) { return; }

            foreach (Command command in newCommands)
            {
                UpdateCommandHotkey(command);
            }
            newCommands.Clear();
        }

        private static KeyBindFlagged FullKeySearch(Command command)
        {
            List<string> keys = command.KeyList();
            var keyCache = new KeyBindFlagged();

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
}

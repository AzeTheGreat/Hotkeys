using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;

namespace Hotkeys
{
    [StaticConstructorOnStartup]
    static class DirectKeys
    {
        public static Dictionary<string, DirectKeyData> directKeys;
        public static bool gizmoTriggered = false;

        static DirectKeys()
        {
            directKeys = Hotkeys.settings.directKeys;
            BuildDirectKeyDefs();
        }

        public static void BuildDirectKeyDefs()
        {
            if (directKeys == null)
            {
                directKeys = new Dictionary<string, DirectKeyData>();
            }

            foreach (var key in directKeys.Values)
            {
                key.CreateKeyDef();
            }
            KeyPrefs.Init();
        }

        public static void AddKey(Command command, bool name = true, bool type = false, bool desc = false)
        {
            string keyName = command.Key(name, type, desc);
            var data = new DirectKeyData
            {
                defName = keyName
            };
            directKeys[keyName] = data;
            data.CreateKeyDef();

            KeyPrefs.Init();
            KeyMods.BuildKeyModData();
            Patch_ApplyGizmoHotkeys.UpdateCache();
            Patch_CommandConstructor.CheckStaticCommands();
            Hotkeys.settings.Write();
        }

        public static void RemoveKey(Command command)
        {
            DirectKeyData data = TryKey(command);
            directKeys.Remove(data.defName);

            List<KeyBindingDef> keyDefs = new List<KeyBindingDef>
                {
                    data.keyDef
                };

            InitializeMod.RemoveKeyDefs(keyDefs);
            KeyPrefs.Init();
            Patch_ApplyGizmoHotkeys.UpdateCache();
            Patch_CommandConstructor.CheckStaticCommands();
            Hotkeys.settings.Write();
        }

        public static bool KeyPresent(Command command)
        {
            if (TryKey(command) != null)
            {
                return true;
            }
            return false;
        }

        public static DirectKeyData GetKey(Command command)
        {
            return TryKey(command);
        }

        public static DirectKeyData GetKey(string key)
        {
            directKeys.TryGetValue(key, out DirectKeyData data);
            return data;
        }

        private static DirectKeyData TryKey(Command command)
        {
            DirectKeyData data;
            List<string> keys = command.KeyList();

            foreach (string key in keys)
            {
                if (directKeys.TryGetValue(key, out data)) { return data; }
            }

            return null;
        }
    }
}

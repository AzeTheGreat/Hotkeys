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
        public static List<DirectKeyData> directKeys;
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
                directKeys = new List<DirectKeyData>();
            }

            foreach (var key in directKeys)
            {
                key.CreateKeyDef();
            }
            KeyPrefs.Init();
        }

        public static void AddKey(Command command, bool name = true, bool type = false, bool desc = false)
        {
            var data = new DirectKeyData
            {
                desLabelCap = command.Key(name, type, desc)
            };
            directKeys.Add(data);
            data.CreateKeyDef();

            KeyPrefs.Init();
            KeyMods.BuildKeyModData();
            Hotkeys.settings.Write();
        }

        public static void RemoveKey(Command command)
        {
            DirectKeyData data = TryKey(command);
            directKeys.Remove(data);

            List<KeyBindingDef> keyDefs = new List<KeyBindingDef>
                {
                    data.keyDef
                };

            InitializeMod.RemoveKeyDefs(keyDefs);
            KeyPrefs.Init();
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

        private static DirectKeyData TryKey(Command command)
        {
            DirectKeyData data;

            for (int i = 0; i < Extensions.names.Length; i++)
            {
                data = directKeys.FirstOrDefault(x => x.desLabelCap == command.Key(Extensions.names[i], Extensions.types[i], Extensions.descs[i]));
                if (data != null) { return data; }
            }

            return null;
        }
    }
}

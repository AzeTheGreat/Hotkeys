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

        public static void AddKey(string name)
        {
            var data = new DirectKeyData
            {
                desLabelCap = name
            };
            directKeys.Add(data);
            data.CreateKeyDef();

            KeyPrefs.Init();
            KeyMods.BuildKeyModData();
        }

        public static void RemoveKey(string name)
        {
            var direct = directKeys.Find(x => x.desLabelCap == name);
            directKeys.Remove(direct);

            List<KeyBindingDef> keyDefs = new List<KeyBindingDef>
                {
                    direct.keyDef
                };
            InitializeMod.RemoveKeyDefs(keyDefs);
            KeyPrefs.Init();
        }
    }
}

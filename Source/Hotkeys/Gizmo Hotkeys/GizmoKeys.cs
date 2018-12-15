using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;

namespace Hotkeys
{
    [StaticConstructorOnStartup]
    static class GizmoKeys
    {
        public static Dictionary<string, GizmoKeyData> gizmoKeys;

        static GizmoKeys()
        {
            gizmoKeys = Hotkeys.settings.gizmoKeys;
            AddSetterKey();
            BuildGizmoKeys();
        }

        private static void AddSetterKey()
        {
            var keyDef = new KeyBindingDef();
            keyDef.category = DefDatabase<KeyBindingCategoryDef>.GetNamed("GizmoKeys");
            keyDef.defName = "Hotkeys_GizmoAssigner";
            keyDef.label = "Assign Gizmos";
            keyDef.defaultKeyCodeA = UnityEngine.KeyCode.Ampersand;
            keyDef.modContentPack = DefDatabase<KeyBindingCategoryDef>.GetNamed("GizmoKeys").modContentPack;
            DefGenerator.AddImpliedDef<KeyBindingDef>(keyDef);
        }

        public static void BuildGizmoKeys()
        {
            if (gizmoKeys == null)
            {
                gizmoKeys = new Dictionary<string, GizmoKeyData>();
            }

            foreach (var keyData in gizmoKeys)
            {
                keyData.Value.BuildKeyDef();
            }
            
            KeyPrefs.Init();
        }

        public static void AddKey(Command command)
        {
            string name = command.Key(true, false, false);
            var data = new GizmoKeyData
            {
                defName = name
            };
            data.BuildKeyDef();

            KeyPrefs.Init();
            KeyMods.BuildKeyModData();

            gizmoKeys[name] = data;
            Hotkeys.settings.Write();
        }

        public static void RemoveKey(Command command)
        {
            GizmoKeyData data = TryKey(command);
            gizmoKeys.Remove(data.defName);

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

        public static GizmoKeyData GetKey(Command command)
        {
            return TryKey(command);
        }

        private static GizmoKeyData TryKey(Command command)
        {
            GizmoKeyData data;

            for (int i = 0; i < Extensions.names.Length; i++)
            {
                if (gizmoKeys.TryGetValue(command.Key(Extensions.names[i], Extensions.types[i], Extensions.descs[i]), out data)) { return data; }
            }

            return null;
        }
    }
}

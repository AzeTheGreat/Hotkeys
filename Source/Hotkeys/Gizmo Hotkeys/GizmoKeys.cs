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

        public static void AddKey(string name)
        {
            var data = new GizmoKeyData
            {
                defName = name
            };
            data.BuildKeyDef();

            KeyPrefs.Init();
            KeyMods.BuildKeyModData();

            gizmoKeys[name] = data;
        }

        public static void RemoveKey(string name)
        {
            GizmoKeys.gizmoKeys.TryGetValue(name, out var gizmo);
            GizmoKeys.gizmoKeys.Remove(name);

            List<KeyBindingDef> keyDefs = new List<KeyBindingDef>
                {
                    gizmo.keyDef
                };
            InitializeMod.RemoveKeyDefs(keyDefs);
            KeyPrefs.Init();
        }
    }
}

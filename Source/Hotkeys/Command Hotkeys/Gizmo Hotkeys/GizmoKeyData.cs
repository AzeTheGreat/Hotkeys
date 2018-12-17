using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;

namespace Hotkeys
{
    public class GizmoKeyData : IExposable
    {
        public string defName;
        public KeyBindingDef keyDef;

        public void ExposeData()
        {
            Scribe_Values.Look(ref defName, "KeybindingDefName");
        }

        public void BuildKeyDef()
        {
            keyDef = new KeyBindingDef();
            keyDef.category = DefDatabase<KeyBindingCategoryDef>.GetNamed("GizmoKeys");
            keyDef.defName = "Hotkeys_GizmoKeys_" + defName;
            keyDef.label = defName;
            keyDef.defaultKeyCodeA = UnityEngine.KeyCode.None;
            keyDef.modContentPack = DefDatabase<KeyBindingCategoryDef>.GetNamed("GizmoKeys").modContentPack;
            DefGenerator.AddImpliedDef<KeyBindingDef>(keyDef);
        }
    }
}

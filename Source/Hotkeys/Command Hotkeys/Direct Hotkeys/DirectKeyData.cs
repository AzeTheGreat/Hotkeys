using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using UnityEngine;
using Verse;

namespace Hotkeys
{
    public class DirectKeyData : GizmoKeyData, IExposable
    {
        private Designator _designator;
        public Designator Designator
        {
            get
            {
                if (_designator == null) { GetDesignator(); }
                return _designator;
            }
            private set
            {
                _designator = value;
            }
        }

        public DirectKeyData()
        {
            defName = "None";
        }

        public new void CreateKeyDef()
        {
            keyDef = new KeyBindingDef();
            keyDef.category = DefDatabase<KeyBindingCategoryDef>.GetNamed("DirectHotkeys");
            keyDef.defName = "Hotkeys_DirectHotkey_" + defName;
            keyDef.label = defName;
            keyDef.defaultKeyCodeA = UnityEngine.KeyCode.None;
            keyDef.modContentPack = DefDatabase<KeyBindingCategoryDef>.GetNamed("DirectHotkeys").modContentPack;
            DefGenerator.AddImpliedDef<KeyBindingDef>(keyDef);
        }

        private void GetDesignator()
        {
            var desCats = DefDatabase<DesignationCategoryDef>.AllDefsListForReading;
            foreach (var desCat in desCats)
            {
                var des = desCat.AllResolvedDesignators.FirstOrDefault(x => x.LabelCap == defName);
                if (des != null)
                {
                    Designator = des;
                }
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using UnityEngine;
using Verse;

namespace Hotkeys
{
    public class DirectKeyData : IExposable
    {
        public string desCategoryLabelCap;
        public string desLabelCap;
        public KeyBindingDef keyDef;

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
            desCategoryLabelCap = "None";
            desLabelCap = "None";
        }

        public void ExposeData()
        {
            Scribe_Values.Look(ref desCategoryLabelCap, "CategoryLabel");
            Scribe_Values.Look(ref desLabelCap, "DesignatorLabel");
        }

        public DesignationCategoryDef GetDesCategory()
        {
            if (!CheckDesCategory()) { return null; }

            var desCat = DefDatabase<DesignationCategoryDef>.AllDefsListForReading.Find(x => x.LabelCap == desCategoryLabelCap);
            return desCat;
        }

        public void CreateKeyDef(int i)
        {
            keyDef = new KeyBindingDef();
            keyDef.category = DefDatabase<KeyBindingCategoryDef>.GetNamed("DirectHotkeys");
            keyDef.defName = "Hotkeys_DirectHotkey_" + i.ToString();
            keyDef.label = desCategoryLabelCap + "/" + desLabelCap;
            keyDef.defaultKeyCodeA = UnityEngine.KeyCode.None;
            keyDef.modContentPack = DefDatabase<KeyBindingCategoryDef>.GetNamed("DirectHotkeys").modContentPack;
            DefGenerator.AddImpliedDef<KeyBindingDef>(keyDef);
        }

        private void GetDesignator()
        {
            if (!CheckDesCategory()) { return; }
            if (!CheckDesignator()) { return; }

            var desCat = DefDatabase<DesignationCategoryDef>.AllDefsListForReading.Find(x => x.LabelCap == desCategoryLabelCap);
            var des = desCat.AllResolvedDesignators.Find(x => x.LabelCap == desLabelCap);
            Designator = des;
        }

        private bool CheckDesCategory()
        {
            var allDesCatDefs = DefDatabase<DesignationCategoryDef>.AllDefsListForReading.Select(x => x.LabelCap);
            if (allDesCatDefs.Contains(desCategoryLabelCap))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private bool CheckDesignator()
        {
            if (!CheckDesCategory()) { return false; }

            var allDesCatDefs = DefDatabase<DesignationCategoryDef>.AllDefsListForReading.Find(x => x.LabelCap == desCategoryLabelCap);
            var allDesignators = allDesCatDefs.AllResolvedDesignators.Select(x => x.LabelCap);
            if (allDesignators.Contains(desLabelCap))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}

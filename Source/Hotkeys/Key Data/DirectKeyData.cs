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

        public Designator GetDesignator()
        {
            if (!CheckDesCategory()) { return null; }
            if (!CheckDesignator()) { return null; }

            var desCat = DefDatabase<DesignationCategoryDef>.AllDefsListForReading.Find(x => x.LabelCap == desCategoryLabelCap);
            var des = desCat.AllResolvedDesignators.Find(x => x.LabelCap == desLabelCap);
            return des;
        }

        public bool CheckDesCategory()
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

        public bool CheckDesignator()
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

using Harmony;
using Verse;
using System.Reflection;
using RimWorld;
using UnityEngine;
using System.Collections.Generic;
using Verse.Sound;
using RimWorld.Planet;
using System.Linq;

namespace Hotkeys
{
    public class HotkeySettings : ModSettings
    {
        public bool useArchitectHotkeys;
        public bool useDirectHotkeys;

        public List<string> desCategoryLabelCaps;
        public List<string> desLabelCaps;

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref useArchitectHotkeys, "Enable_Architect_Hotkeys");
            Scribe_Values.Look(ref useDirectHotkeys, "Enable_Direct_Hotkeys");

            Scribe_Collections.Look(ref desCategoryLabelCaps, "Designation_Categories");
            Scribe_Collections.Look(ref desLabelCaps, "Designators");

            // If lists don't exist create them
            if (desCategoryLabelCaps == null) { desCategoryLabelCaps = new List<string>(); }
            if (desLabelCaps == null) { desLabelCaps = new List<string>(); }
        }

        public DesignationCategoryDef GetDesCategory(int index)
        {
            if(!CheckDesCategory(index)) { return null; }

            var desCat = DefDatabase<DesignationCategoryDef>.AllDefsListForReading.Find(x => x.LabelCap == desCategoryLabelCaps[index]);
            return desCat;
        }

        public Designator GetDesignator(int index)
        {
            if (!CheckDesCategory(index)) { return null; }
            if (!CheckDesignator(index)) { return null; }

            var desCat = DefDatabase<DesignationCategoryDef>.AllDefsListForReading.Find(x=>x.LabelCap == desCategoryLabelCaps[index]);
            var des = desCat.AllResolvedDesignators.Find(x => x.LabelCap == desLabelCaps[index]);
            return des;
        }

        public bool CheckDesCategory(int index)
        {
            var allDesCatDefs = DefDatabase<DesignationCategoryDef>.AllDefsListForReading.Select(x => x.LabelCap);
            if (allDesCatDefs.Contains(desCategoryLabelCaps[index]))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool CheckDesignator(int index)
        {
            if (!CheckDesCategory(index)) { return false; }

            var allDesCatDefs = DefDatabase<DesignationCategoryDef>.AllDefsListForReading.Find(x => x.LabelCap == desCategoryLabelCaps[index]);
            var allDesignators = allDesCatDefs.AllResolvedDesignators.Select(x=>x.LabelCap);
            if (allDesignators.Contains(desLabelCaps[index]))
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

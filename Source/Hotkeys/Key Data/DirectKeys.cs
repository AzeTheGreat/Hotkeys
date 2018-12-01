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
        public static List<string> desCategoryLabelCaps;
        public static List<string> desLabelCaps;

        static DirectKeys()
        {
            desCategoryLabelCaps = Hotkeys.settings.desCategoryLabelCaps;
            desLabelCaps = Hotkeys.settings.desLabelCaps;
        }

        public static DesignationCategoryDef GetDesCategory(int index)
        {
            if (!CheckDesCategory(index)) { return null; }

            var desCat = DefDatabase<DesignationCategoryDef>.AllDefsListForReading.Find(x => x.LabelCap == desCategoryLabelCaps[index]);
            return desCat;
        }

        public static Designator GetDesignator(int index)
        {
            if (!CheckDesCategory(index)) { return null; }
            if (!CheckDesignator(index)) { return null; }

            var desCat = DefDatabase<DesignationCategoryDef>.AllDefsListForReading.Find(x => x.LabelCap == desCategoryLabelCaps[index]);
            var des = desCat.AllResolvedDesignators.Find(x => x.LabelCap == desLabelCaps[index]);
            return des;
        }

        public static bool CheckDesCategory(int index)
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

        public static bool CheckDesignator(int index)
        {
            if (!CheckDesCategory(index)) { return false; }

            var allDesCatDefs = DefDatabase<DesignationCategoryDef>.AllDefsListForReading.Find(x => x.LabelCap == desCategoryLabelCaps[index]);
            var allDesignators = allDesCatDefs.AllResolvedDesignators.Select(x => x.LabelCap);
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

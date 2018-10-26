using Harmony;
using Verse;
using RimWorld;
using UnityEngine;
using System.Collections.Generic;
using System;


namespace Hotkeys
{
    [HarmonyPatch(typeof(Game))]
    [HarmonyPatch("UpdatePlay")]
    public class DirectHotkeysPatch
    {
        static void Postfix()
        {
            var settings = LoadedModManager.GetMod<Hotkeys>().GetSettings<HotkeySettings>();

            if (Event.current.type != EventType.KeyDown) { return; }
            if (!settings.useDirectHotkeys) { return; }

            List<KeyBindingDef> designators = DefDatabase<KeyBindingDef>.AllDefsListForReading.FindAll(x => x.category == DefDatabase<KeyBindingCategoryDef>.GetNamed("DirectHotkeys"));

            foreach (var des in designators)
            {
                int desIndex = Int32.Parse(des.defName.Remove(0, 21));

                if (des.JustPressed)
                {
                    var desCatName = settings.desCategories[desIndex];
                    var designatorName = settings.designators[desIndex];
                    var designator = DefDatabase<DesignationCategoryDef>.AllDefsListForReading.Find(x => x.defName.Contains(desCatName)).AllResolvedDesignators.Find(x => x.LabelCap.Contains(designatorName));
                    Find.DesignatorManager.Select(designator);
                }
            }
        }
    }
}


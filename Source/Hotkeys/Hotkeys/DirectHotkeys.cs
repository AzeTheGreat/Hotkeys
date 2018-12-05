using Harmony;
using Verse;
using RimWorld;
using UnityEngine;
using System.Collections.Generic;
using System;
using System.Linq;


namespace Hotkeys
{
    [HarmonyPatch(typeof(Game))]
    [HarmonyPatch("UpdatePlay")]
    public class DirectHotkeysPatch
    {
        static void Postfix()
        {
            var settings = LoadedModManager.GetMod<Hotkeys>().GetSettings<Hotkeys_Settings>();
            var saved = LoadedModManager.GetMod<Hotkeys_Save>().GetSettings<Hotkeys_SettingsSave>();

            if (Event.current.type != EventType.KeyDown) { return; }
            if (!settings.useDirectHotkeys) { return; }

            List<KeyBindingDef> designatorKeys = DefDatabase<KeyBindingDef>.AllDefsListForReading.FindAll(x => x.category == DefDatabase<KeyBindingCategoryDef>.GetNamed("DirectHotkeys"));

            for (int i = 0; i < designatorKeys.Count; i++)
            {
                if (designatorKeys[i].JustPressed)
                {
                    var designator = saved.GetDesignator(i);
                    if (designator != null)
                    {
                        Find.DesignatorManager.Select(designator);
                    }
                }
            }
        }
    }
}


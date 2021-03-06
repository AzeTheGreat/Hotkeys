﻿using HarmonyLib;
using RimWorld;
using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace Hotkeys
{
    [HarmonyPatch(typeof(MainTabWindow_Architect), nameof(MainTabWindow_Architect.ExtraOnGUI))]
    public class Patch_ArchitectHotkeys
    {
        static bool keyWasDown = false;

        static void Prefix(ref ArchitectCategoryTab ___selectedDesPanel, ref List<ArchitectCategoryTab> ___desPanelsCached)
        {
            keyWasDown = false;

            if (Event.current.type != EventType.KeyDown) { return; }
            if (!Hotkeys.settings.useArchitectHotkeys) { return; }

            List<KeyBindingDef> categories = DefDatabase<KeyBindingDef>.AllDefsListForReading.FindAll(x => x.category == DefDatabase<KeyBindingCategoryDef>.GetNamed("ArchitectHotkeys"));
            
            foreach (var category in categories)
            {
                string tab = category.defName.Remove(0,23);

                if (category.JustPressed)
                {
                    var panel = ___desPanelsCached.Find(x => x.def.defName.Contains(tab));

                    if (panel != ___selectedDesPanel)
                    {
                        keyWasDown = true;
                        ___selectedDesPanel = panel;
                    }   
                }
            }
        }    

        static void Postfix()
        {
            // Due to keydown persisting for 6 frames (why?) this immediately deslects the 'carry through'
            if (keyWasDown)
                Find.DesignatorManager.Deselect();
        }
    } 
}
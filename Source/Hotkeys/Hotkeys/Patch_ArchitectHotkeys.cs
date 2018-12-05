using Harmony;
using Verse;
using RimWorld;
using UnityEngine;
using System.Collections.Generic;

namespace Hotkeys
{
<<<<<<< HEAD:Source/Hotkeys/Hotkeys/Patch_ArchitectHotkeys.cs
    [HarmonyPatch(typeof(MainTabWindow_Architect), nameof(MainTabWindow_Architect.ExtraOnGUI))]
    public class Patch_ArchitectHotkeys
=======
    [HarmonyPatch(typeof(MainTabWindow_Architect))]
    [HarmonyPatch("ExtraOnGUI")]
    public class ArchitectHotkeyPatch
>>>>>>> parent of 852b8cd... Reduced strings in harmony patches:Source/Hotkeys/Hotkeys/ArchitectHotkeys.cs
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
            {
                Find.DesignatorManager.Deselect();
            }
        }
    } 
}
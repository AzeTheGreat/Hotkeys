using Harmony;
using Verse;
using System.Reflection;
using RimWorld;
using UnityEngine;
using System.Collections.Generic;
using Verse.Sound;
using RimWorld.Planet;


namespace Hotkeys
{
    public class Hotkeys : Mod
    {
        public Hotkeys(ModContentPack content) : base(content)
        {
            var harmonyHotkeys = HarmonyInstance.Create("Hotkeys");
            harmonyHotkeys.PatchAll(Assembly.GetExecutingAssembly());
        }
    }



    [HarmonyPatch(typeof(MainTabWindow_Architect))]
    [HarmonyPatch("ExtraOnGUI")]
    public class ArchitectHotkeyPatch
    {
        static bool keyWasDown = false;

        static void Prefix(ref ArchitectCategoryTab ___selectedDesPanel, ref List<ArchitectCategoryTab> ___desPanelsCached)
        {
            
            keyWasDown = false;

            // If no key pressed just end
            if (Event.current.type != EventType.KeyDown) { return; }

            List<KeyBindingDef> categories = DefDatabase<KeyBindingDef>.AllDefsListForReading.FindAll(x => x.category == DefDatabase<KeyBindingCategoryDef>.GetNamed("ArchitectHotkeys"));
            
            foreach (var category in categories)
            {
                string tab = category.defName.Remove(0,17);

                if (category.JustPressed)
                {
                    keyWasDown = true;
                    ___selectedDesPanel = ___desPanelsCached.Find(x => x.def.defName.Contains(tab));
                }
            }
        }    

        static void Postfix()
        {
            // Due to keydown persisting for 6 frames (?) this immediately deslects the 'carry through'
            if (keyWasDown)
            {
                Find.DesignatorManager.Deselect();
            }
        }
    } 
}
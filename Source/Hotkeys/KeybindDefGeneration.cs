using RimWorld;
using Verse;
using Harmony;
using System.Collections.Generic;
using System.Linq;

namespace Hotkeys
{
    [HarmonyPatch(typeof(DefGenerator))]
    [HarmonyPatch("GenerateImpliedDefs_PostResolve")]
    public class KeybindDefGenerationPatch
    {
        static void Postfix()
        {
            // Generate keybindings for all architect subtabs.
            foreach (var def in DefDatabase<DesignationCategoryDef>.AllDefsListForReading)
            {
                var keyDef = new KeyBindingDef();
                keyDef.category = DefDatabase<KeyBindingCategoryDef>.GetNamed("ArchitectHotkeys");
                keyDef.defName = "ArchitectHotkeys_" + def.defName;
                keyDef.label = def.label + " tab";
                keyDef.defaultKeyCodeA = UnityEngine.KeyCode.None;
                keyDef.modContentPack = def.modContentPack;
                DefGenerator.AddImpliedDef<KeyBindingDef>(keyDef);

                //var test = DefDatabase<DesignationCategoryDef>.GetNamed("balh").AllResolvedDesignators.Find(x => x.Label.Contains("blah"));
            }

            var settings = LoadedModManager.GetMod<Hotkeys>().GetSettings<HotkeySettings>();

            // Sanitize settings - check for removed defs
            var allDesCatDefs = DefDatabase<DesignationCategoryDef>.AllDefsListForReading.Select(defCat => defCat.LabelCap);
            var toRemove = new List<int>();

            foreach (var desCat in settings.desCategories)
            {
                if (!allDesCatDefs.Contains(desCat))
                {
                    int index = settings.desCategories.IndexOf(desCat);
                    toRemove.Add(index);
                }

                if (allDesCatDefs.Contains(desCat))
                {
                    var allDesDefs = DefDatabase<DesignationCategoryDef>.GetNamed(desCat).AllResolvedDesignators.Select(des => des.Label);

                    foreach (var des in allDesDefs)
                    {
                        if (!allDesDefs.Contains(des))
                        {
                            int index = settings.desCategories.IndexOf(desCat);
                            toRemove.Add(index);  
                        }
                    }
                }
            }

            foreach (var i in toRemove)
            {
                settings.desCategories.RemoveAt(i);
                settings.designators.RemoveAt(i);
                settings.Write();
            }

            // Generate keybindings for all direct hotkeys
            foreach (var desCat in settings.desCategories)
            {
                Log.Message("Constructing: " + desCat);
                int index = settings.desCategories.IndexOf(desCat);
                string designator = settings.designators[index];
                var def = DefDatabase<DesignationCategoryDef>.GetNamed(desCat);

                var keyDef = new KeyBindingDef();
                keyDef.category = DefDatabase<KeyBindingCategoryDef>.GetNamed("ArchitectHotkeys");
                keyDef.defName = "ArchitectHotkeys_" + desCat + "_" + designator;
                keyDef.label = desCat + ": " + designator;
                keyDef.defaultKeyCodeA = UnityEngine.KeyCode.None;
                keyDef.modContentPack = def.modContentPack;
                DefGenerator.AddImpliedDef<KeyBindingDef>(keyDef);
            }
        }
    }
}

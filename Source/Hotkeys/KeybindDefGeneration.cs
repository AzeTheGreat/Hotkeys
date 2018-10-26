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
            var settings = LoadedModManager.GetMod<Hotkeys>().GetSettings<HotkeySettings>();

            if (settings.useArchitectHotkeys)
            {
                // Generate keybindings for all architect subtabs.
                foreach (var def in DefDatabase<DesignationCategoryDef>.AllDefsListForReading)
                {
                    var keyDef = new KeyBindingDef();
                    keyDef.category = DefDatabase<KeyBindingCategoryDef>.GetNamed("ArchitectHotkeys");
                    keyDef.defName = "Hotkeys_ArchitectHotkey" + def.defName;
                    keyDef.label = def.label + " tab";
                    keyDef.defaultKeyCodeA = UnityEngine.KeyCode.None;
                    keyDef.modContentPack = DefDatabase<KeyBindingCategoryDef>.GetNamed("ArchitectHotkeys").modContentPack;
                    DefGenerator.AddImpliedDef<KeyBindingDef>(keyDef);
                }
            }

            if (settings.useDirectHotkeys)
            {
                // Generate keybindings for all direct hotkeys
                for (int i = 0; i < settings.desCategories.Count; i++)
                {
                    var keyDef = new KeyBindingDef();
                    keyDef.category = DefDatabase<KeyBindingCategoryDef>.GetNamed("DirectHotkeys");
                    keyDef.defName = "Hotkeys_DirectHotkey_" + i.ToString();
                    keyDef.label = "Direct Hotkey " + i.ToString();
                    keyDef.defaultKeyCodeA = UnityEngine.KeyCode.None;
                    keyDef.modContentPack = DefDatabase<KeyBindingCategoryDef>.GetNamed("DirectHotkeys").modContentPack;
                    DefGenerator.AddImpliedDef<KeyBindingDef>(keyDef);
                }
            }

            

            // Sanitize settings - check for removed defs
            //var allDesCatDefs = DefDatabase<DesignationCategoryDef>.AllDefsListForReading.Select(x => x.LabelCap);

            //int index = 0;
            //while (index < settings.desCategories.Count)
            //{
            //    var desCat = settings.desCategories[index];

            //    if (!allDesCatDefs.Contains(desCat))
            //    {
            //        settings.desCategories.RemoveAt(index);
            //        settings.designators.RemoveAt(index);
            //    }
            //    else
            //    {
            //        index++;
            //    }
            //}

            //index = 0;
            //while (index < settings.designators.Count)
            //{
            //    var desCat = settings.desCategories[index];
            //    var des = settings.designators[index];
            //    var allDesDefs = DefDatabase<DesignationCategoryDef>.GetNamed(desCat).AllResolvedDesignators.Select(x => x.LabelCap);
            //    //var allDesDefs = DefDatabase<DesignationDef>.AllDefsListForReading.Select(x => x.LabelCap);

            //    Log.Message("Category: " + desCat);
            //    foreach (var name in allDesDefs)
            //    {
            //        Log.Message("Exists: " + name);
            //    }


            //    if (!allDesDefs.Contains(des))
            //    {
            //        Log.Message("Remove: " + des);
            //        settings.desCategories.RemoveAt(index);
            //        settings.designators.RemoveAt(index);
            //    }
            //    else
            //    {
            //        index++;
            //    }
            //}

            

            //foreach (var desCat in settings.desCategories)
            //{
            //    Log.Message("Constructing: " + desCat);
            //    int i = settings.desCategories.IndexOf(desCat);
            //    string designator = settings.designators[i];
            //    var def = DefDatabase<DesignationCategoryDef>.GetNamed(desCat);

            //    var keyDef = new KeyBindingDef();
            //    keyDef.category = DefDatabase<KeyBindingCategoryDef>.GetNamed("ArchitectHotkeys");
            //    keyDef.defName = "ArchitectHotkeys_" + desCat + "_" + designator;
            //    keyDef.label = desCat + ": " + designator;
            //    keyDef.defaultKeyCodeA = UnityEngine.KeyCode.None;
            //    keyDef.modContentPack = def.modContentPack;
            //    DefGenerator.AddImpliedDef<KeyBindingDef>(keyDef);
            //}
        }
    }
}

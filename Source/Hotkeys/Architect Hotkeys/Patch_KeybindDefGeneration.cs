using HarmonyLib;
using RimWorld;
using Verse;

namespace Hotkeys
{
    [HarmonyPatch(typeof(DefGenerator), nameof(DefGenerator.GenerateImpliedDefs_PostResolve))]
    public class Patch_KeybindDefGeneration
    {
        static void Postfix()
        {
            var settings = LoadedModManager.GetMod<Hotkeys>().GetSettings<Hotkeys_Settings>();

            if (settings.useArchitectHotkeys)
            {
                // Generate keybindings for all architect subtabs.
                foreach (var def in DefDatabase<DesignationCategoryDef>.AllDefsListForReading)
                {
                    var keyDef = new KeyBindingDef
                    {
                        category = DefDatabase<KeyBindingCategoryDef>.GetNamed("ArchitectHotkeys"),
                        defName = "Hotkeys_ArchitectHotkey" + def.defName,
                        label = def.label + " tab",
                        defaultKeyCodeA = UnityEngine.KeyCode.None,
                        modContentPack = DefDatabase<KeyBindingCategoryDef>.GetNamed("ArchitectHotkeys").modContentPack
                    };
                    DefGenerator.AddImpliedDef<KeyBindingDef>(keyDef);
                }
            }
        }
    }
}

using Harmony;
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
                    var keyDef = new KeyBindingDef();
                    keyDef.category = DefDatabase<KeyBindingCategoryDef>.GetNamed("ArchitectHotkeys");
                    keyDef.defName = "Hotkeys_ArchitectHotkey" + def.defName;
                    keyDef.label = def.label + " tab";
                    keyDef.defaultKeyCodeA = UnityEngine.KeyCode.None;
                    keyDef.modContentPack = DefDatabase<KeyBindingCategoryDef>.GetNamed("ArchitectHotkeys").modContentPack;
                    DefGenerator.AddImpliedDef<KeyBindingDef>(keyDef);
                }
            }
        }
    }
}

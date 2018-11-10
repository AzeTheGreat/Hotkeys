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
                for (int i = 0; i < settings.desCategoryLabelCaps.Count; i++)
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
        }
    }
}

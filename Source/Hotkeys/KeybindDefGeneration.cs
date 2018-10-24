using RimWorld;
using Verse;
using Harmony;

namespace Hotkeys
{
    [HarmonyPatch(typeof(DefGenerator))]
    [HarmonyPatch("GenerateImpliedDefs_PostResolve")]
    public class KeybindDefGenerationPatch
    {
        static void Postfix()
        {
            foreach (var def in DefDatabase<DesignationCategoryDef>.AllDefsListForReading)
            {
                var keyDef = new KeyBindingDef();
                keyDef.category = DefDatabase<KeyBindingCategoryDef>.GetNamed("ArchitectHotkeys");
                keyDef.defName = "ArchitectHotkeys_" + def.defName;
                keyDef.label = def.label + " tab";
                keyDef.defaultKeyCodeA = UnityEngine.KeyCode.None;
                keyDef.modContentPack = def.modContentPack;
                DefGenerator.AddImpliedDef<KeyBindingDef>(keyDef);
            }
        }
    }
}

using Harmony;
using Verse;

namespace Hotkeys
{
    // Postfix to clear modifiers when resest button is pressed
    [HarmonyPatch(typeof(KeyPrefsData), nameof(KeyPrefsData.ResetToDefaults))]
    public class Patch_ResetModifiers
    {
        static void Postfix()
        {
            KeyMods.allKeyModifiers.Clear();
            KeyMods.BuildKeyModData();
        }
    }

}


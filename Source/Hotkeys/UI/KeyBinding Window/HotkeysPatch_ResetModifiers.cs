using Verse;
using Harmony;

namespace Hotkeys
{
    // Postfix to clear modifiers when resest button is pressed
    [HarmonyPatch(typeof(KeyPrefsData), nameof(KeyPrefsData.ResetToDefaults))]
    public class HotkeysPatch_ResetModifiers
    {
        static void Postfix()
        {
            Hotkeys_Save.saved.allKeyModifiers.Clear();
            HotkeysGlobal.BuildKeyModData();
        }
    }

}


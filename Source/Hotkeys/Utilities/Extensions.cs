using Verse;
using System.Reflection;


namespace Hotkeys
{
    static class Extensions
    {
        public static KeyModData ModifierData(this KeyBindingDef keyDef)
        {
            KeyMods.allKeyModifiers.TryGetValue(keyDef.defName, out KeyModData keyModData);
            return keyModData;
        }

        public static string HotkeyLabel(this KeyBindingDef keyDef)
        {
            string label = keyDef.MainKeyLabel;

            foreach (var keyCode in keyDef.ModifierData().keyBindModsA)
            {
                string keyLabel = keyCode.ToStringReadable();
                label = keyLabel + " + " + label;
            }

            return label;
        }
    }
}

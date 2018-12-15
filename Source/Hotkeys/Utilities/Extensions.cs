using Verse;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;


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

        public static readonly bool[] names = { true, true, true, false, true, false, false };
        public static readonly bool[] types = { true, true, false, true, false, true, false };
        public static readonly bool[] descs = { true, false, true, true, false, false, true };
 
        private static readonly int descKeyLength = 10;

        public static string Key(this Command command, bool name, bool type, bool desc)
        {
            string label = "";

            if (name)
            {
                label = command.LabelCap;
            }
            if (type)
            {
                label = label + command.GetType().ToString();
            }
            if (desc)
            {
                string description = command.Desc;
                if (description.Length > descKeyLength)
                {
                    description.Substring(0, descKeyLength);
                }

                label += description;
            }

            return label;
        }
    }
}

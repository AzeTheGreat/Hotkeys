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

        public static string Truncate(this string str, int toLength)
        {
            if (str.Length > toLength)
            {
                str = str.Substring(0, toLength);
            }
            return str;
        }

        public static readonly bool[] names = { true, true, true, false, true, false, false };
        public static readonly bool[] types = { true, true, false, true, false, true, false };
        public static readonly bool[] descs = { true, false, true, true, false, false, true };
 
        public static readonly int descKeyLength = 20;

        public static string Key(this Command command, bool name, bool type, bool desc)
        {
            string label = "";

            if (name)
            {
                if (!command.LabelCap.NullOrEmpty()) { label = command.LabelCap; } 
            }
            if (type)
            {
                string s = command.GetType().ToString();
                label += " (" + s.Substring(s.LastIndexOf(".") + 1) + ")";
            }
            if (desc)
            {
                string description = command?.Desc.Truncate(descKeyLength);

                if (!description.NullOrEmpty()) { label += " '" + description + "'"; }
            }

            return label;
        }
    }
}

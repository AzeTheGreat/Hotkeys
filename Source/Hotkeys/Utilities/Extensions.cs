using Verse;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Text;


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
            if ((str?.Length ?? 0) > toLength)
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
            StringBuilder label = new StringBuilder();

            if (name)
            {
                if (!command.LabelCap.NullOrEmpty()) { label.Append(command.LabelCap); }
            }
            if (type)
            {
                string s = command.GetType().ToString();
                if (name) { label.Append(" "); }
                label.Append("(");
                label.Append(s.Substring(s.LastIndexOf(".") + 1));
                label.Append(")");
            }

            if (desc)
            {
                string description = command?.Desc.Truncate(descKeyLength);

                if (!description.NullOrEmpty())
                {
                    if (type) { label.Append(" "); }
                    label.Append("'");
                    label.Append(description);
                    label.Append("'");
                }
            }

            return label.ToString();
        }

        public static List<string> KeyList(this Command command)
        {
            string name = command.Key(true, false, false);
            string type = command.Key(false, true, false);
            string desc = command.Key(false, false, true);

            List<string> keys = new List<string>();

            for (int i = 0; i < names.Length; i++)
            {
                StringBuilder key = new StringBuilder();

                if (names[i]) { key.Append(name); }
                if (names[i] && types[i]) { key.Append(" "); }
                if (types[i]) { key.Append(type); }
                if(types[i] && descs[i]) { key.Append(" "); }
                if (descs[i]) { key.Append(desc); }

                keys.Add(key.ToString());
            }

            return keys;
        }

        public static Dictionary<string, KeyModData> Clone(this Dictionary<string, KeyModData> original)
        {
            Dictionary<string, KeyModData> returned = new Dictionary<string, KeyModData>();

            foreach (KeyValuePair<string, KeyModData> pair in original)
            {
                returned[pair.Key] = pair.Value.Clone();
            }

            return returned;
        }

        public static KeyModData Clone(this KeyModData old)
        {
            return new KeyModData
            {
                keyBindModsA = old.keyBindModsA.ListFullCopy(),
                keyBindModsB = old.keyBindModsB.ListFullCopy()
            };
        }
    }
}

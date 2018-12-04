using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;

namespace Hotkeys
{
    [StaticConstructorOnStartup]
    static class DirectKeys
    {
        public static List<DirectKeyData> directKeys;

        static DirectKeys()
        {
            directKeys = Hotkeys.settings.directKeys;
            BuildDirectKeyDefs();
        }

        public static void BuildDirectKeyDefs()
        {
            if (directKeys == null)
            {
                directKeys = new List<DirectKeyData>();
            }

            for (int i = 0; i < directKeys.Count; i++)
            {
                var keyDef = new KeyBindingDef();
                keyDef.category = DefDatabase<KeyBindingCategoryDef>.GetNamed("DirectHotkeys");
                keyDef.defName = "Hotkeys_DirectHotkey_" + i.ToString();
                keyDef.label = directKeys[i].desCategoryLabelCap + "/" + directKeys[i].desLabelCap;
                keyDef.defaultKeyCodeA = UnityEngine.KeyCode.None;
                keyDef.modContentPack = DefDatabase<KeyBindingCategoryDef>.GetNamed("DirectHotkeys").modContentPack;
                DefGenerator.AddImpliedDef<KeyBindingDef>(keyDef);
            }
            KeyPrefs.Init();
        }
    }
}

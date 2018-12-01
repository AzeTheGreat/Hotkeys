using Verse;
using System.Collections.Generic;
using System.Linq;

namespace Hotkeys
{
    public class Hotkeys_Settings : ModSettings
    {
        // Saved
        public Dictionary<string, KeyModData> allKeyModifiers;
        public List<string> desCategoryLabelCaps;
        public List<string> desLabelCaps;

        // Settings
        public bool useArchitectHotkeys;
        public bool useDirectHotkeys;
        public bool useMultiKeys;

        public Hotkeys_Settings()
        {
            allKeyModifiers = new Dictionary<string, KeyModData>();
            desCategoryLabelCaps = new List<string>();
            desLabelCaps = new List<string>();

            useArchitectHotkeys = false;
            useDirectHotkeys = false;
            useMultiKeys = false;
        }

        public override void ExposeData()
        {
            Scribe_Collections.Look(ref allKeyModifiers, "Hotkeys_Key_Modifiers");
            Scribe_Collections.Look(ref desCategoryLabelCaps, "Designation_Categories");
            Scribe_Collections.Look(ref desLabelCaps, "Designators");

            //base.ExposeData();
            Scribe_Values.Look(ref useArchitectHotkeys, "Enable_Architect_Hotkeys");
            Scribe_Values.Look(ref useDirectHotkeys, "Enable_Direct_Hotkeys");
            Scribe_Values.Look(ref useMultiKeys, "Enable_Multi_Keybindings");
        }
    }
}

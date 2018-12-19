using Verse;
using System.Collections.Generic;
using System.Linq;

namespace Hotkeys
{
    public class Hotkeys_Settings : ModSettings
    {
        // Saved
        public Dictionary<string, KeyModData> allKeyModifiers;
        public Dictionary<string, DirectKeyData> directKeys;
        public Dictionary<string, GizmoKeyData> gizmoKeys;

        // Settings
        public bool useArchitectHotkeys;
        public bool useCommandHotkeys;
        public bool useMultiKeys;

        public Hotkeys_Settings()
        {
            allKeyModifiers = new Dictionary<string, KeyModData>();
            directKeys = new Dictionary<string, DirectKeyData>();
            gizmoKeys = new Dictionary<string, GizmoKeyData>();

            useArchitectHotkeys = true;
            useCommandHotkeys = true;
            useMultiKeys = true;
        }

        public override void ExposeData()
        {
            Scribe_Collections.Look(ref allKeyModifiers, "Key_Modifiers");
            Scribe_Collections.Look(ref directKeys, "Direct_Keys");
            Scribe_Collections.Look(ref gizmoKeys, "Gizmo_Keys");

            //base.ExposeData();
            Scribe_Values.Look(ref useArchitectHotkeys, "Enable_Architect_Hotkeys");
            Scribe_Values.Look(ref useCommandHotkeys, "Enable_Command_Hotkeys");
            Scribe_Values.Look(ref useMultiKeys, "Enable_Multi_Keybindings");
        }
    }
}

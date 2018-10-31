using Harmony;
using Verse;
using System.Reflection;
using RimWorld;
using UnityEngine;
using System.Collections.Generic;
using Verse.Sound;
using RimWorld.Planet;
using System.Linq;

namespace Hotkeys
{
    public class HotkeySettings : ModSettings
    {
        public bool useArchitectHotkeys;
        public bool useDirectHotkeys;
        public bool useMultiKeys;

        public List<string> desCategoryLabelCaps;
        public List<string> desLabelCaps;

        public HotkeySettings()
        {
            useArchitectHotkeys = false;
            useDirectHotkeys = false;
            useMultiKeys = false;

            desCategoryLabelCaps = new List<string>();
            desLabelCaps = new List<string>();
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref useArchitectHotkeys, "Enable_Architect_Hotkeys");
            Scribe_Values.Look(ref useDirectHotkeys, "Enable_Direct_Hotkeys");
            Scribe_Values.Look(ref useMultiKeys, "Enable_Multi_Keybindings");

            Scribe_Collections.Look(ref desCategoryLabelCaps, "Designation_Categories");
            Scribe_Collections.Look(ref desLabelCaps, "Designators");
        }



        public DesignationCategoryDef GetDesCategory(int index)
        {
            if (!CheckDesCategory(index)) { return null; }

            var desCat = DefDatabase<DesignationCategoryDef>.AllDefsListForReading.Find(x => x.LabelCap == desCategoryLabelCaps[index]);
            return desCat;
        }

        public Designator GetDesignator(int index)
        {
            if (!CheckDesCategory(index)) { return null; }
            if (!CheckDesignator(index)) { return null; }

            var desCat = DefDatabase<DesignationCategoryDef>.AllDefsListForReading.Find(x => x.LabelCap == desCategoryLabelCaps[index]);
            var des = desCat.AllResolvedDesignators.Find(x => x.LabelCap == desLabelCaps[index]);
            return des;
        }

        public bool CheckDesCategory(int index)
        {
            var allDesCatDefs = DefDatabase<DesignationCategoryDef>.AllDefsListForReading.Select(x => x.LabelCap);
            if (allDesCatDefs.Contains(desCategoryLabelCaps[index]))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool CheckDesignator(int index)
        {
            if (!CheckDesCategory(index)) { return false; }

            var allDesCatDefs = DefDatabase<DesignationCategoryDef>.AllDefsListForReading.Find(x => x.LabelCap == desCategoryLabelCaps[index]);
            var allDesignators = allDesCatDefs.AllResolvedDesignators.Select(x => x.LabelCap);
            if (allDesignators.Contains(desLabelCaps[index]))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }

    // Needed for second settings
    public class HotkeysLate : Mod
    {
        public static HotkeySettingsLate settings;
        public static bool isInit = false;

        public HotkeysLate(ModContentPack content) : base(content)
        {
            isInit = true;
            LongEventHandler.QueueLongEvent(() => settings = GetSettings<HotkeySettingsLate>(), null, true, null);
        }
    }

    // Needed to avoid trying to call defs before generation
    public class HotkeySettingsLate : ModSettings
    {
        public Dictionary<KeyBindingDef, ExposableList<KeyCode>> keyBindModsA;
        public Dictionary<KeyBindingDef, ExposableList<KeyCode>> keyBindModsB;

        public HotkeySettingsLate()
        {
            keyBindModsA = new Dictionary<KeyBindingDef, ExposableList<KeyCode>>();
            keyBindModsB = new Dictionary<KeyBindingDef, ExposableList<KeyCode>>();
        }

        public override void ExposeData()
        {
            Scribe_Collections.Look(ref keyBindModsA, "List_of_Keybind_Modifiers_A");
            Scribe_Collections.Look(ref keyBindModsB, "List_of_Keybind_Modifiers_B");
        }
    }
}

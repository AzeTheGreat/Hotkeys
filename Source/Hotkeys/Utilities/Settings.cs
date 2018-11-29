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
    public class Hotkeys_Settings : ModSettings
    {
        public bool useArchitectHotkeys;
        public bool useDirectHotkeys;
        public bool useMultiKeys;

        public Hotkeys_Settings()
        {
            useArchitectHotkeys = false;
            useDirectHotkeys = false;
            useMultiKeys = false;
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref useArchitectHotkeys, "Enable_Architect_Hotkeys");
            Scribe_Values.Look(ref useDirectHotkeys, "Enable_Direct_Hotkeys");
            Scribe_Values.Look(ref useMultiKeys, "Enable_Multi_Keybindings");
        }
    }

    public class Hotkeys_Save : Mod
    {
        public static Hotkeys_SettingsSave saved;
        public static bool isInit = false;

        public Hotkeys_Save(ModContentPack content) : base(content)
        {
            // HARMONY
            var harmonyHotkeys = HarmonyInstance.Create("Hotkeys");
            HarmonyInstance.DEBUG = false;
            harmonyHotkeys.PatchAll(Assembly.GetExecutingAssembly());

            // INITIALIZE
            saved = GetSettings<Hotkeys_SettingsSave>();
            //LongEventHandler.QueueLongEvent(() => HotkeysGlobal.InitializeMod(), null, false, null);

            // THIS
            //isInit = true;
        }
    }

    public class Hotkeys_SettingsSave : ModSettings
    {
        public Dictionary<string, KeyModData> allKeyModifiers;

        public List<string> desCategoryLabelCaps;
        public List<string> desLabelCaps;

        public Hotkeys_SettingsSave()
        {
            allKeyModifiers = new Dictionary<string, KeyModData>();

            desCategoryLabelCaps = new List<string>();
            desLabelCaps = new List<string>();
        }

        public override void ExposeData()
        {
            Scribe_Collections.Look(ref allKeyModifiers, "Hotkeys_Key_Modifiers");

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
}

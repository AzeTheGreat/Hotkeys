using Harmony;
using Verse;
using System.Reflection;
using RimWorld;
using UnityEngine;
using System.Collections.Generic;
using Verse.Sound;
using RimWorld.Planet;

namespace Hotkeys
{
    public class Hotkeys : Mod
    {
        public override string SettingsCategory() => "Hotkeys";
        private HotkeySettings settings;

        public Hotkeys(ModContentPack content) : base(content)
        {
            // Initialize Harmony
            var harmonyHotkeys = HarmonyInstance.Create("Hotkeys");
            harmonyHotkeys.PatchAll(Assembly.GetExecutingAssembly());

            settings = GetSettings<HotkeySettings>();
        }

        // The UI part
        public override void DoSettingsWindowContents(Rect inRect)
        {
            settings.desCategories[0] = Widgets.TextField(inRect.TopHalf().TopHalf(), settings.desCategories[0]);
            settings.designators[0] = Widgets.TextField(inRect.BottomHalf().BottomHalf(), settings.designators[0]);

            settings.Write();
        }
    }

    public class HotkeySettings : ModSettings
    {
        public List<string> desCategories;
        public List<string> designators;

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Collections.Look(ref desCategories, "Designation_Categories");
            Scribe_Collections.Look(ref designators, "Designators");

            // If lists don't exist create them
            if (desCategories == null) { desCategories = new List<string>();}
            if (designators == null) { designators = new List<string>(); }
        }
    }
}




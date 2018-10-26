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
        private Vector2 scrollPosition;

        public Hotkeys(ModContentPack content) : base(content)
        {
            // Initialize Harmony
            var harmonyHotkeys = HarmonyInstance.Create("Hotkeys");
            harmonyHotkeys.PatchAll(Assembly.GetExecutingAssembly());

            settings = GetSettings<HotkeySettings>();
            scrollPosition = new Vector2(0f, 0f);

        }

        // The UI part
        public override void DoSettingsWindowContents(Rect canvas)
        {
            float contentHeight = 30f * settings.desCategories.Count + 200f;

            Rect source = new Rect(0f, 0f, canvas.width - 24f, contentHeight);
            Widgets.BeginScrollView(canvas, ref scrollPosition, source, true);

            var lMain = new Listing_Standard();
            lMain.ColumnWidth = source.width;
            lMain.Begin(source);

            lMain.CheckboxLabeled("Architect Hotkeys", ref settings.useArchitectHotkeys, "Check to enable the use of hotkeys to select subtabs in the Architect Tab.");
            lMain.GapLine();

            lMain.CheckboxLabeled("Direct Hotkeys", ref settings.useDirectHotkeys, "Check to enable direct hotkeys to any designator");
            lMain.GapLine();
            lMain.End();

            if (settings.useDirectHotkeys)
            {
                var grid = new GridLayout(new Rect(source.xMin, source.yMin + lMain.CurHeight, source.width, source.height - lMain.CurHeight), 3, 1, 0, 0);

                CategoryFloatMenus(grid);
                DesignatorFloatMenus(grid);
            }

            Widgets.EndScrollView();
            settings.Write();
        }

        private void DesignatorFloatMenus(GridLayout grid)
        {
            var rect = grid.GetCellRect(2, 0, 1);
            var listing = new Listing_Standard();
            listing.Begin(rect);

            for (int index = 0; index < settings.designators.Count; index++)
            {
                    var des = settings.designators[index];

                    if (listing.ButtonText(des))
                    {
                        int buttonNum = index;
                        List<FloatMenuOption> options = new List<FloatMenuOption>();

                        if (settings.desCategories[index] != "None")
                        {
                            foreach (var designator in DefDatabase<DesignationCategoryDef>.GetNamed(settings.desCategories[index]).AllResolvedDesignators)
                            {
                                options.Add(new FloatMenuOption(designator.LabelCap, delegate ()
                                {
                                    settings.designators[buttonNum] = designator.LabelCap;
                                }));
                            }
                        }

                        options.Add(new FloatMenuOption("None", delegate ()
                        {
                            settings.designators[buttonNum] = "None";
                        }));

                        FloatMenu window = new FloatMenu(options, "Select Category", false);
                        Find.WindowStack.Add(window);
                    }
            }

            listing.Gap();

            if (listing.ButtonText("Add Hotkey", "Add additional direct hotkeys"))
            {
                settings.desCategories.Add("None");
                settings.designators.Add("None");
            }

            listing.End();
        }

        private void CategoryFloatMenus(GridLayout grid)
        {
            var rect1 = grid.GetCellRect(0, 0, 2);
            var listing1 = new Listing_Standard();
            listing1.Begin(rect1);

            for (int index = 0; index < settings.desCategories.Count; index++)
            {
                var cat = settings.desCategories[index];

                if (listing1.ButtonTextLabeled("Direct Hotkey " + index.ToString(), cat))
                {
                    int buttonNum = index;
                    List<FloatMenuOption> options = new List<FloatMenuOption>();

                    foreach (var desCat in DefDatabase<DesignationCategoryDef>.AllDefsListForReading)
                    {
                        options.Add(new FloatMenuOption(desCat.LabelCap, delegate ()
                        {
                            settings.desCategories[buttonNum] = desCat.LabelCap;
                        }));
                    }

                    options.Add(new FloatMenuOption("None", delegate ()
                    {
                        settings.desCategories[buttonNum] = "None";
                    }));

                    FloatMenu window = new FloatMenu(options, "Select Category", false);
                    Find.WindowStack.Add(window);
                }
            }

            listing1.Gap();

            listing1.Label("Game must be restarted for keybindings to be available.");
            listing1.Gap();
            listing1.GapLine();

            listing1.End();
        }
    }



    public class HotkeySettings : ModSettings
    {
        public bool useArchitectHotkeys;
        public bool useDirectHotkeys;

        public List<string> desCategories;
        public List<string> designators;

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref useArchitectHotkeys, "Enable_Architect_Hotkeys");
            Scribe_Values.Look(ref useDirectHotkeys, "Enable_Direct_Hotkeys");

            Scribe_Collections.Look(ref desCategories, "Designation_Categories");
            Scribe_Collections.Look(ref designators, "Designators");

            // If lists don't exist create them
            if (desCategories == null) { desCategories = new List<string>();}
            if (designators == null) { designators = new List<string>(); }
        }
    }
}





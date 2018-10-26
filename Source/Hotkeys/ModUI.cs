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

        public override void DoSettingsWindowContents(Rect canvas)
        {
            float contentHeight = 30f * settings.desCategoryLabelCaps.Count + 200f;

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
                var grid = new GridLayout(new Rect(source.xMin, source.yMin + lMain.CurHeight, source.width, source.height - lMain.CurHeight), 4, 1, 0, 0);

                CategoryFloatMenus(grid);
                DesignatorFloatMenus(grid);
                RemoveButtons(grid);
            }

            Widgets.EndScrollView();
            settings.Write();
        }

        private void RemoveButtons(GridLayout grid)
        {
            var rect = grid.GetCellRect(3, 0, 1);
            var listing = new Listing_Standard();
            var rect2 = new Rect(rect.xMax - 30f, rect.yMin, 30f, rect.height);
            listing.Begin(rect2);

            for (int i = 0; i < settings.desLabelCaps.Count; i++)
            {
                if (listing.ButtonText("-"))
                {
                    settings.desCategoryLabelCaps.RemoveAt(i);
                    settings.desLabelCaps.RemoveAt(i);
                }
            }

            listing.End();
        }

        private void DesignatorFloatMenus(GridLayout grid)
        {
            var rect = grid.GetCellRect(2, 0, 1);
            var listing = new Listing_Standard();
            listing.Begin(rect);

            for (int index = 0; index < settings.desLabelCaps.Count; index++)
            {
                if (listing.ButtonText(settings.desLabelCaps[index]))
                {
                    var options = GetDesFloatMenuOptions(index);

                    FloatMenu window = new FloatMenu(options, "Select Category", false);
                    Find.WindowStack.Add(window);
                }
            }

            listing.Gap();

            if (listing.ButtonText("Add Hotkey", "Add additional direct hotkeys"))
            {
                settings.desCategoryLabelCaps.Add("None");
                settings.desLabelCaps.Add("None");
            }

            listing.End();
        }

        private void CategoryFloatMenus(GridLayout grid)
        {
            var rect = grid.GetCellRect(0, 0, 2);
            var listing = new Listing_Standard();
            listing.Begin(rect);

            for (int index = 0; index < settings.desCategoryLabelCaps.Count; index++)
            {
                if (listing.ButtonTextLabeled("Direct Hotkey " + index.ToString(), settings.desCategoryLabelCaps[index]))
                {
                    var options = GetCatFloatMenuOptions(index);

                    FloatMenu window = new FloatMenu(options, "Select Category", false);
                    Find.WindowStack.Add(window);
                }
            }

            listing.Gap();

            listing.Label("Game must be restarted for new keybindings to be available.");
            listing.Gap();
            listing.GapLine();

            listing.End();
        }

        private List<FloatMenuOption> GetDesFloatMenuOptions(int index)
        {
            int buttonNum = index;
            var options = new List<FloatMenuOption>();

            if (settings.CheckDesCategory(index))
            {
                var designators = settings.GetDesCategory(index).AllResolvedDesignators;
                foreach (var designator in designators)
                {
                    options.Add(new FloatMenuOption(designator.LabelCap, delegate ()
                    {
                        settings.desLabelCaps[buttonNum] = designator.LabelCap;
                    }));
                }
            }
            else
            {
                options.Add(new FloatMenuOption("None", delegate ()
                {
                    settings.desLabelCaps[buttonNum] = "None";
                }));
            }

            return options;
        }

        private List<FloatMenuOption> GetCatFloatMenuOptions(int index)
        {
            int buttonNum = index;
            var options = new List<FloatMenuOption>();

            var desCatDefs = DefDatabase<DesignationCategoryDef>.AllDefsListForReading;
            foreach (var desCat in desCatDefs)
            {
                options.Add(new FloatMenuOption(desCat.LabelCap, delegate ()
                {
                    settings.desCategoryLabelCaps[buttonNum] = desCat.LabelCap;
                }));
            }

            return options;
        }
    }
}




using System.Collections.Generic;
using UnityEngine;
using Verse;
using Harmony;
using System.Reflection;
using RimWorld;

namespace Hotkeys
{
    public class Hotkeys : Mod
    {
        public override string SettingsCategory() => "Hotkeys";

        public static Hotkeys_Settings settings;

        private Vector2 scrollPosition;
        public static bool isInit = false;

        public Hotkeys(ModContentPack content) : base(content)
        {
            // HARMONY
            var harmonyHotkeys = HarmonyInstance.Create("Hotkeys");
            HarmonyInstance.DEBUG = false;
            harmonyHotkeys.PatchAll(Assembly.GetExecutingAssembly());

            // SETTINGS
            settings = GetSettings<Hotkeys_Settings>();

            // THIS
            scrollPosition = new Vector2(0f, 0f);
        }

        public override void DoSettingsWindowContents(Rect canvas)
        {
            float contentHeight = 30f * settings.desCategoryLabelCaps.Count + 1000f;

            Rect source = new Rect(0f, 0f, canvas.width - 24f, contentHeight);
            Widgets.BeginScrollView(canvas, ref scrollPosition, source, true);

            var lMain = new Listing_Standard();
            lMain.ColumnWidth = source.width;
            lMain.Begin(source);

            lMain.GapLine();
            lMain.CheckboxLabeled("Multi Keybindings", ref settings.useMultiKeys, "Check to enable binding multiple keystrokes to each keybinding");
            lMain.CheckboxLabeled("Architect Hotkeys", ref settings.useArchitectHotkeys, "Check to enable the use of hotkeys to select subtabs in the Architect Tab.");
            lMain.GapLine();
            lMain.Label("The game MUST be restarted to add or remove keybinding options.  Set keybinds in the standard menu.");


            lMain.Gap();
            lMain.Gap();
            lMain.CheckboxLabeled("Direct Hotkeys", ref settings.useDirectHotkeys, "Check to enable direct hotkeys to any designator");
            lMain.GapLine();

            if (settings.useDirectHotkeys)
            {
                lMain.Gap();
                lMain.End();

                var grid = new GridLayout(new Rect(source.xMin, source.yMin + lMain.CurHeight, source.width, source.height - lMain.CurHeight), 5, 1, 0, 0);

                CategoryFloatMenus(grid);
                DesignatorFloatMenus(grid);
                RemoveButtons(grid);
            }
            else
            {
                lMain.End();
            }

            Widgets.EndScrollView();
            settings.Write();
        }

        public override void WriteSettings()
        {
            UpdateDirectHotkeys();
            base.WriteSettings();
        }

        private void UpdateDirectHotkeys()
        {
            List<KeyBindingDef> allDirectDefs = DefDatabase<KeyBindingDef>.AllDefsListForReading.FindAll(x => x.category == DefDatabase<KeyBindingCategoryDef>.GetNamed("DirectHotkeys"));
            List<KeyBindingDef> allKeyDefs = DefDatabase<KeyBindingDef>.AllDefsListForReading;

            foreach (var keyDef in allDirectDefs)
            {
                allKeyDefs.RemoveAll(x => x == keyDef);
            }

            List<KeyBindingDef> allOldDefs = allKeyDefs.ListFullCopy();

            DefDatabase<KeyBindingDef>.Clear();
            Log.Message(allOldDefs.Count.ToString());

            foreach (var keyDef in allOldDefs)
            {
                DefDatabase<KeyBindingDef>.Add(keyDef);
            }

            for (int i = 0; i < settings.desCategoryLabelCaps.Count; i++)
            {
                var keyDef = new KeyBindingDef();
                keyDef.category = DefDatabase<KeyBindingCategoryDef>.GetNamed("DirectHotkeys");
                keyDef.defName = "Hotkeys_DirectHotkey_" + i.ToString();
                keyDef.label = settings.desCategoryLabelCaps[i] + "/" + settings.desLabelCaps[i];
                keyDef.defaultKeyCodeA = UnityEngine.KeyCode.None;
                keyDef.modContentPack = DefDatabase<KeyBindingCategoryDef>.GetNamed("DirectHotkeys").modContentPack;
                DefGenerator.AddImpliedDef<KeyBindingDef>(keyDef);
            }
            KeyPrefs.Init();

        }

        private void RemoveButtons(GridLayout grid)
        {
            var rect = grid.GetCellRect(4, 0, 1);
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

            listing.End();
        }

        private List<FloatMenuOption> GetDesFloatMenuOptions(int index)
        {
            int buttonNum = index;
            var options = new List<FloatMenuOption>();

            if (DirectKeys.CheckDesCategory(index))
            {
                var designators = DirectKeys.GetDesCategory(index).AllResolvedDesignators;
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




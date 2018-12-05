using Harmony;
using Verse;
<<<<<<< HEAD:Source/Hotkeys/Settings/Hotkeys.cs
using Harmony;
using System.Reflection;
using RimWorld;
=======
using System.Reflection;
using RimWorld;
using UnityEngine;
using System.Collections.Generic;
using Verse.Sound;
using RimWorld.Planet;
>>>>>>> parent of 22a6550... Cleaned up usings:Source/Hotkeys/UI/ModSettings.cs

namespace Hotkeys
{
    public class Hotkeys : Mod
    {
        public override string SettingsCategory() => "Hotkeys";

        public static Hotkeys_Settings settings;

        private Vector2 scrollPosition;
        public static bool isInit = false;

        private float indent = 10f;

        public Hotkeys(ModContentPack content) : base(content)
        {
            // HARMONY
            var harmonyHotkeys = HarmonyInstance.Create("Hotkeys");
            HarmonyInstance.DEBUG = false;
            harmonyHotkeys.PatchAll(Assembly.GetExecutingAssembly());

            // SETTINGS
            Log.Message("Settings Get Got");
            settings = GetSettings<Hotkeys_Settings>();

            // THIS
            scrollPosition = new Vector2(0f, 0f);
        }

        public override void DoSettingsWindowContents(Rect canvas)
        {
            float contentHeight = 30f * DirectKeys.directKeys.Count + 1000f;

            Rect source = new Rect(0f, 0f, canvas.width - 24f, contentHeight);
            Widgets.BeginScrollView(canvas, ref scrollPosition, source, true);

            var lMain = new Listing_Standard();
            lMain.ColumnWidth = source.width;
            lMain.Begin(source);

            lMain.GapLine();
            lMain.CheckboxLabeled("Multi Keybindings", ref settings.useMultiKeys, "Check to enable binding multiple keystrokes to each keybinding");
            lMain.CheckboxLabeled("Architect Hotkeys", ref settings.useArchitectHotkeys, "Check to enable the use of hotkeys to select subtabs in the Architect Tab.");
            lMain.GapLine();
            //lMain.Label("The game MUST be restarted to add or remove keybinding options.  Set keybinds in the standard menu.");


            lMain.Gap();
            lMain.Gap();
            lMain.CheckboxLabeled("Direct Hotkeys", ref settings.useDirectHotkeys, "Check to enable direct hotkeys to any designator");
            lMain.GapLine();

            if (settings.useDirectHotkeys)
            {
                lMain.Gap();
                lMain.End();

                var grid = new GridLayout(new Rect(source.xMin + indent, source.yMin + lMain.CurHeight, source.width - 2*indent, source.height - lMain.CurHeight), 5, 1, 0, 0);

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
            settings.Write();
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

            DirectKeys.BuildDirectKeyDefs(); 
        }

        private void RemoveButtons(GridLayout grid)
        {
            var rect = grid.GetCellRect(4, 0, 1);
            var listing = new Listing_Standard();
            var rect2 = new Rect(rect.xMax - 30f, rect.yMin, 30f, rect.height);
            listing.Begin(rect2);

            for (int i = 0; i < DirectKeys.directKeys.Count; i++)
            {
                if (listing.ButtonText("-"))
                {
                    DirectKeys.directKeys.RemoveAt(i);
                }
            }

            listing.End();
        }

        private void DesignatorFloatMenus(GridLayout grid)
        {
            var rect = grid.GetCellRect(2, 0, 1);
            var listing = new Listing_Standard();
            listing.Begin(rect);

            for (int index = 0; index < DirectKeys.directKeys.Count; index++)
            {
                if (listing.ButtonText(DirectKeys.directKeys[index].desLabelCap))
                {
                    var options = GetDesFloatMenuOptions(index);

                    FloatMenu window = new FloatMenu(options, "Select Category", false);
                    Find.WindowStack.Add(window);
                }
            }

            listing.Gap();

            if (listing.ButtonText("Add Hotkey", "Add additional direct hotkeys"))
            {
                DirectKeys.directKeys.Add(new DirectKeyData());
            }

            listing.End();
        }

        private void CategoryFloatMenus(GridLayout grid)
        {
            var rect = grid.GetCellRect(0, 0, 2);
            var listing = new Listing_Standard();
            listing.Begin(rect);

            for (int index = 0; index < DirectKeys.directKeys.Count; index++)
            {
                if (listing.ButtonTextLabeled("Direct Hotkey " + index.ToString(), DirectKeys.directKeys[index].desCategoryLabelCap))
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

            if (DirectKeys.directKeys[index].desCategoryLabelCap != "None")
            {
                var designators = DirectKeys.directKeys[index].GetDesCategory().AllResolvedDesignators;
                foreach (var designator in designators)
                {
                    options.Add(new FloatMenuOption(designator.LabelCap, delegate ()
                    {
                        DirectKeys.directKeys[buttonNum].desLabelCap = designator.LabelCap;
                    }));
                }
            }
            else
            {
                options.Add(new FloatMenuOption("None", delegate ()
                {
                    DirectKeys.directKeys[buttonNum].desLabelCap = "None";
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
                    DirectKeys.directKeys[buttonNum].desCategoryLabelCap = desCat.LabelCap;
                }));
            }

            return options;
        }
    }
}




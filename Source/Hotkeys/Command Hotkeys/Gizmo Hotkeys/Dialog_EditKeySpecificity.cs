using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using UnityEngine;
using Verse;

namespace Hotkeys
{
    class Dialog_EditKeySpecificity : Window
    {
        public Command Command { private get; set; }

        private bool name;
        private bool type;
        private bool desc;

        public override void DoWindowContents(Rect canvas)
        {
            var listing = new Listing_Standard();
            listing.Begin(canvas);
            Text.Anchor = TextAnchor.MiddleCenter;
            Text.Font = GameFont.Medium;
            listing.Label("Edit Key Specificity");
            GenUI.ResetLabelAlign();
            Text.Font = GameFont.Small;
            listing.GapLine();

            listing.Label("Select what information should be used to assign Hotkeys to commands.  For most purposes just the Name is enough.  Where there is overlap or custom behavior is desired, the most specified key is selected first.");
            listing.GapLine();
            Text.Anchor = TextAnchor.MiddleCenter;
            listing.Label("Key: " + Command.Key(name, type, desc));
            GenUI.ResetLabelAlign();
            listing.GapLine();

            listing.CheckboxLabeled("Name: " + Command.Key(true, false, false), ref name);
            listing.CheckboxLabeled("Type: " + Command.Key(false, true, false), ref type);
            listing.CheckboxLabeled("Desc: " + Command.Key(false, false, true), ref desc);

            listing.Gap();
            listing.Gap();
            listing.Gap();

            if (listing.ButtonText("Close"))
            {
                Close();
            }
            
            listing.End();
        }

        public override void PreClose()
        {
            if (GizmoKeys.KeyPresent(Command))
            {
                GizmoKeys.RemoveKey(Command);
                GizmoKeys.AddKey(Command, name, type, desc);
                Messages.Message("Edited Gizmo Hotkey '" + Command.LabelCap + "'.", MessageTypeDefOf.TaskCompletion, false);
            }
            if (DirectKeys.KeyPresent(Command))
            {
                DirectKeys.RemoveKey(Command);
                DirectKeys.AddKey(Command, name, type, desc);
                Messages.Message("Edited Direct Hotkey '" + Command.LabelCap + "'.", MessageTypeDefOf.TaskCompletion, false);

            }

            Hotkeys.settings.Write();
        }

        public override void PreOpen()
        {
            base.PreOpen();

            if (Command.hotKey.defName?.Contains(Command.Key(true, false, false)) ?? false)
            {
                name = true;
            }
            if (Command.hotKey.defName?.Contains(Command.Key(false, true, false)) ?? false)
            {
                type = true;
            }
            if (Command.hotKey.defName?.Contains(Command.Key(false, false, true)) ?? false)
            {
                desc = true;
            }
        }
    }
}

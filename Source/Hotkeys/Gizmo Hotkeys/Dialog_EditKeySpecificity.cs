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

        private readonly int descDisplayLength = 20;

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

            listing.CheckboxLabeled("Name: " + Command.LabelCap, ref name);
            listing.CheckboxLabeled("Type: " + Command.GetType().ToString(), ref type);

            string description = Command.Desc.Truncate(descDisplayLength);

            listing.CheckboxLabeled("Desc: " + description, ref desc);
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

            if (Command.hotKey.defName.Contains(Command.LabelCap))
            {
                name = true;
            }
            if (Command.hotKey.defName.Contains(Command.GetType().ToString()))
            {
                type = true;
            }

            string description = Command.Desc;

            if (description.Length > Extensions.descKeyLength)
            {
                description = description.Substring(0, Extensions.descKeyLength);
            }

            if (Command.hotKey.defName.Contains(description))
            {
                desc = true;
            }
        }
    }
}

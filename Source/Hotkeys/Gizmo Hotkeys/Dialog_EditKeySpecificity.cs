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

        private readonly int descDisplayLength = 100;

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
            listing.Label("Key: ");
            GenUI.ResetLabelAlign();
            listing.GapLine();

            listing.CheckboxLabeled("Name: " + Command.LabelCap, ref name);
            listing.CheckboxLabeled("Type: " + Command.GetType().ToString(), ref type);

            string description = Command.Desc;
            if (description.Length > descDisplayLength)
            {
                description = description.Substring(0, descDisplayLength);
            }

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
            
        }
    }
}

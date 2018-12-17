using Harmony;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.Sound;
using System.Collections.Generic;


namespace Hotkeys
{
    [HarmonyPatch(typeof(Game), nameof(Game.UpdatePlay))]
    public class Patch_DirectHotkeys
    {
        static void Prefix()
        {
            if (DirectKeys.gizmoTriggered) { DirectKeys.gizmoTriggered = false; return; }
            if (Event.current.type != EventType.KeyDown) { return; }
            if (!Hotkeys.settings.useDirectHotkeys) { return; }

            var directKeys = DirectKeys.directKeys;

            foreach (DirectKeyData directKey in directKeys.Values)
            {
                if (directKey.keyDef.JustPressed)
                {
                    var designator = directKey.Designator;
                    if (designator != null)
                    {
                        SoundDefOf.SelectDesignator.PlayOneShotOnCamera((Map)null);
                        Find.DesignatorManager.Select(designator);
                    }
                }
            }

            DirectKeys.gizmoTriggered = false;
        }
    }
}




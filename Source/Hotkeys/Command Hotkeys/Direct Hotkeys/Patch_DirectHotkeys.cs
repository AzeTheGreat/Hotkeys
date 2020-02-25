using HarmonyLib;
using UnityEngine;
using Verse;
using Verse.Sound;


namespace Hotkeys
{
    [HarmonyPatch(typeof(Game), nameof(Game.UpdatePlay))]
    public class Patch_DirectHotkeys
    {
        static void Prefix()
        {
            if (DirectKeys.gizmoTriggered) { DirectKeys.gizmoTriggered = false; return; }
            if (Event.current.type != EventType.KeyDown) { return; }
            if (!Hotkeys.settings.useCommandHotkeys) { return; }

            var directKeys = DirectKeys.directKeys;

            foreach (DirectKeyData directKey in directKeys.Values)
            {
                if (directKey.keyDef.JustPressed)
                {
                    var designator = directKey.Designator;
                    if (designator != null)
                    {
                        designator.activateSound.PlayOneShotOnCamera(null);
                        Find.DesignatorManager.Select(designator);
                    }
                }
            }

            DirectKeys.gizmoTriggered = false;
        }
    }
}




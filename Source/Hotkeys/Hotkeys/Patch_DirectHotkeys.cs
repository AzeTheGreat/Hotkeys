using Harmony;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.Sound;


namespace Hotkeys
{
    [HarmonyPatch(typeof(Game), nameof(Game.UpdatePlay))]
    public class Patch_DirectHotkeys
    {
        static void Postfix()
        {
            if (Event.current.type != EventType.KeyDown) { return; }
            if (!Hotkeys.settings.useDirectHotkeys) { return; }
            if (Find.Selector.SelectedObjects.Count > 0) { return; }

            var directKeys = DirectKeys.directKeys;
            for (int i = 0; i < directKeys.Count; i++)
            {
                if (directKeys[i].keyDef.JustPressed)
                {
                    var designator = directKeys[i].Designator;
                    if (designator != null)
                    {
                        SoundDefOf.SelectDesignator.PlayOneShotOnCamera((Map)null);
                        Find.DesignatorManager.Select(designator);
                    }
                }
            }
        }
    }
}


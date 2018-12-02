using Harmony;
using System.Collections.Generic;
using UnityEngine;
using Verse;


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

            List<KeyBindingDef> designatorKeys = DefDatabase<KeyBindingDef>.AllDefsListForReading.FindAll(x => x.category == DefDatabase<KeyBindingCategoryDef>.GetNamed("DirectHotkeys"));

            for (int i = 0; i < designatorKeys.Count; i++)
            {
                if (designatorKeys[i].JustPressed)
                {
                    var designator = DirectKeys.GetDesignator(i);
                    if (designator != null)
                    {
                        Find.DesignatorManager.Select(designator);
                    }
                }
            }
        }
    }
}


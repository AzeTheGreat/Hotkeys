using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Harmony;
using Verse;

namespace Hotkeys
{
    [HarmonyPatch(typeof(Command), nameof(Command.GizmoOnGUI))]
    class Patch_ApplyGizmoHotkeys
    {
        static void Prefix(Command __instance)
        {
            if (GizmoKeys.gizmoKeys.TryGetValue(__instance.LabelCap, out GizmoKeyData gizmoKeyData))
            {
                __instance.hotKey = gizmoKeyData.keyDef;
            }

            var direct = DirectKeys.directKeys.FirstOrDefault(x => x.desLabelCap == __instance.LabelCap);
            if (direct != null)
            {
                __instance.hotKey = direct.keyDef;
            }
        }
    }
}

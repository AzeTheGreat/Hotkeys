using Harmony;
using Verse;


namespace Hotkeys
{
    [HarmonyPatch(typeof(Command), nameof(Command.GizmoOnGUI))]
    public class Patch_CheckGizmoKeybindings
    {
        static void Postfix(Command __instance)
        {
            if(__instance.hotKey == null) { return; }

            if (__instance.hotKey.JustPressed)
            {
                DirectKeys.gizmoTriggered = true;
            }
        }
    }
}




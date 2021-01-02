using HarmonyLib;
using Verse;


namespace Hotkeys
{
    [HarmonyPatch(typeof(Command), "GizmoOnGUIInt")]
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

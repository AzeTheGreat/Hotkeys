using Harmony;
using Verse;


namespace Hotkeys
{
    [HarmonyPatch(typeof(Command), nameof(Command.GizmoOnGUI))]
    class Patch_ApplyGizmoHotkeys
    {
        static void Prefix(Command __instance)
        {
            //Type type = __instance.GetType();
            //string typeName = type.ToString();
            //Log.Message(typeName);

            //if (__instance.LabelCap == null) { return; }

            DirectKeyData direct = DirectKeys.GetKey(__instance);
            if (direct != null)
            {
                __instance.hotKey = direct.keyDef;
            }

            GizmoKeyData gizmo = GizmoKeys.GetKey(__instance);
            if (gizmo != null)
            {
                __instance.hotKey = gizmo.keyDef;
            }
        }
    }

    [HarmonyPatch(typeof(Command), MethodType.Constructor)]
    class Patch_CommandConstructor
    {
        static void Postfix()
        {
            //Log.Message("Constructed");
        }
    }
}

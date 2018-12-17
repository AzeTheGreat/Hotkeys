using Harmony;
using Verse;
using System.Collections;
using System.Collections.Generic;


namespace Hotkeys
{
    [HarmonyPatch(typeof(Command), nameof(Command.GizmoOnGUI))]
    class Patch_ApplyGizmoHotkeys
    {
        static void Prefix(Command __instance)
        {
            //var watch = System.Diagnostics.Stopwatch.StartNew();
            //for (int i = 0; i < 10000; i++)
            //{
                List<string> keys = __instance.KeyList();

                foreach (string key in keys)
                {
                    DirectKeyData direct = DirectKeys.GetKey(key);
                    if (direct != null)
                    {
                        __instance.hotKey = direct.keyDef;
                        //return;
                    }

                    GizmoKeyData gizmo = GizmoKeys.GetKey(key);
                    if (gizmo != null)
                    {
                        __instance.hotKey = gizmo.keyDef;
                        //return;
                    }
                }
            //}
            //watch.Stop();
            //Log.Message(watch.ElapsedMilliseconds.ToString());
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

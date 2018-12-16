using Harmony;
using System.Collections.Generic;
using Verse;
using UnityEngine;
using RimWorld;


namespace Hotkeys
{
    [HarmonyPatch(typeof(Command), nameof(Command.GizmoOnGUI))]
    class Patch_AssignGizmoKey
    {
        static void Postfix(ref GizmoResult __result, Command __instance)
        {
            if (__result.State != GizmoState.OpenedFloatMenu) { return; }

            KeyBindingDef keyDef = DefDatabase<KeyBindingDef>.AllDefsListForReading.Find(x => x.defName == "Hotkeys_GizmoAssigner");
            if (Event.current.button == 1 && keyDef.IsDown)
            {
                List<FloatMenuOption> options = new List<FloatMenuOption>();
                bool alreadyDirect = DirectKeys.KeyPresent(__instance);
                bool alreadyGizmo = GizmoKeys.KeyPresent(__instance);

                if (!alreadyGizmo)
                {
                    options.Add(new FloatMenuOption("Make Hotkey", delegate ()
                    {
                        if (alreadyDirect) { DirectToGizmo(__instance); }
                        if (!alreadyDirect) { MakeGizmoHotkey(__instance); }
                    }));
                }
                if (!alreadyDirect && __instance is Designator)
                {
                    options.Add(new FloatMenuOption("Make Direct Hotkey", delegate ()
                    {
                        if (alreadyGizmo) { GizmoToDirect(__instance); }
                        if (!alreadyGizmo) { MakeDirectHotkey(__instance); }
                    }));
                }
                if (alreadyGizmo || alreadyDirect)
                {
                    options.Add(new FloatMenuOption("Clear Hotkey", delegate ()
                    {
                        ClearHotkey(__instance, alreadyDirect, alreadyGizmo);
                    }));
                    options.Add(new FloatMenuOption("Edit Key", delegate ()
                    {
                        var edit = new Dialog_EditKeySpecificity();
                        edit.Command = __instance;
                        Find.WindowStack.Add(edit);
                    }));
                }
                
                FloatMenu window = new FloatMenu(options, "Select Category", false);
                Find.WindowStack.Add(window);

                __result = new GizmoResult(GizmoState.Mouseover, null);
            }
        }

        private static void GizmoToDirect(Command __instance)
        {
            GizmoKeys.RemoveKey(__instance);
            DirectKeys.AddKey(__instance);
            Messages.Message("Gizmo Hotkey '" + __instance.LabelCap + "' changed to Direct Hotkey.", MessageTypeDefOf.TaskCompletion, false);
        }

        private static void DirectToGizmo(Command __instance)
        {
            DirectKeys.RemoveKey(__instance);
            GizmoKeys.AddKey(__instance);
            Messages.Message("Direct Hotkey '" + __instance.LabelCap + "' changed to Gizmo Hotkey.", MessageTypeDefOf.TaskCompletion, false);
        }

        private static void MakeGizmoHotkey(Command __instance)
        {
            GizmoKeys.AddKey(__instance);
            Messages.Message("Gizmo Hotkey '" + __instance.LabelCap + "' added.", MessageTypeDefOf.TaskCompletion, false);
        }

        private static void MakeDirectHotkey(Command __instance)
        {
            DirectKeys.AddKey(__instance);
            Messages.Message("Direct Hotkey '" + __instance.LabelCap + "' added.", MessageTypeDefOf.TaskCompletion, false);
        }

        private static void ClearHotkey(Command __instance, bool alreadyDirect, bool alreadyGizmo)
        {
            if (alreadyDirect)
            {
                DirectKeys.RemoveKey(__instance);
                Messages.Message("Direct Hotkey '" + __instance.LabelCap + "' cleared.", MessageTypeDefOf.TaskCompletion, false);
            }
            if (alreadyGizmo)
            {
                GizmoKeys.RemoveKey(__instance);
                Messages.Message("Gizmo Hotkey '" + __instance.LabelCap + "' cleared.", MessageTypeDefOf.TaskCompletion, false);
            }
        }
    }
}

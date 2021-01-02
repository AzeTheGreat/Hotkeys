using HarmonyLib;
using System.Collections.Generic;
using Verse;
using UnityEngine;
using RimWorld;


namespace Hotkeys
{
    [HarmonyPatch(typeof(Command), "GizmoOnGUIInt")]
    static class Patch_AssignGizmoKey
    {
        [HarmonyPriority(Priority.First)]
        static void Postfix(ref GizmoResult __result, Command __instance)
        {
            if (!Hotkeys.settings.useCommandHotkeys || __result.State != GizmoState.OpenedFloatMenu)
                return;

            Log.Message("PATCH Proceeds: " + __result.State);

            var keyDef = DefDatabase<KeyBindingDef>.AllDefsListForReading.Find(x => x.defName == "Hotkeys_GizmoAssigner");
            if (!(Event.current.button == 1 && keyDef.IsDown))
                return;

            var options = GetFloatMenuOptions(__instance);
            var window = new FloatMenu(options, "Select Category", false);
            Find.WindowStack.Add(window);
            __result = new GizmoResult(GizmoState.Clear, null);
        }

        private static List<FloatMenuOption> GetFloatMenuOptions(Command __instance)
        {
            var options = new List<FloatMenuOption>();
            bool alreadyDirect = DirectKeys.KeyPresent(__instance);
            bool alreadyGizmo = GizmoKeys.KeyPresent(__instance);

            if (!alreadyGizmo && !alreadyDirect)
            {
                options.Add(new FloatMenuOption("Make Hotkey", () => __instance.MakeGizmoHotkey()));
                if (__instance is Designator)
                    options.Add(new FloatMenuOption("Make Direct Hotkey", () => __instance.MakeDirectHotkey()));
            }

            if (alreadyGizmo)
            {
                options.Add(new FloatMenuOption("Edit Key", () =>
                {
                    var edit = new Dialog_EditKeySpecificity();
                    edit.Command = __instance;
                    Find.WindowStack.Add(edit);
                }));
            }

            if (alreadyGizmo || alreadyDirect)
                options.Add(new FloatMenuOption("Clear Hotkey", () => __instance.ClearHotkey()));

            return options;
        }

        private static void MakeGizmoHotkey(this Command __instance)
        {
            GizmoKeys.AddKey(__instance);
            Message(__instance, "Gizmo", "added");
        }

        private static void MakeDirectHotkey(this Command __instance)
        {
            DirectKeys.AddKey(__instance);
            Message(__instance, "Direct", "added");
        }

        private static void ClearHotkey(this Command __instance)
        {
            if (DirectKeys.KeyPresent(__instance))
            {
                DirectKeys.RemoveKey(__instance);
                Message(__instance, "Direct", "cleared");
            }
            if (GizmoKeys.KeyPresent(__instance))
            {
                GizmoKeys.RemoveKey(__instance);
                Message(__instance, "Gizmo", "cleared");
            }
        }

        private static void Message(Command __instance, string keyType, string change) => 
            Messages.Message(keyType + " Hotkey " + __instance.LabelCap + " " + change, MessageTypeDefOf.TaskCompletion, false);
    }
}

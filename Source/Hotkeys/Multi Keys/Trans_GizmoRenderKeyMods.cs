using HarmonyLib;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using Verse;
using UnityEngine;

namespace Hotkeys
{
    // Transpiler to show multi key labels
    [HarmonyPatch(typeof(Command), nameof(Command.GizmoOnGUI))]
    public class Trans_GizmoRenderKeyMods
    {
        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            bool afterTarget = false;
            MethodInfo Contains = AccessTools.Method(typeof(HashSet<KeyCode>), nameof(HashSet<KeyCode>.Contains));
            MethodInfo ToStringReadable = AccessTools.Method(typeof(GenText), nameof(GenText.ToStringReadable));

            foreach (CodeInstruction i in instructions)
            {
                if (i.opcode == OpCodes.Callvirt && i.OperandIs(Contains))
                {
                    afterTarget = true;
                }

                if (i.opcode == OpCodes.Brtrue && afterTarget)
                {
                    afterTarget = false;

                    yield return new CodeInstruction(OpCodes.Pop);

                    yield return new CodeInstruction(OpCodes.Ldarg_0);
                    yield return new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(typeof(Command), nameof(Command.hotKey)));
                    yield return new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(Trans_GizmoRenderKeyMods), nameof(Trans_GizmoRenderKeyMods.CheckRenderedKeys)));

                    yield return i;
                    continue;
                }

                if (i.opcode == OpCodes.Call && i.OperandIs(ToStringReadable))
                {
                    yield return i;

                    yield return new CodeInstruction(OpCodes.Pop);

                    yield return new CodeInstruction(OpCodes.Ldarg_0);
                    yield return new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(typeof(Command), nameof(Command.hotKey)));
                    yield return new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(Trans_GizmoRenderKeyMods), nameof(Trans_GizmoRenderKeyMods.RenderKeyMods)));

                    continue;
                }

                yield return i;
            }
        }

        private static bool CheckRenderedKeys(KeyBindingDef keyDef)
        {
            if (!KeyMods.drawnGimoKeyDefs.Contains(keyDef))
            {
                KeyMods.drawnGimoKeyDefs.Add(keyDef);
                return false;
            }

            return true;
        }

        private static string RenderKeyMods(KeyBindingDef keyDef)
        {
            return keyDef.HotkeyLabel();
        }
    }

    // Patch to clear drawnGizmoKeyDefs at start of GizmoGridDrawer
    [HarmonyPatch(typeof(GizmoGridDrawer), nameof(GizmoGridDrawer.DrawGizmoGrid))]
    public class Patch_ClearDrawnGizmoKeyDefs
    {
        static void Prefix()
        {
            KeyMods.drawnGimoKeyDefs.Clear();
        }
    }
}

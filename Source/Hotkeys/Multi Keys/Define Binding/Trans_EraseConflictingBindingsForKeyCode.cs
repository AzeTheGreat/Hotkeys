using HarmonyLib;
using System.Collections.Generic;
using System.Reflection.Emit;
using Verse;

namespace Hotkeys
{
    // Clear modifiers as well when keybinds conflict.
    [HarmonyPatch(typeof(KeyPrefsData), nameof(KeyPrefsData.EraseConflictingBindingsForKeyCode))]
    public class Trans_EraseConflictingBindingsForKeyCode
    {
        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var targetA = AccessTools.Field(typeof(KeyBindingData), nameof(KeyBindingData.keyBindingA));
            var targetB = AccessTools.Field(typeof(KeyBindingData), nameof(KeyBindingData.keyBindingB));

            foreach (var i in instructions)
            {
                yield return i;

                if (i.opcode == OpCodes.Stfld && i.OperandIs(targetA))
                {
                    yield return new CodeInstruction(OpCodes.Ldloc_1);
                    yield return new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(Trans_EraseConflictingBindingsForKeyCode), nameof(Trans_EraseConflictingBindingsForKeyCode.ClearA)));
                }
                else if (i.opcode == OpCodes.Stfld && i.OperandIs(targetA))
                {
                    yield return new CodeInstruction(OpCodes.Ldloc_1);
                    yield return new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(Trans_EraseConflictingBindingsForKeyCode), nameof(Trans_EraseConflictingBindingsForKeyCode.ClearA)));
                } 
            }
        }

        private static void ClearA(KeyBindingDef keyBindingDef) => Patch_KeyBindDrawing.ResetModifierList(KeyPrefs.BindingSlot.A, keyBindingDef);
        private static void ClearB(KeyBindingDef keyBindingDef) => Patch_KeyBindDrawing.ResetModifierList(KeyPrefs.BindingSlot.A, keyBindingDef);
    }
}
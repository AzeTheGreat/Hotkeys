using Harmony;
using RimWorld;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using Verse;

namespace Hotkeys
{
    // Transpiler to clear old modifier lists when keybinding settings window is opened.
    [HarmonyPatch(typeof(Dialog_Options), nameof(Dialog_Options.DoWindowContents))]
    public class Trans_KeybindingSettingsOpened
    {
         static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            MethodInfo Add = AccessTools.Method(typeof(WindowStack), nameof(WindowStack.Add));
            bool afterTarget = false;

            foreach (CodeInstruction i in instructions)
            {
                if (i.opcode == OpCodes.Ldstr && (string)i.operand == "KeyboardConfig")
                {
                    afterTarget = true;
                }
                if (i.opcode == OpCodes.Callvirt && i.operand == Add && afterTarget)
                {
                    yield return i;
                    yield return new CodeInstruction(OpCodes.Call,
                        AccessTools.Method(typeof(Trans_KeybindingSettingsOpened), nameof(Trans_KeybindingSettingsOpened.BackupKeyBindings)));
                    afterTarget = false;
                    continue;
                }
                yield return i;
            }
        }

        private static void BackupKeyBindings()
        {
            if (!Hotkeys.settings.useMultiKeys) { return; }

            Global.oldKeyModifiers = new Dictionary<string, KeyModData>(Hotkeys_Save.saved.allKeyModifiers);
        }
    }

}


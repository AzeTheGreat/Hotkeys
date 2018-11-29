using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using Verse;
using RimWorld;
using Harmony;

namespace Hotkeys
{
    // Transpiler to reset modifiers if closed
    [HarmonyPatch(typeof(Dialog_KeyBindings), nameof(Dialog_KeyBindings.DoWindowContents))]
    public class HotkeysPatch_KeybindingSettingsClosed
    {
        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            MethodInfo Close = AccessTools.Method(typeof(Window), nameof(Window.Close));
            bool afterTarget = false;

            foreach (CodeInstruction i in instructions)
            {
                if (i.opcode == OpCodes.Ldstr && (string)i.operand == "CancelButton")
                {
                    afterTarget = true;
                }
                if (i.opcode == OpCodes.Callvirt && i.operand == Close && afterTarget)
                {
                    yield return i;
                    yield return new CodeInstruction(OpCodes.Call,
                        AccessTools.Method(typeof(HotkeysPatch_KeybindingSettingsClosed), nameof(HotkeysPatch_KeybindingSettingsClosed.RestoreKeyBindings)));
                    afterTarget = false;
                    continue;
                }
                yield return i;
            }
        }

        private static void RestoreKeyBindings()
        {
            if (!Hotkeys.settings.useMultiKeys) { return; }

            Hotkeys_Save.saved.allKeyModifiers = new Dictionary<string, KeyModData>(HotkeysGlobal.oldKeyModifiers);
        }
    }

}


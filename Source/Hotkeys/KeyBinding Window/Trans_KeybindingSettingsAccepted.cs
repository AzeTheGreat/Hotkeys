using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using Verse;
using RimWorld;
using Harmony;

namespace Hotkeys
{
    // Transpiler to rebuild overlap dictionary when accepted
    [HarmonyPatch(typeof(Dialog_KeyBindings), nameof(Dialog_KeyBindings.DoWindowContents))]
    public class Trans_KeybindingSettingsAccepted
    {
        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            MethodInfo Close = AccessTools.Method(typeof(Window), nameof(Window.Close));
            bool afterTarget = false;

            foreach (CodeInstruction i in instructions)
            {
                if (i.opcode == OpCodes.Ldstr && (string)i.operand == "OK")
                {
                    afterTarget = true;
                }
                if (i.opcode == OpCodes.Callvirt && i.operand == Close && afterTarget)
                {
                    yield return i;
                    yield return new CodeInstruction(OpCodes.Call,
                        AccessTools.Method(typeof(Trans_KeybindingSettingsAccepted), nameof(Trans_KeybindingSettingsAccepted.RebuildOverlapDict)));
                    afterTarget = false;
                    continue;
                }
                yield return i;
            }
        }

        private static void RebuildOverlapDict()
        {
            if (!Hotkeys.settings.useMultiKeys) { return; }

            KeyMods.BuildOverlappingKeys();
        }
    }

}


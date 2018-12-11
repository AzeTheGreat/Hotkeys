using Harmony;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using Verse;

namespace Hotkeys
{
    // Transpiler to generate KeyModData at proper time
    [HarmonyPatch(typeof(KeyPrefs), nameof(KeyPrefs.Init))]
    public class Trans_InjectKeyModData
    {
        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            MethodInfo AddMissingDefaultBindings = AccessTools.Method(typeof(KeyPrefsData), nameof(KeyPrefsData.AddMissingDefaultBindings));

            foreach (CodeInstruction i in instructions)
            {
                if (i.opcode == OpCodes.Callvirt && i.operand == AddMissingDefaultBindings)
                {
                    yield return i;
                    yield return new CodeInstruction(OpCodes.Call,
                        AccessTools.Method(typeof(Trans_InjectKeyModData), nameof(Trans_InjectKeyModData.InjectKeyBindMods)));
                    continue;
                }
                yield return i;
            }
        }

        private static void InjectKeyBindMods()
        {
            KeyMods.BuildKeyModData();
        }
    }
}

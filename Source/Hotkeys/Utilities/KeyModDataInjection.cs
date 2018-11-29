using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Reflection.Emit;
using Verse;
using RimWorld;
using RimWorld.Planet;
using Harmony;
using UnityEngine;

namespace Hotkeys
{
    // Transpiler to generate KeyModData at proper time
    [HarmonyPatch(typeof(KeyPrefs), nameof(KeyPrefs.Init))]
    public class HotkeysPatch_InjectKeyModData
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
                        AccessTools.Method(typeof(HotkeysPatch_InjectKeyModData), nameof(HotkeysPatch_InjectKeyModData.InjectKeyBindMods)));
                    continue;
                }
                yield return i;
            }
        }

        private static void InjectKeyBindMods()
        {
            Log.Message("KeyBindMods Injected");
            HotkeysGlobal.BuildKeyModData();
        }
    }
}

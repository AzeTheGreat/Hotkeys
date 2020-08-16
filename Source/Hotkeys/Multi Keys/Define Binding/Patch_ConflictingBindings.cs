using HarmonyLib;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;

namespace Hotkeys
{
    [HarmonyPatch(typeof(KeyPrefsData), "ConflictingBindings")]
    static class Patch_ConflictingBindings
    {
        static IEnumerable<KeyBindingDef> Postfix(IEnumerable<KeyBindingDef> conflictingDefs, KeyBindingDef keyDef, KeyCode code, KeyPrefsData __instance)
        {
            foreach (var def in conflictingDefs)
            {
                if (CheckAllKeys(keyDef, def, code, __instance))
                    yield return def;
            }
        }

        private static bool CheckAllKeys(KeyBindingDef assignedKeyDef, KeyBindingDef existingKeyDef, KeyCode assignedCode, KeyPrefsData __instance)
        {
            __instance.keyPrefs.TryGetValue(assignedKeyDef, out KeyBindingData prefDataAssigned);
            __instance.keyPrefs.TryGetValue(existingKeyDef, out KeyBindingData prefDataExisting);

            var assignedCodes = new List<KeyCode>();
            var existingCodes = new List<KeyCode>();

            if (assignedCode == prefDataAssigned.keyBindingA)
                assignedCodes = assignedKeyDef.ModifierData().keyBindModsA;
            if (assignedCode == prefDataAssigned.keyBindingB)
                assignedCodes = assignedKeyDef.ModifierData().keyBindModsB;

            if (prefDataExisting.keyBindingA == assignedCode)
                existingCodes = existingKeyDef.ModifierData().keyBindModsA;
            if (prefDataExisting.keyBindingB == assignedCode)
                existingCodes = existingKeyDef.ModifierData().keyBindModsB;

            return assignedCodes.OrderBy(i => i).SequenceEqual(existingCodes.OrderBy(i => i));
        }
    }
}
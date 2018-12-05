using Verse;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace Hotkeys
{
    // Custom Class
    public static class BindingConflicts
    {
        public static List<KeyBindingDef> ConflictingBindings(KeyBindingDef keyDef, KeyCode code, KeyPrefsData __instance)
        {
            List<KeyBindingDef> allKeyDefs = DefDatabase<KeyBindingDef>.AllDefsListForReading;
            List<KeyBindingDef> conflictingDefs = new List<KeyBindingDef>();

            foreach (var def in allKeyDefs)
            {
                KeyBindingData prefData;
                if (def != keyDef
                    && ((def.category == keyDef.category && def.category.selfConflicting) || keyDef.category.checkForConflicts.Contains(def.category)
                    || (keyDef.extraConflictTags != null && def.extraConflictTags != null && keyDef.extraConflictTags.Any((string tag) => def.extraConflictTags.Contains(tag))))
                    && __instance.keyPrefs.TryGetValue(def, out prefData) && (CheckAllKeys(keyDef, def, prefData, code, __instance)))
                {
                    conflictingDefs.Add(def);
                }
            }
            return conflictingDefs;
        }

        private static bool CheckAllKeys(KeyBindingDef assignedKeyDef, KeyBindingDef existingKeyDef, KeyBindingData prefDataExisting, KeyCode assignedCode, KeyPrefsData __instance)
        {
            var settings = Hotkeys.settings;
            if (settings == null) { settings = LoadedModManager.GetMod<Hotkeys>().GetSettings<Hotkeys_Settings>(); }

            __instance.keyPrefs.TryGetValue(assignedKeyDef, out KeyBindingData prefDataAssigned);

            List<KeyCode> assignedCodes = new List<KeyCode>();
            List<KeyCode> existingCodes = new List<KeyCode>();

            if (assignedCode == prefDataAssigned.keyBindingA)
            {
                List<KeyCode> aCodes = assignedKeyDef.ModifierData().keyBindModsA;
                assignedCodes.AddRange(aCodes);
            }
            if (assignedCode == prefDataAssigned.keyBindingB)
            {
                List<KeyCode> aCodes = assignedKeyDef.ModifierData().keyBindModsB;
                assignedCodes.AddRange(aCodes);
            }

            if (prefDataExisting.keyBindingA == assignedCode)
            {
                List<KeyCode> eCodes = existingKeyDef.ModifierData().keyBindModsA;
                existingCodes.AddRange(eCodes);
                return assignedCodes.OrderBy(i => i).SequenceEqual(existingCodes.OrderBy(i => i));
            }
            if (prefDataExisting.keyBindingB == assignedCode)
            {
                List<KeyCode> eCodes = existingKeyDef.ModifierData().keyBindModsB;
                existingCodes.AddRange(eCodes);
                return assignedCodes.OrderBy(i => i).SequenceEqual(existingCodes.OrderBy(i => i));
            }

            else
            {
                return false;
            }
        }
    }
}
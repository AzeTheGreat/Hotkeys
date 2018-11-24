using Harmony;
using Verse;
using System.Reflection;
using RimWorld;
using UnityEngine;
using System.Collections.Generic;
using Verse.Sound;
using RimWorld.Planet;
using System.Linq;

namespace Hotkeys
{
    public static class DictKeyListKeysExtension
    {
        public static void Build(this Dictionary<KeyCode, ExposableList<KeyCode>> dict)
        {
            if (dict == null) { dict = new Dictionary<KeyCode, ExposableList<KeyCode>>(); }

            Log.Message("Building Overlap List");

            List<KeyBindingDef> allKeyDefs = DefDatabase<KeyBindingDef>.AllDefsListForReading;
            KeyPrefsData keyPrefData = KeyPrefs.KeyPrefsData.Clone();

            List<KeyCode> overlappingTriggerKeys = new List<KeyCode>();
            foreach (KeyBindingDef keyDef in allKeyDefs)
            {
                overlappingTriggerKeys.Add(keyPrefData.GetBoundKeyCode(keyDef, KeyPrefs.BindingSlot.A));
                overlappingTriggerKeys.Add(keyPrefData.GetBoundKeyCode(keyDef, KeyPrefs.BindingSlot.B));
            }

            overlappingTriggerKeys.Distinct().ToList();

            foreach (KeyBindingDef keyDef in allKeyDefs)
            {

                KeyCode keyCodeA = keyPrefData.GetBoundKeyCode(keyDef, KeyPrefs.BindingSlot.A);
                KeyCode keyCodeB = keyPrefData.GetBoundKeyCode(keyDef, KeyPrefs.BindingSlot.B);

                ExposableList<KeyCode> newKeyCodes = new ExposableList<KeyCode>();
                ExposableList<KeyCode> storedKeyCodes = new ExposableList<KeyCode>();

                if (overlappingTriggerKeys.Contains(keyCodeA))
                {
                    HotkeysLate.settings.keyBindModsA.TryGetValue(keyDef.defName, out newKeyCodes);
                    dict.TryGetValue(keyCodeA, out storedKeyCodes);

                    if (newKeyCodes == null) { newKeyCodes = new ExposableList<KeyCode>(); }
                    if (storedKeyCodes == null) { storedKeyCodes = new ExposableList<KeyCode>(); }

                    ExposableList<KeyCode> modifierKeyCodes = storedKeyCodes.Concat(newKeyCodes).ToList() as ExposableList<KeyCode>;

                    if(modifierKeyCodes == null) { modifierKeyCodes = new ExposableList<KeyCode>(); }
                    modifierKeyCodes.Distinct().ToList();

                    dict[keyCodeA] = modifierKeyCodes;
                }

                if (overlappingTriggerKeys.Contains(keyCodeB))
                {
                    HotkeysLate.settings.keyBindModsB.TryGetValue(keyDef.defName, out newKeyCodes);
                    dict.TryGetValue(keyCodeB, out storedKeyCodes);

                    if (newKeyCodes == null) { newKeyCodes = new ExposableList<KeyCode>(); }
                    if (storedKeyCodes == null) { storedKeyCodes = new ExposableList<KeyCode>(); }

                    ExposableList<KeyCode> modifierKeyCodes = storedKeyCodes.Concat(newKeyCodes).ToList() as ExposableList<KeyCode>;

                    if (modifierKeyCodes == null) { modifierKeyCodes = new ExposableList<KeyCode>(); }
                    modifierKeyCodes.Distinct().ToList();

                    dict[keyCodeB] = modifierKeyCodes;
                }
            }
        }
    }
}

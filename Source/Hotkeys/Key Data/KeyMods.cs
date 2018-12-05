<<<<<<< HEAD:Source/Hotkeys/Key Data/KeyMods.cs
﻿using System.Collections.Generic;
using UnityEngine;
using Verse;
using Harmony;
using System.Reflection;
using RimWorld;
=======
﻿using Harmony;
using Verse;
using System.Reflection;
using RimWorld;
using UnityEngine;
using System.Collections.Generic;
using Verse.Sound;
using RimWorld.Planet;
>>>>>>> parent of 22a6550... Cleaned up usings:Source/Hotkeys/Utilities/Global.cs
using System.Linq;

namespace Hotkeys
{
    [StaticConstructorOnStartup]
    static class KeyMods
    {
        public static Dictionary<string, KeyModData> allKeyModifiers;
        public static Dictionary<string, KeyModData> oldKeyModifiers;

        static KeyMods()
        {
            allKeyModifiers = Hotkeys.settings.allKeyModifiers;
        }

        public static void BuildKeyModData()
        {
            if (allKeyModifiers == null)
            {
                allKeyModifiers = new Dictionary<string, KeyModData>();
            }

            List<KeyBindingDef> allKeyDefs = DefDatabase<KeyBindingDef>.AllDefsListForReading;
            foreach (var keyDef in allKeyDefs)
            {
                if (!allKeyModifiers.ContainsKey(keyDef.defName))
                {
                    var newModData = new KeyModData();
                    allKeyModifiers[keyDef.defName] = newModData;
                }
            }
        }

        public static void BuildOverlappingKeys()
        {
            var overlappingKeyMods = new Dictionary<KeyCode, List<KeyCode>>();

            List<KeyBindingDef> allKeyDefs = DefDatabase<KeyBindingDef>.AllDefsListForReading;
            KeyPrefsData keyPrefData = KeyPrefs.KeyPrefsData.Clone();

            List<KeyCode> allTriggerKeys = new List<KeyCode>();
            foreach (KeyBindingDef keyDef in allKeyDefs)
            {
                allTriggerKeys.Add(keyPrefData.GetBoundKeyCode(keyDef, KeyPrefs.BindingSlot.A));
                allTriggerKeys.Add(keyPrefData.GetBoundKeyCode(keyDef, KeyPrefs.BindingSlot.B));
            }

            List<KeyCode> overlappingTriggerKeys = allTriggerKeys.GroupBy(x => x).SelectMany(x => x.Skip(1)).Distinct().ToList();
            overlappingTriggerKeys.Remove(KeyCode.None);

            foreach (KeyBindingDef keyDef in allKeyDefs)
            {
                KeyCode keyCodeA = keyPrefData.GetBoundKeyCode(keyDef, KeyPrefs.BindingSlot.A);
                KeyCode keyCodeB = keyPrefData.GetBoundKeyCode(keyDef, KeyPrefs.BindingSlot.B);

                List<KeyCode> newKeyCodes = new List<KeyCode>();
                List<KeyCode> storedKeyCodes = new List<KeyCode>();

                if (overlappingTriggerKeys.Contains(keyCodeA))
                {
                    newKeyCodes = keyDef.ModifierData().keyBindModsA;
                    overlappingKeyMods.TryGetValue(keyCodeA, out storedKeyCodes);
                    if (newKeyCodes == null) { newKeyCodes = new List<KeyCode>(); }
                    if (storedKeyCodes == null) { storedKeyCodes = new List<KeyCode>(); }

                    List<KeyCode> modifierKeyCodes = storedKeyCodes.Union(newKeyCodes).ToList();
                    if (modifierKeyCodes == null) { modifierKeyCodes = new List<KeyCode>(); }

                    overlappingKeyMods[keyCodeA] = modifierKeyCodes;
                }

                if (overlappingTriggerKeys.Contains(keyCodeB))
                {
                    newKeyCodes = keyDef.ModifierData().keyBindModsB;
                    overlappingKeyMods.TryGetValue(keyCodeB, out storedKeyCodes);
                    if (newKeyCodes == null) { newKeyCodes = new List<KeyCode>(); }
                    if (storedKeyCodes == null) { storedKeyCodes = new List<KeyCode>(); }

                    List<KeyCode> modifierKeyCodes = storedKeyCodes.Union(newKeyCodes).ToList();
                    if (modifierKeyCodes == null) { modifierKeyCodes = new List<KeyCode>(); }

                    overlappingKeyMods[keyCodeB] = modifierKeyCodes;
                }
            }

            //Assign overlaps to keymods
            foreach (var keyDef in allKeyDefs)
            {
                KeyCode keyCodeA = keyPrefData.GetBoundKeyCode(keyDef, KeyPrefs.BindingSlot.A);
                KeyCode keyCodeB = keyPrefData.GetBoundKeyCode(keyDef, KeyPrefs.BindingSlot.B);

                if (overlappingKeyMods.TryGetValue(keyCodeA, out List<KeyCode> overlapsA))
                {
                    keyDef.ModifierData().overlappingKeysA = overlapsA;
                }
                if (overlappingKeyMods.TryGetValue(keyCodeB, out List<KeyCode> overlapsB))
                {
                    keyDef.ModifierData().overlappingKeysB = overlapsB;
                }
            }
        }
    }
}

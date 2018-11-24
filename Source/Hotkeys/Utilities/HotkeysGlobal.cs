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
    public static class HotkeysGlobal
    {
        public static ExposableList<KeyCode> keysPressed;
        public static bool lShiftWasUp;
        public static bool rShiftWasUp;

        public static Dictionary<string, ExposableList<KeyCode>> oldKeyBindModsA;
        public static Dictionary<string, ExposableList<KeyCode>> oldKeyBindModsB;

        public static Dictionary<KeyCode, List<KeyCode>> overlappingKeyMods;



        public static bool AllModifierKeysDown(KeyBindingDef keyDef, bool resultA, bool resultB)
        {
            KeyPrefsData keyPrefData = KeyPrefs.KeyPrefsData.Clone();
            ExposableList<KeyCode> keyCodes = new ExposableList<KeyCode>();
            bool allDownA = true;
            bool allDownB = true;



            if (HotkeysLate.settings.keyBindModsA.TryGetValue(keyDef.defName, out keyCodes))
            {
                foreach (var keyCode in keyCodes)
                {
                    if (!Input.GetKey(keyCode))
                    {
                        allDownA = false;
                    }
                }

                keyPrefData.keyPrefs.TryGetValue(keyDef, out KeyBindingData keyData);
                if (overlappingKeyMods != null && overlappingKeyMods.TryGetValue(keyData.keyBindingA, out List<KeyCode> mods))
                {
                    foreach (KeyCode code in mods)
                    {
                        if (!keyCodes.Contains(code) && Input.GetKey(code))
                        {
                            allDownA = false;
                        }
                    }
                }
            }

            if (HotkeysLate.settings.keyBindModsB.TryGetValue(keyDef.defName, out keyCodes))
            {
                foreach (var keyCode in keyCodes)
                {
                    if (!Input.GetKey(keyCode))
                    {
                        allDownB = false;
                    }
                }

                keyPrefData.keyPrefs.TryGetValue(keyDef, out KeyBindingData keyData);
                if (overlappingKeyMods != null && overlappingKeyMods.TryGetValue(keyData.keyBindingB, out List<KeyCode> mods))
                {
                    foreach (KeyCode code in mods)
                    {
                        if (!keyCodes.Contains(code) && Input.GetKey(code))
                        {
                            allDownB = false;
                        }
                    }
                }
            }

            return (allDownA && resultA) || (allDownB && resultB);
        }



        public static void BuildOverlappingKeys()
        {
            if (overlappingKeyMods == null) { overlappingKeyMods = new Dictionary<KeyCode, List<KeyCode>>(); }
            overlappingKeyMods.Clear();

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
                    HotkeysLate.settings.keyBindModsA.TryGetValue(keyDef.defName, out ExposableList<KeyCode> newCodes);
                    newKeyCodes = newCodes;
                    overlappingKeyMods.TryGetValue(keyCodeA, out storedKeyCodes);
                    if (newKeyCodes == null) { newKeyCodes = new ExposableList<KeyCode>(); }
                    if (storedKeyCodes == null) { storedKeyCodes = new List<KeyCode>(); }

                    List<KeyCode> modifierKeyCodes = storedKeyCodes.Union(newKeyCodes).ToList();
                    if (modifierKeyCodes == null) { modifierKeyCodes = new List<KeyCode>(); }

                    overlappingKeyMods[keyCodeA] = modifierKeyCodes;
                }
            }
        }

    } 
}




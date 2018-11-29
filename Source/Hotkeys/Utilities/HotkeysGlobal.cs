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
    // Static Constructor runs after all other startup processes to initialize mod
    [StaticConstructorOnStartup]
    public class InitializeMod
    {
        static InitializeMod()
        {
            HotkeysGlobal.BuildOverlappingKeys();
            Hotkeys_Save.isInit = true;
        }
    }

    // Global references for some variables/methods
    public static class HotkeysGlobal
    {
        // Intermediate Fields
        public static List<KeyCode> keysPressed;
        public static bool lShiftWasUp;
        public static bool rShiftWasUp;

        public static Dictionary<string, KeyModData> oldKeyModifiers;

        public static void BuildKeyModData()
        {
            var allKeyModifiers = Hotkeys_Save.saved.allKeyModifiers;

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




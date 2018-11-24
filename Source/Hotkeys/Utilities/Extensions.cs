using Harmony;
using Verse;
using System.Reflection;
using RimWorld;
using UnityEngine;
using System.Collections.Generic;
using Verse.Sound;
using RimWorld.Planet;
using System.Linq;

//namespace Hotkeys
//{
//    public static class DictKeyListKeysExtension
//    {
//        public static void Build(this Dictionary<KeyCode, List<KeyCode>> dict)
//        {
//            if (dict == null) { dict = new Dictionary<KeyCode, List<KeyCode>>(); }
//            dict.Clear();

//            Log.Message("Building Overlap List");

//            List<KeyBindingDef> allKeyDefs = DefDatabase<KeyBindingDef>.AllDefsListForReading;
//            KeyPrefsData keyPrefData = KeyPrefs.KeyPrefsData.Clone();

//            List<KeyCode> allTriggerKeys = new List<KeyCode>();
//            foreach (KeyBindingDef keyDef in allKeyDefs)
//            {
//                allTriggerKeys.Add(keyPrefData.GetBoundKeyCode(keyDef, KeyPrefs.BindingSlot.A));
//                allTriggerKeys.Add(keyPrefData.GetBoundKeyCode(keyDef, KeyPrefs.BindingSlot.B));
//            }

//            List<KeyCode> overlappingTriggerKeys = allTriggerKeys.GroupBy(x => x).SelectMany(x => x.Skip(1)).Distinct().ToList();
//            overlappingTriggerKeys.Remove(KeyCode.None);

//            foreach (var code in overlappingTriggerKeys)
//            {
//                Log.Message("triggerKeys:" + code.ToString());
//            }

//            foreach (KeyBindingDef keyDef in allKeyDefs)
//            {
//                KeyCode keyCodeA = keyPrefData.GetBoundKeyCode(keyDef, KeyPrefs.BindingSlot.A);
//                KeyCode keyCodeB = keyPrefData.GetBoundKeyCode(keyDef, KeyPrefs.BindingSlot.B);

//                List<KeyCode> newKeyCodes = new List<KeyCode>();
//                List<KeyCode> storedKeyCodes = new List<KeyCode>();

//                if (overlappingTriggerKeys.Contains(keyCodeA))
//                {
//                    HotkeysLate.settings.keyBindModsA.TryGetValue(keyDef.defName, out ExposableList<KeyCode> newCodes);
//                    newKeyCodes = newCodes;
//                    dict.TryGetValue(keyCodeA, out storedKeyCodes);
//                    if (newKeyCodes == null) { newKeyCodes = new ExposableList<KeyCode>(); }
//                    if (storedKeyCodes == null) { storedKeyCodes = new List<KeyCode>(); }

//                    List<KeyCode> modifierKeyCodes = storedKeyCodes.Union(newKeyCodes).ToList();
//                    if (modifierKeyCodes == null) { modifierKeyCodes = new List<KeyCode>(); }

//                    foreach (var code in modifierKeyCodes)
//                    {
//                        Log.Message(keyCodeA.ToString() + ": " + code.ToString());
//                    }

//                    dict[keyCodeA] = modifierKeyCodes;
//                }
//            }
//        }
//    }
//}

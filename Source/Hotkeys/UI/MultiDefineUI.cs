using Harmony;
using Verse;
using RimWorld;
using UnityEngine;
using System.Collections.Generic;
using System;
using System.Linq;

namespace Hotkeys
{
    // Detour the define binding window
    [HarmonyPatch(typeof(Dialog_DefineBinding))]
    [HarmonyPatch("DoWindowContents")]
    public class KeyBindingWindowPatch
    {
        private static bool AnyShiftPressed()
        {
            if (Input.GetKeyDown(KeyCode.LeftShift)) { return true; }
            if (Input.GetKeyDown(KeyCode.RightShift)) { return true; }
            if (Input.GetKeyUp(KeyCode.LeftShift)) { return true; }
            if (Input.GetKeyUp(KeyCode.RightShift)) { return true; }

            return false;
        }

        private static void BindOnKeyUp(ref KeyPrefsData ___keyPrefsData, ref KeyBindingDef ___keyDef, ref KeyPrefs.BindingSlot ___slot, Dialog_DefineBinding __instance, ExposableList<KeyCode> keysPressed)
        {
            KeyCode lastPressed = keysPressed.Last();
            keysPressed.RemoveLast();
            ___keyPrefsData.SetBinding(___keyDef, ___slot, lastPressed);

            var settings = HotkeysLate.settings;
            if (___slot == KeyPrefs.BindingSlot.A) { settings.keyBindModsA[___keyDef.defName] = new ExposableList<KeyCode>(keysPressed); }
            if (___slot == KeyPrefs.BindingSlot.B) { settings.keyBindModsB[___keyDef.defName] = new ExposableList<KeyCode>(keysPressed); }
            settings.Write();

            ___keyPrefsData.EraseConflictingBindingsForKeyCode(___keyDef, lastPressed, delegate (KeyBindingDef oldDef)
            {
                Messages.Message("KeyBindingOverwritten".Translate(oldDef.LabelCap), MessageTypeDefOf.TaskCompletion, false);
            });

            __instance.Close(true);
            //Event.current.Use();  Dunno what the original purpose of this is for.
        }

        
        static bool Prefix(Rect inRect, ref KeyPrefsData ___keyPrefsData, ref KeyBindingDef ___keyDef, ref KeyPrefs.BindingSlot ___slot, Dialog_DefineBinding __instance)
        {
            if (!Hotkeys.settings.useMultiKeys) { return true; }

            Text.Anchor = TextAnchor.MiddleCenter;
            Widgets.Label(inRect, "PressAnyKeyOrEsc".Translate());
            Text.Anchor = TextAnchor.UpperLeft;

            if ((Event.current.isKey && Event.current.keyCode != KeyCode.None) || (AnyShiftPressed()))
            {
                ExposableList<KeyCode> keysPressed = HotkeysGlobal.keysPressed;

                if (Event.current.type == EventType.KeyUp)
                {
                    BindOnKeyUp(ref ___keyPrefsData, ref ___keyDef, ref ___slot, __instance, keysPressed);
                }
                if (Input.GetKeyUp(KeyCode.LeftShift) && !HotkeysGlobal.lShiftWasUp && keysPressed.Contains(KeyCode.LeftShift))
                {
                    HotkeysGlobal.lShiftWasUp = true;
                    BindOnKeyUp(ref ___keyPrefsData, ref ___keyDef, ref ___slot, __instance, keysPressed);
                }
                if (Input.GetKeyUp(KeyCode.RightShift) && !HotkeysGlobal.rShiftWasUp && keysPressed.Contains(KeyCode.RightShift))
                {
                    HotkeysGlobal.rShiftWasUp = true;
                    BindOnKeyUp(ref ___keyPrefsData, ref ___keyDef, ref ___slot, __instance, keysPressed);
                }

                if (Event.current.type == EventType.KeyDown)
                {
                    if (!keysPressed.Contains(Event.current.keyCode)) { keysPressed.Add(Event.current.keyCode); }
                }
                if (Input.GetKeyDown(KeyCode.LeftShift))
                {
                    if (!keysPressed.Contains(KeyCode.LeftShift)) { keysPressed.Add(KeyCode.LeftShift); }
                }
                if (Input.GetKeyDown(KeyCode.RightShift))
                {
                    if (!keysPressed.Contains(KeyCode.RightShift)) { keysPressed.Add(KeyCode.RightShift); }
                }

                if (Event.current.keyCode == KeyCode.Escape)
                {
                    __instance.Close(true);
                    Event.current.Use();
                }
            }
            return false;
        }
    }

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
            var settings = HotkeysLate.settings;
            if (settings == null) { settings = LoadedModManager.GetMod<HotkeysLate>().GetSettings<HotkeySettingsLate>(); }

            __instance.keyPrefs.TryGetValue(assignedKeyDef, out KeyBindingData prefDataAssigned);

            ExposableList<KeyCode> assignedCodes = new ExposableList<KeyCode>();
            ExposableList<KeyCode> existingCodes = new ExposableList<KeyCode>();

            if (assignedCode == prefDataAssigned.keyBindingA)
            {
                if (settings.keyBindModsA.TryGetValue(assignedKeyDef.defName, out ExposableList<KeyCode> aCodes))
                {
                    assignedCodes.AddRange(aCodes);
                }
            }
            if (assignedCode == prefDataAssigned.keyBindingB)
            {
                if (settings.keyBindModsB.TryGetValue(assignedKeyDef.defName, out ExposableList<KeyCode> aCodes))
                {
                    assignedCodes.AddRange(aCodes);
                }
            }
            if (prefDataExisting.keyBindingA == assignedCode)
            {
                if (settings.keyBindModsA.TryGetValue(existingKeyDef.defName, out ExposableList<KeyCode> eCodes))
                {
                    existingCodes.AddRange(eCodes);
                }
                return assignedCodes.OrderBy(i => i).SequenceEqual(existingCodes.OrderBy(i => i));
            }
            if (prefDataExisting.keyBindingB == assignedCode)
            {
                if (settings.keyBindModsB.TryGetValue(existingKeyDef.defName, out ExposableList<KeyCode> eCodes))
                {
                    existingCodes.AddRange(eCodes);
                }
                return assignedCodes.OrderBy(i => i).SequenceEqual(existingCodes.OrderBy(i => i));
            }
            else
            {
                return false;
            }
        }
    }

    // Detour for something
    [HarmonyPatch(typeof(KeyPrefsData))]
    [HarmonyPatch("ErrorCheckOn")]
    public class Patch_ErrorCheckOn
    {
        static bool Prefix(KeyPrefsData __instance, KeyBindingDef keyDef, KeyPrefs.BindingSlot slot)
        {
            if (!Hotkeys.settings.useMultiKeys) { return true; }

            KeyCode boundKeyCode = __instance.GetBoundKeyCode(keyDef, slot);
            if (boundKeyCode != KeyCode.None)
            {
                foreach (KeyBindingDef keyBindingDef in BindingConflicts.ConflictingBindings(keyDef, boundKeyCode, __instance))
                {
                    bool flag = boundKeyCode != keyDef.GetDefaultKeyCode(slot);
                    Log.Error(string.Concat(new object[]
                    {
                        "Key binding conflict: ",
                        keyBindingDef,
                        " and ",
                        keyDef,
                        " are both bound to ",
                        boundKeyCode,
                        ".",
                        (!flag) ? string.Empty : " Fixed automatically."
                    }), false);
                    if (flag)
                    {
                        if (slot == KeyPrefs.BindingSlot.A)
                        {
                            __instance.keyPrefs[keyDef].keyBindingA = keyDef.defaultKeyCodeA;
                        }
                        else
                        {
                            __instance.keyPrefs[keyDef].keyBindingB = keyDef.defaultKeyCodeB;
                        }
                        KeyPrefs.Save();
                    }
                }
            }

            return false;
        }
    }

    // Detour to change how conflicting keybindings are handled.
    [HarmonyPatch(typeof(KeyPrefsData))]
    [HarmonyPatch("EraseConflictingBindingsForKeyCode")]
    public class Patch_EraseConflictingBindingsForKeyCode
    {
        static bool Prefix(KeyPrefsData __instance, KeyBindingDef keyDef, KeyCode keyCode, Action<KeyBindingDef> callBackOnErase = null)
        {
            if (!Hotkeys.settings.useMultiKeys) { return true; }

            foreach (KeyBindingDef keyBindingDef in BindingConflicts.ConflictingBindings(keyDef, keyCode, __instance))
            {
                KeyBindingData keyBindingData = __instance.keyPrefs[keyBindingDef];
                if (keyBindingData.keyBindingA == keyCode)
                {
                    Log.Message("Reset");
                    keyBindingData.keyBindingA = KeyCode.None;
                    HotkeysPatch_KeyBindDrawing.ResetModifierList(KeyPrefs.BindingSlot.A, keyBindingDef);
                }
                if (keyBindingData.keyBindingB == keyCode)
                {
                    keyBindingData.keyBindingB = KeyCode.None;
                    HotkeysPatch_KeyBindDrawing.ResetModifierList(KeyPrefs.BindingSlot.B, keyBindingDef);
                }
                if (callBackOnErase != null)
                {
                    callBackOnErase(keyBindingDef);
                }
            }

            return false;
        }
    }

}
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
    [HarmonyPatch(typeof(Dialog_DefineBinding), nameof(Dialog_DefineBinding.DoWindowContents))]
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

        private static void BindOnKeyUp(ref KeyPrefsData ___keyPrefsData, ref KeyBindingDef ___keyDef, ref KeyPrefs.BindingSlot ___slot, Dialog_DefineBinding __instance, List<KeyCode> keysPressed)
        {
            KeyCode lastPressed = keysPressed.Last();
            keysPressed.RemoveLast();
            ___keyPrefsData.SetBinding(___keyDef, ___slot, lastPressed);

            var settings = Hotkeys_Save.saved;
            if (___slot == KeyPrefs.BindingSlot.A) { ___keyDef.ModifierData().keyBindModsA = new List<KeyCode>(keysPressed); }
            if (___slot == KeyPrefs.BindingSlot.B) { ___keyDef.ModifierData().keyBindModsB = new List<KeyCode>(keysPressed); }
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
                List<KeyCode> keysPressed = HotkeysGlobal.keysPressed;

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
            var settings = Hotkeys_Save.saved;
            if (settings == null) { settings = LoadedModManager.GetMod<Hotkeys_Save>().GetSettings<Hotkeys_SettingsSave>(); }

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

    // Detour to use my ConflictingBindingsMethod
    [HarmonyPatch(typeof(KeyPrefsData), "ErrorCheckOn")]
    public class Patch_ErrorCheckOn
    {
        static bool Prefix(KeyPrefsData __instance, KeyBindingDef keyDef, KeyPrefs.BindingSlot slot)
        {
            Log.Message("ErrorCheckOn");
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
    [HarmonyPatch(typeof(KeyPrefsData), nameof(KeyPrefsData.EraseConflictingBindingsForKeyCode))]
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
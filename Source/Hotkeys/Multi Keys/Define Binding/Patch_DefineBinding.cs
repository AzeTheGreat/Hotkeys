﻿using HarmonyLib;
using RimWorld;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;

namespace Hotkeys
{
    // Detour the define binding window
    [HarmonyPatch(typeof(Dialog_DefineBinding), nameof(Dialog_DefineBinding.DoWindowContents))]
    public class Patch_DefineBinding
    {
        private static bool AnyShiftPressed()
        {
            if (Input.GetKeyDown(KeyCode.LeftShift)) { return true; }
            if (Input.GetKeyDown(KeyCode.RightShift)) { return true; }
            if (Input.GetKeyUp(KeyCode.LeftShift)) { return true; }
            if (Input.GetKeyUp(KeyCode.RightShift)) { return true; }

            return false;
        }

        private static void BindOnKeyUp(ref KeyPrefsData ___keyPrefsData, ref KeyBindingDef ___keyDef, ref KeyPrefs.BindingSlot ___slot, List<KeyCode> keysPressed)
        {
            KeyCode lastPressed = keysPressed.Last();
            keysPressed.RemoveLast();
            ___keyPrefsData.SetBinding(___keyDef, ___slot, lastPressed);

            var settings = Hotkeys.settings;
            if (___slot == KeyPrefs.BindingSlot.A) { ___keyDef.ModifierData().keyBindModsA = new List<KeyCode>(keysPressed); }
            if (___slot == KeyPrefs.BindingSlot.B) { ___keyDef.ModifierData().keyBindModsB = new List<KeyCode>(keysPressed); }
            settings.Write();

            ___keyPrefsData.EraseConflictingBindingsForKeyCode(___keyDef, lastPressed, (KeyBindingDef oldDef) => Messages.Message("KeyBindingOverwritten".Translate(oldDef.LabelCap), MessageTypeDefOf.TaskCompletion, false));
        }

        static bool Prefix(Rect inRect, ref KeyPrefsData ___keyPrefsData, ref KeyBindingDef ___keyDef, ref KeyPrefs.BindingSlot ___slot, Dialog_DefineBinding __instance)
        {
            if (!Hotkeys.settings.useMultiKeys) { return true; }

            Text.Anchor = TextAnchor.MiddleCenter;
            Widgets.Label(inRect, "PressAnyKeyOrEsc".Translate());
            Text.Anchor = TextAnchor.UpperLeft;

            if ((Event.current.isKey && Event.current.keyCode != KeyCode.None) || (AnyShiftPressed()))
            {
                List<KeyCode> keysPressed = IntermediateKeys.keysPressed;

                if (Event.current.keyCode == KeyCode.Escape)
                    Close();

                if (Event.current.type == EventType.KeyDown && !keysPressed.Contains(Event.current.keyCode))
                    keysPressed.Add(Event.current.keyCode);   

                if (Input.GetKeyDown(KeyCode.LeftShift) && !keysPressed.Contains(KeyCode.LeftShift))
                    keysPressed.Add(KeyCode.LeftShift);
                    
                if (Input.GetKeyDown(KeyCode.RightShift) && !keysPressed.Contains(KeyCode.RightShift))
                    keysPressed.Add(KeyCode.RightShift);

                if (Event.current.type == EventType.KeyUp || Input.GetKeyUp(KeyCode.LeftShift) || Input.GetKeyUp(KeyCode.RightShift))
                {
                    BindOnKeyUp(ref ___keyPrefsData, ref ___keyDef, ref ___slot, keysPressed);
                    Close();
                }
            }

            return false;

            void Close()
            {
                __instance.Close(true);
                Event.current.Use();
            }
        }
    }
}
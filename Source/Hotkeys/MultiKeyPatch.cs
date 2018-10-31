using Harmony;
using Verse;
using RimWorld;
using UnityEngine;
using System.Collections.Generic;
using System;
using System.Linq;

namespace Hotkeys
{
    [HarmonyPatch(typeof(KeyBindingDef))]
    [HarmonyPatch("JustPressed")]
    [HarmonyPatch(MethodType.Getter)]
    public class MultiKeyPatchJustPressed
    {
        // Kinda dirty maybe make separate harmony to patch later?
        static void Postfix(ref bool __result, KeyBindingDef __instance)
        {
            if (!__result || !HotkeysLate.isInit) { return; }

            KeyBindingData keyBindingData;
            if (KeyPrefs.KeyPrefsData.keyPrefs.TryGetValue(__instance, out keyBindingData))
            {
                bool resultA = Input.GetKeyDown(keyBindingData.keyBindingA);
                bool resultB = Input.GetKeyDown(keyBindingData.keyBindingB);
                __result = HotkeysGlobal.AllModifierKeysDown(__instance, resultA, resultB);
            }
        }
    }

    [HarmonyPatch(typeof(KeyBindingDef))]
    [HarmonyPatch("IsDown")]
    [HarmonyPatch(MethodType.Getter)]
    public class MultiKeyPatchIsDown
    {
        static void Postfix(ref bool __result, KeyBindingDef __instance)
        {
            if (!__result || !HotkeysLate.isInit) { return; }

            KeyBindingData keyBindingData;
            if (KeyPrefs.KeyPrefsData.keyPrefs.TryGetValue(__instance, out keyBindingData))
            {
                bool resultA = Input.GetKey(keyBindingData.keyBindingA);
                bool resultB = Input.GetKey(keyBindingData.keyBindingB);
                __result = HotkeysGlobal.AllModifierKeysDown(__instance, resultA, resultB);
            }
        }
    }



    [HarmonyPatch(typeof(Dialog_DefineBinding))]
    [HarmonyPatch("DoWindowContents")]
    public class KeyBindingWindowPatch
    {
        static bool Prefix(Rect inRect, ref KeyPrefsData ___keyPrefsData, ref KeyBindingDef ___keyDef, ref KeyPrefs.BindingSlot ___slot, Dialog_DefineBinding __instance)
        {
            Text.Anchor = TextAnchor.MiddleCenter;
            Widgets.Label(inRect, "PressAnyKeyOrEsc".Translate());
            Text.Anchor = TextAnchor.UpperLeft;

            if (Event.current.isKey && Event.current.keyCode != KeyCode.None)
            {
                var keysPressed = HotkeysGlobal.keysPressed;

                if (Event.current.type == EventType.KeyUp)
                {
                    ___keyPrefsData.SetBinding(___keyDef, ___slot, Event.current.keyCode);
                    keysPressed.Remove(Event.current.keyCode);

                    var settings = HotkeysLate.settings;
                    if (___slot == KeyPrefs.BindingSlot.A) { settings.keyBindModsA[___keyDef] = new ExposableList<KeyCode>(keysPressed); }
                    if (___slot == KeyPrefs.BindingSlot.B) { settings.keyBindModsB[___keyDef] = new ExposableList<KeyCode>(keysPressed); }
                    settings.Write();

                    ___keyPrefsData.EraseConflictingBindingsForKeyCode(___keyDef, Event.current.keyCode, delegate (KeyBindingDef oldDef)
                    {
                        Messages.Message("KeyBindingOverwritten".Translate(oldDef.LabelCap), MessageTypeDefOf.TaskCompletion, false);
                    });

                    __instance.Close(true);
                    Event.current.Use();
                }

                if (Event.current.type == EventType.KeyDown)
                {
                    if (!keysPressed.Contains(Event.current.keyCode)) { keysPressed.Add(Event.current.keyCode); }
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

    [HarmonyPatch(typeof(KeyPrefsData))]
    [HarmonyPatch("ConflictingBindings")]
    public class BindingConflictPatch
    {
        static bool Prefix(KeyBindingDef keyDef, KeyCode code, KeyPrefsData __instance, ref IEnumerable<KeyBindingDef> __result)
        {
            List<KeyBindingDef> allKeyDefs = DefDatabase<KeyBindingDef>.AllDefsListForReading;
            List<KeyBindingDef> conflictingDefs = new List<KeyBindingDef>();

            foreach (var def in allKeyDefs)
            {
                KeyBindingData prefData;
                if (def != keyDef
                    && ((def.category == keyDef.category && def.category.selfConflicting) || keyDef.category.checkForConflicts.Contains(def.category)
                    || (keyDef.extraConflictTags != null && def.extraConflictTags != null && keyDef.extraConflictTags.Any((string tag) => def.extraConflictTags.Contains(tag))))
                    && __instance.keyPrefs.TryGetValue(def, out prefData) && (CheckAllKeys(keyDef, def, prefData, code)))
                {
                    conflictingDefs.Add(def);
                }
            }
            __result = conflictingDefs;

            return false;
        }

        private static bool CheckAllKeys(KeyBindingDef assignedKeyDef, KeyBindingDef existingKeyDef, KeyBindingData prefData, KeyCode assignedCode)
        {
            var settings = HotkeysLate.settings;
            if (settings == null) { settings = LoadedModManager.GetMod<HotkeysLate>().GetSettings<HotkeySettingsLate>(); }

            if (prefData.keyBindingA == assignedCode)
            {
                if (settings.keyBindModsA.TryGetValue(assignedKeyDef, out ExposableList<KeyCode> assignedCodes) &&
                    settings.keyBindModsA.TryGetValue(existingKeyDef, out ExposableList<KeyCode> existingCodes))   
                {
                    return assignedCodes.OrderBy(i => i).SequenceEqual(existingCodes.OrderBy(i => i));
                }
                return false;
            }
            if (prefData.keyBindingB == assignedCode)
            {
                if (settings.keyBindModsB.TryGetValue(assignedKeyDef, out ExposableList<KeyCode> assignedCodes) &&
                    settings.keyBindModsB.TryGetValue(existingKeyDef, out ExposableList<KeyCode> existingCodes))
                {
                    return assignedCodes.OrderBy(i => i).SequenceEqual(existingCodes.OrderBy(i => i));
                }
                return false;
            }
            else
            {
                return false;
            }
        }
    }



    [HarmonyPatch(typeof(Dialog_KeyBindings))]
    [HarmonyPatch("DrawKeyEntry")]
    public class KeyBindDrawingPatch
    {
        static bool Prefix(KeyBindingDef keyDef, Rect parentRect, ref float curY, bool skipDrawing, ref KeyPrefsData ___keyPrefsData)
        {
            if (!skipDrawing)
            {
                Rect rect = new Rect(parentRect.x, parentRect.y + curY, parentRect.width, 34f).ContractedBy(3f);
                GenUI.SetLabelAlign(TextAnchor.MiddleLeft);
                Widgets.Label(rect, keyDef.LabelCap);
                GenUI.ResetLabelAlign();
                float num = 4f;
                Vector2 vector = new Vector2(140f, 28f);
                Rect rect2 = new Rect(rect.x + rect.width - vector.x * 2f - num, rect.y, vector.x, vector.y);
                Rect rect3 = new Rect(rect.x + rect.width - vector.x, rect.y, vector.x, vector.y);
                TooltipHandler.TipRegion(rect2, new TipSignal("BindingButtonToolTip".Translate()));
                TooltipHandler.TipRegion(rect3, new TipSignal("BindingButtonToolTip".Translate()));

                string label1 = GetLabelForKeyDef(___keyPrefsData, keyDef, KeyPrefs.BindingSlot.A);
                string label2 = GetLabelForKeyDef(___keyPrefsData, keyDef, KeyPrefs.BindingSlot.B);

                if (Widgets.ButtonText(rect2, label1, true, false, true))
                {
                    SettingButtonClicked(keyDef, KeyPrefs.BindingSlot.A, ___keyPrefsData);
                }
                if (Widgets.ButtonText(rect3, label2, true, false, true))
                {
                    SettingButtonClicked(keyDef, KeyPrefs.BindingSlot.B, ___keyPrefsData);
                }
            }
            curY += 34f;

            return false;
        }



        private static void SettingButtonClicked(KeyBindingDef keyDef, KeyPrefs.BindingSlot slot, KeyPrefsData keyPrefsData)
        {
            if (Event.current.button == 0)
            {
                if (HotkeysGlobal.keysPressed == null) { HotkeysGlobal.keysPressed = new ExposableList<KeyCode>(); }
                HotkeysGlobal.keysPressed.Clear();
                Find.WindowStack.Add(new Dialog_DefineBinding(keyPrefsData, keyDef, slot));
                Event.current.Use();
            }
            else if (Event.current.button == 1)
            {
                List<FloatMenuOption> list = new List<FloatMenuOption>();
                list.Add(new FloatMenuOption("ResetBinding".Translate(), delegate ()
                {
                    KeyCode keyCode = (slot != KeyPrefs.BindingSlot.A) ? keyDef.defaultKeyCodeB : keyDef.defaultKeyCodeA;
                    keyPrefsData.SetBinding(keyDef, slot, keyCode);
                    ResetModifierList(slot, keyDef);

                }, MenuOptionPriority.Default, null, null, 0f, null, null));
                list.Add(new FloatMenuOption("ClearBinding".Translate(), delegate ()
                {
                    keyPrefsData.SetBinding(keyDef, slot, KeyCode.None);
                    ResetModifierList(slot, keyDef);

                }, MenuOptionPriority.Default, null, null, 0f, null, null));
                Find.WindowStack.Add(new FloatMenu(list));
            }
        }

        private static void ResetModifierList(KeyPrefs.BindingSlot slot, KeyBindingDef keyDef)
        {
            var settings = HotkeysLate.settings;
            if (slot == KeyPrefs.BindingSlot.A)
            {
                settings.keyBindModsA[keyDef] = new ExposableList<KeyCode>();
                settings.Write();
            }
            if (slot == KeyPrefs.BindingSlot.B)
            {
                settings.keyBindModsB[keyDef] = new ExposableList<KeyCode>();
                settings.Write();
            }
        }

        private static string GetLabelForKeyDef(KeyPrefsData keyPrefsData, KeyBindingDef keyDef, KeyPrefs.BindingSlot slot)
        {
            string mainKey = keyPrefsData.GetBoundKeyCode(keyDef, slot).ToStringReadable();
            bool keyPresent = false;
            ExposableList<KeyCode> modifierKeyCodes = new ExposableList<KeyCode>();
            var settings = HotkeysLate.settings;

            if (slot == KeyPrefs.BindingSlot.A)
            {
                keyPresent = settings.keyBindModsA.TryGetValue(keyDef, out modifierKeyCodes);
            }
            if (slot == KeyPrefs.BindingSlot.B)
            {
                keyPresent = settings.keyBindModsB.TryGetValue(keyDef, out modifierKeyCodes);
            }

            if (keyPresent)
            {
                foreach (var keyCode in modifierKeyCodes)
                {
                    mainKey = keyCode.ToStringReadable() + " + " + mainKey;
                }
            }

            return mainKey;
        }
    }
}


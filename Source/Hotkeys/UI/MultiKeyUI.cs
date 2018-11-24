using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Reflection.Emit;
using Verse;
using RimWorld;
using RimWorld.Planet;
using Harmony;
using UnityEngine;

namespace Hotkeys
{
    [HarmonyPatch(typeof(Dialog_KeyBindings))]
    [HarmonyPatch("DrawKeyEntry")]
    public class HotkeysPatch_KeyBindDrawing
    {
        static bool Prefix(KeyBindingDef keyDef, Rect parentRect, ref float curY, bool skipDrawing, ref KeyPrefsData ___keyPrefsData)
        {
            if (!Hotkeys.settings.useMultiKeys) { return true; }

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
                HotkeysGlobal.lShiftWasUp = false;
                HotkeysGlobal.rShiftWasUp = false;
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

        public static void ResetModifierList(KeyPrefs.BindingSlot slot, KeyBindingDef keyDef)
        {
            var settings = HotkeysLate.settings;
            if (slot == KeyPrefs.BindingSlot.A)
            {
                settings.keyBindModsA[keyDef.defName] = new ExposableList<KeyCode>();
                settings.Write();
            }
            if (slot == KeyPrefs.BindingSlot.B)
            {
                settings.keyBindModsB[keyDef.defName] = new ExposableList<KeyCode>();
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
                keyPresent = settings.keyBindModsA.TryGetValue(keyDef.defName, out modifierKeyCodes);
            }
            if (slot == KeyPrefs.BindingSlot.B)
            {
                keyPresent = settings.keyBindModsB.TryGetValue(keyDef.defName, out modifierKeyCodes);
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

    // Postfix to clear modifiers when resest button is pressed
    [HarmonyPatch(typeof(KeyPrefsData), nameof(KeyPrefsData.ResetToDefaults))]
    public class HotkeysPatch_ResetModifiers
    {
        static void Postfix()
        {
            HotkeysLate.settings.keyBindModsA.Clear();
            HotkeysLate.settings.keyBindModsB.Clear();
        }
    }

    // Transpiler to reset modifiers if closed
    [HarmonyPatch(typeof(Dialog_KeyBindings), nameof(Dialog_KeyBindings.DoWindowContents))]
    public class HotkeysPatch_KeybindingSettingsClosed
    {
        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            MethodInfo Close = AccessTools.Method(typeof(Window), nameof(Window.Close));
            bool afterTarget = false;

            foreach (CodeInstruction i in instructions)
            {
                if (i.opcode == OpCodes.Ldstr && (string)i.operand == "CancelButton")
                {
                    afterTarget = true;
                }
                if (i.opcode == OpCodes.Callvirt && i.operand == Close && afterTarget)
                {
                    i.operand = AccessTools.Method(typeof(HotkeysPatch_KeybindingSettingsClosed), nameof(HotkeysPatch_KeybindingSettingsClosed.Replacement));
                    afterTarget = false;
                }
                yield return i;
            }
        }

        private static void Replacement(Window that, bool toClose)
        {
            that.Close(toClose);
            HotkeysLate.settings.keyBindModsA = new Dictionary<string, ExposableList<KeyCode>>(HotkeysGlobal.oldKeyBindModsA);
            HotkeysLate.settings.keyBindModsB = new Dictionary<string, ExposableList<KeyCode>>(HotkeysGlobal.oldKeyBindModsB);
            HotkeysLate.settings.Write();
        }
    }

    //  Transpiler to clear old modifier lists when keybinding settings window is opened.
    [HarmonyPatch(typeof(Dialog_Options), nameof(Dialog_Options.DoWindowContents))]
    public class HotkeysPatch_KeybindingSettingsOpened
    {
         static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            MethodInfo Add = AccessTools.Method(typeof(WindowStack), nameof(WindowStack.Add));
            bool afterTarget = false;

            foreach (CodeInstruction i in instructions)
            {
                if (i.opcode == OpCodes.Ldstr && (string)i.operand == "KeyboardConfig")
                {
                    afterTarget = true;
                }
                if (i.opcode == OpCodes.Callvirt && i.operand == Add && afterTarget)
                {
                    i.operand = AccessTools.Method(typeof(HotkeysPatch_KeybindingSettingsOpened), nameof(HotkeysPatch_KeybindingSettingsOpened.Replacement));
                }
                yield return i;
            }
        }

        private static void Replacement(WindowStack windowStack, Window window)
        {
            windowStack.Add(window);
            HotkeysGlobal.oldKeyBindModsA = new Dictionary<string, ExposableList<KeyCode>>(HotkeysLate.settings.keyBindModsA);
            HotkeysGlobal.oldKeyBindModsB = new Dictionary<string, ExposableList<KeyCode>>(HotkeysLate.settings.keyBindModsB);
        }
    }



    //[HarmonyPatch(typeof(Dialog_KeyBindings))]
    //[HarmonyPatch(nameof(Dialog_KeyBindings.DoWindowContents))]
    //public class HotkeysPatch_DoWindowContents
    //{
    //    static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
    //    {
    //        MethodInfo ResetToDefaults = AccessTools.Method(typeof(KeyPrefsData), nameof(KeyPrefsData.ResetToDefaults));

    //        foreach (CodeInstruction i in instructions)
    //        {
    //            if (i.opcode == OpCodes.Callvirt && i.operand == ResetToDefaults)
    //            {
    //                i.operand = AccessTools.Method(typeof(HotkeysPatch_DoWindowContents), nameof(HotkeysPatch_DoWindowContents.Testing));
    //            }

    //            yield return i;
    //        }
    //    }

    //    public static void Testing(KeyPrefsData that)
    //    {
    //        that.ResetToDefaults();
    //        Log.Message("Transpiled");
    //    }
    //}
}


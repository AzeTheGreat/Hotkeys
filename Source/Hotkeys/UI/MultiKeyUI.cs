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
    // Detour to take over keybinding screen
    [HarmonyPatch(typeof(Dialog_KeyBindings), "DrawKeyEntry")]
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
                if (HotkeysGlobal.keysPressed == null) { HotkeysGlobal.keysPressed = new List<KeyCode>(); }
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
            var settings = Hotkeys_Save.saved;
            if (slot == KeyPrefs.BindingSlot.A)
            {
                keyDef.ModifierData().keyBindModsA = new List<KeyCode>();
                settings.Write();
            }
            if (slot == KeyPrefs.BindingSlot.B)
            {
                keyDef.ModifierData().keyBindModsB = new List<KeyCode>();
                settings.Write();
            }
        }

        private static string GetLabelForKeyDef(KeyPrefsData keyPrefsData, KeyBindingDef keyDef, KeyPrefs.BindingSlot slot)
        {
            string mainKey = keyPrefsData.GetBoundKeyCode(keyDef, slot).ToStringReadable();
            List<KeyCode> modifierKeyCodes = new List<KeyCode>();
            var settings = Hotkeys_Save.saved;

            if (slot == KeyPrefs.BindingSlot.A)
            {
                modifierKeyCodes.AddRange(keyDef.ModifierData().keyBindModsA);
            }
            if (slot == KeyPrefs.BindingSlot.B)
            {
                modifierKeyCodes.AddRange(keyDef.ModifierData().keyBindModsB);
            }

            foreach (var keyCode in modifierKeyCodes)
            {
                mainKey = keyCode.ToStringReadable() + " + " + mainKey;
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
            Hotkeys_Save.saved.allKeyModifiers.Clear();
            HotkeysGlobal.BuildKeyModData();
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
                    yield return i;
                    yield return new CodeInstruction(OpCodes.Call,
                        AccessTools.Method(typeof(HotkeysPatch_KeybindingSettingsClosed), nameof(HotkeysPatch_KeybindingSettingsClosed.RestoreKeyBindings)));
                    afterTarget = false;
                    continue;
                }
                yield return i;
            }
        }

        private static void RestoreKeyBindings()
        {
            if (!Hotkeys.settings.useMultiKeys) { return; }

            Hotkeys_Save.saved.allKeyModifiers = new Dictionary<string, KeyModData>(HotkeysGlobal.oldKeyModifiers);
        }
    }

    // Transpiler to clear old modifier lists when keybinding settings window is opened.
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
                    yield return i;
                    yield return new CodeInstruction(OpCodes.Call,
                        AccessTools.Method(typeof(HotkeysPatch_KeybindingSettingsOpened), nameof(HotkeysPatch_KeybindingSettingsOpened.BackupKeyBindings)));
                    afterTarget = false;
                    continue;
                }
                yield return i;
            }
        }

        private static void BackupKeyBindings()
        {
            if (!Hotkeys.settings.useMultiKeys) { return; }

            HotkeysGlobal.oldKeyModifiers = new Dictionary<string, KeyModData>(Hotkeys_Save.saved.allKeyModifiers);
        }
    }

    // Transpiler to rebuild overlap dictionary when accepted
    [HarmonyPatch(typeof(Dialog_KeyBindings), nameof(Dialog_KeyBindings.DoWindowContents))]
    public class HotkeysPatch_KeybindingSettingsAccepted
    {
        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            MethodInfo Close = AccessTools.Method(typeof(Window), nameof(Window.Close));
            bool afterTarget = false;

            foreach (CodeInstruction i in instructions)
            {
                if (i.opcode == OpCodes.Ldstr && (string)i.operand == "OK")
                {
                    afterTarget = true;
                }
                if (i.opcode == OpCodes.Callvirt && i.operand == Close && afterTarget)
                {
                    yield return i;
                    yield return new CodeInstruction(OpCodes.Call,
                        AccessTools.Method(typeof(HotkeysPatch_KeybindingSettingsAccepted), nameof(HotkeysPatch_KeybindingSettingsAccepted.RebuildOverlapDict)));
                    afterTarget = false;
                    continue;
                }
                yield return i;
            }
        }

        private static void RebuildOverlapDict()
        {
            if (!Hotkeys.settings.useMultiKeys) { return; }

            HotkeysGlobal.BuildOverlappingKeys();
        }
    }

}


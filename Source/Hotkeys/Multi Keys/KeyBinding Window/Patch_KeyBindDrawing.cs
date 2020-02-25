using HarmonyLib;
using RimWorld;
using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace Hotkeys
{
    // Detour to take over keybinding screen
    [HarmonyPatch(typeof(Dialog_KeyBindings), "DrawKeyEntry")]
    public class Patch_KeyBindDrawing
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
                if (IntermediateKeys.keysPressed == null) { IntermediateKeys.keysPressed = new List<KeyCode>(); }
                IntermediateKeys.keysPressed.Clear();
                Find.WindowStack.Add(new Dialog_DefineBinding(keyPrefsData, keyDef, slot));
                Event.current.Use();
            }
            else if (Event.current.button == 1)
            {
                var list = new List<FloatMenuOption>();
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
            var settings = Hotkeys.settings;
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
            var settings = Hotkeys.settings;

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
}


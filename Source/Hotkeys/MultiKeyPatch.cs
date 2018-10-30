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
        static bool Postfix(bool __result, KeyBindingDef __instance)
        {
            KeyBindingData keyBindingData;
            __result = KeyPrefs.KeyPrefsData.keyPrefs.TryGetValue(__instance, out keyBindingData) && (Input.GetKeyDown(keyBindingData.keyBindingA) && Input.GetKeyDown(keyBindingData.keyBindingB));
            return __result;
        }
    }

    [HarmonyPatch(typeof(KeyBindingDef))]
    [HarmonyPatch("IsDown")]
    [HarmonyPatch(MethodType.Getter)]
    public class MultiKeyPatchIsDown
    {
        static bool Postfix(bool __result, KeyBindingDef __instance)
        {
            KeyBindingData keyBindingData;
            __result = KeyPrefs.KeyPrefsData.keyPrefs.TryGetValue(__instance, out keyBindingData) && (Input.GetKey(keyBindingData.keyBindingA) && Input.GetKey(keyBindingData.keyBindingB));
            return __result;
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
                    settings.keyBindMods[___keyDef] = new ExposableList<KeyCode>(keysPressed);
                    settings.Write();

                    __instance.Close(true);
                    Event.current.Use();
                }

                if (Event.current.type == EventType.KeyDown)
                {
                    if (!keysPressed.Contains(Event.current.keyCode))
                    {
                        keysPressed.Add(Event.current.keyCode);
                    }
                }
                
                if( Event.current.keyCode == KeyCode.Escape)
                {
                    __instance.Close(true);
                    Event.current.Use();
                }
            }
            return false;
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
                }, MenuOptionPriority.Default, null, null, 0f, null, null));
                list.Add(new FloatMenuOption("ClearBinding".Translate(), delegate ()
                {
                    keyPrefsData.SetBinding(keyDef, slot, KeyCode.None);
                }, MenuOptionPriority.Default, null, null, 0f, null, null));
                Find.WindowStack.Add(new FloatMenu(list));
            }
        }

        private static string GetLabelForKeyDef(KeyPrefsData keyPrefsData, KeyBindingDef keyDef, KeyPrefs.BindingSlot slot)
        {
            string mainKey = keyPrefsData.GetBoundKeyCode(keyDef, slot).ToStringReadable();

            if (HotkeysLate.settings.keyBindMods.TryGetValue(keyDef, out ExposableList<KeyCode> modifierKeyCodes))
            {
                foreach (var keyCode in modifierKeyCodes)
                {
                    mainKey = mainKey + " + " + keyCode.ToStringReadable();
                }
            }
            return mainKey;
        }
    }
}


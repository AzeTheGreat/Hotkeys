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
            if (HotkeysGlobal.keysPressed == null) { HotkeysGlobal.keysPressed = new ExposableList<KeyCode>(); }

            Text.Anchor = TextAnchor.MiddleCenter;
            Widgets.Label(inRect, "PressAnyKeyOrEsc".Translate());
            Text.Anchor = TextAnchor.UpperLeft;

            if (Event.current.isKey && Event.current.keyCode != KeyCode.None)
            {
                if (Event.current.type == EventType.KeyUp)
                {
                    ___keyPrefsData.SetBinding(___keyDef, ___slot, Event.current.keyCode);

                    HotkeysGlobal.keysPressed.Remove(Event.current.keyCode);

                    var settings = HotkeysLate.settings;
                    settings.keyBindMods[___keyDef] = HotkeysGlobal.keysPressed;
                    settings.Write();

                    HotkeysGlobal.keysPressed.Clear();
                    __instance.Close(true);
                    Event.current.Use();
                }

                if (Event.current.type == EventType.KeyDown)
                {
                    HotkeysGlobal.keysPressed.Add(Event.current.keyCode);
                }
                
                if( Event.current.keyCode == KeyCode.Escape)
                {
                    HotkeysGlobal.keysPressed.Clear();
                    __instance.Close(true);
                    Event.current.Use();
                }
            }

            return false;
        }


    }
}


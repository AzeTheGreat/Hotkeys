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
    [HarmonyPatch("KeyDownEvent")]
    [HarmonyPatch(MethodType.Getter)]
    public class MultiKeyPatchKeyDownEvent
    {
        static void Postfix(ref bool __result, KeyBindingDef __instance)
        {
            if (!__result || !HotkeysLate.isInit || !Hotkeys.settings.useMultiKeys) { return; }

            KeyBindingData keyBindingData;
            if (KeyPrefs.KeyPrefsData.keyPrefs.TryGetValue(__instance, out keyBindingData))
            {
                bool resultA = Input.GetKeyDown(keyBindingData.keyBindingA) || Event.current.keyCode == keyBindingData.keyBindingA;
                bool resultB = Input.GetKeyDown(keyBindingData.keyBindingB) || Event.current.keyCode == keyBindingData.keyBindingB;
                __result = HotkeysGlobal.AllModifierKeysDown(__instance, resultA, resultB);
            }
        }
    }

    [HarmonyPatch(typeof(KeyBindingDef))]
    [HarmonyPatch("IsDownEvent")]
    [HarmonyPatch(MethodType.Getter)]
    public class MultiKeyPatchIsDownEvent
    {
        static void Postfix(ref bool __result, KeyBindingDef __instance)
        {
            if (!__result || !HotkeysLate.isInit || !Hotkeys.settings.useMultiKeys) { return; }

            KeyBindingData keyBindingData;
            if (KeyPrefs.KeyPrefsData.keyPrefs.TryGetValue(__instance, out keyBindingData))
            {
                bool resultA = Input.GetKey(keyBindingData.keyBindingA);
                bool resultB = Input.GetKey(keyBindingData.keyBindingB);
                __result = HotkeysGlobal.AllModifierKeysDown(__instance, resultA, resultB);
            }
        }
    }

    [HarmonyPatch(typeof(KeyBindingDef))]
    [HarmonyPatch("JustPressed")]
    [HarmonyPatch(MethodType.Getter)]
    public class MultiKeyPatchJustPressed
    {
        // Kinda dirty maybe make separate harmony to patch later?
        static void Postfix(ref bool __result, KeyBindingDef __instance)
        {
            if (!__result || !HotkeysLate.isInit || !Hotkeys.settings.useMultiKeys) { return; }

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
            if (!__result || !HotkeysLate.isInit || !Hotkeys.settings.useMultiKeys) { return; }

            KeyBindingData keyBindingData;
            if (KeyPrefs.KeyPrefsData.keyPrefs.TryGetValue(__instance, out keyBindingData))
            {
                bool resultA = Input.GetKey(keyBindingData.keyBindingA);
                bool resultB = Input.GetKey(keyBindingData.keyBindingB);
                __result = HotkeysGlobal.AllModifierKeysDown(__instance, resultA, resultB);
            }
        }
    }
}


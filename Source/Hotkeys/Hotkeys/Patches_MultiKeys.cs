﻿using Harmony;
using UnityEngine;
using Verse;

namespace Hotkeys
{
    [HarmonyPatch(typeof(KeyBindingDef), nameof(KeyBindingDef.KeyDownEvent), MethodType.Getter)]
    public class Patch_KeyDownEvent
    {
        static void Postfix(ref bool __result, KeyBindingDef __instance)
        {
            if (!__result || !Hotkeys_Save.isInit || !Hotkeys.settings.useMultiKeys) { return; }

            KeyBindingData keyBindingData;
            if (KeyPrefs.KeyPrefsData.keyPrefs.TryGetValue(__instance, out keyBindingData))
            {
                bool resultA = Input.GetKeyDown(keyBindingData.keyBindingA) || Event.current.keyCode == keyBindingData.keyBindingA;
                bool resultB = Input.GetKeyDown(keyBindingData.keyBindingB) || Event.current.keyCode == keyBindingData.keyBindingB;
                __result = __instance.ModifierData().AllModifierKeysDown(__instance, resultA, resultB);
            }
        }
    }

    [HarmonyPatch(typeof(KeyBindingDef), nameof(KeyBindingDef.IsDownEvent), MethodType.Getter)]
    public class Patch_IsDownEvent
    {
        static void Postfix(ref bool __result, KeyBindingDef __instance)
        {
            if (!__result || !Hotkeys_Save.isInit || !Hotkeys.settings.useMultiKeys) { return; }

            KeyBindingData keyBindingData;
            if (KeyPrefs.KeyPrefsData.keyPrefs.TryGetValue(__instance, out keyBindingData))
            {
                bool resultA = Input.GetKey(keyBindingData.keyBindingA);
                bool resultB = Input.GetKey(keyBindingData.keyBindingB);
                __result = __instance.ModifierData().AllModifierKeysDown(__instance, resultA, resultB);
            }
        }
    }

    [HarmonyPatch(typeof(KeyBindingDef), nameof(KeyBindingDef.JustPressed), MethodType.Getter)]
    public class Patch_JustPressed
    {
        // Kinda dirty maybe make separate harmony to patch later?
        static void Postfix(ref bool __result, KeyBindingDef __instance)
        {
            if (!__result || !Hotkeys_Save.isInit || !Hotkeys.settings.useMultiKeys) { return; }

            KeyBindingData keyBindingData;
            if (KeyPrefs.KeyPrefsData.keyPrefs.TryGetValue(__instance, out keyBindingData))
            {
                bool resultA = Input.GetKeyDown(keyBindingData.keyBindingA);
                bool resultB = Input.GetKeyDown(keyBindingData.keyBindingB);
                __result = __instance.ModifierData().AllModifierKeysDown(__instance, resultA, resultB);
            }
        }
    }

    [HarmonyPatch(typeof(KeyBindingDef), nameof(KeyBindingDef.IsDown), MethodType.Getter)]
    public class Patch_IsDown
    {
        static void Postfix(ref bool __result, KeyBindingDef __instance)
        {
            if (!__result || !Hotkeys_Save.isInit || !Hotkeys.settings.useMultiKeys) { return; }

            KeyBindingData keyBindingData;
            if (KeyPrefs.KeyPrefsData.keyPrefs.TryGetValue(__instance, out keyBindingData))
            {
                bool resultA = Input.GetKey(keyBindingData.keyBindingA);
                bool resultB = Input.GetKey(keyBindingData.keyBindingB);
                __result = __instance.ModifierData().AllModifierKeysDown(__instance, resultA, resultB);
            }
        }
    }
}

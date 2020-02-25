using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using Verse;

namespace Hotkeys
{
    class Patches_MultiKeys
    {
        [HarmonyPatch]
        class Patch_KeyDown
        {
            static IEnumerable<MethodBase> TargetMethods()
            {
                yield return AccessTools.PropertyGetter(typeof(KeyBindingDef), nameof(KeyBindingDef.IsDownEvent));
                yield return AccessTools.PropertyGetter(typeof(KeyBindingDef), nameof(KeyBindingDef.IsDown));
            }

            static void Postfix(ref bool __result, KeyBindingDef __instance) => Patch(ref __result, __instance, Input.GetKey);
        }

        [HarmonyPatch]
        class Patch_KeyPressed
        {
            static IEnumerable<MethodBase> TargetMethods()
            {
                yield return AccessTools.PropertyGetter(typeof(KeyBindingDef), nameof(KeyBindingDef.JustPressed));
                yield return AccessTools.PropertyGetter(typeof(KeyBindingDef), nameof(KeyBindingDef.KeyDownEvent));
            }

            static void Postfix(ref bool __result, KeyBindingDef __instance) => Patch(ref __result, __instance, Input.GetKeyDown);
        }

        private static void Patch(ref bool result, KeyBindingDef instance, Func<KeyCode, bool> processKey)
        {
            // Kinda dirty maybe make separate Harmony to patch later?
            if (!result || !Hotkeys.isInit || !Hotkeys.settings.useMultiKeys)
                return;

            if (KeyPrefs.KeyPrefsData.keyPrefs.TryGetValue(instance, out KeyBindingData keyBindingData))
            {
                bool resultA = processKey(keyBindingData.keyBindingA);
                bool resultB = processKey(keyBindingData.keyBindingB);
                result = instance.ModifierData().AllModifierKeysDown(instance, resultA, resultB);
            }
        }
    }
}


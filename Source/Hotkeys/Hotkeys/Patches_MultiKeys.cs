using Harmony;
using Verse;
using RimWorld;
using UnityEngine;
using System.Collections.Generic;
using System;
using System.Linq;

namespace Hotkeys
{
<<<<<<< HEAD:Source/Hotkeys/Hotkeys/Patches_MultiKeys.cs
    [HarmonyPatch(typeof(KeyBindingDef), nameof(KeyBindingDef.KeyDownEvent), MethodType.Getter)]
    public class Patch_KeyDownEvent
=======
    [HarmonyPatch(typeof(KeyBindingDef), nameof(KeyBindingDef.KeyDownEvent))]
    [HarmonyPatch(MethodType.Getter)]
    public class MultiKeyPatchKeyDownEvent
>>>>>>> parent of 852b8cd... Reduced strings in harmony patches:Source/Hotkeys/Hotkeys/MultiKeyPatch.cs
    {
        static void Postfix(ref bool __result, KeyBindingDef __instance)
        {
            if (!__result || !Hotkeys.isInit || !Hotkeys.settings.useMultiKeys) { return; }

            KeyBindingData keyBindingData;
            if (KeyPrefs.KeyPrefsData.keyPrefs.TryGetValue(__instance, out keyBindingData))
            {
                bool resultA = Input.GetKeyDown(keyBindingData.keyBindingA) || Event.current.keyCode == keyBindingData.keyBindingA;
                bool resultB = Input.GetKeyDown(keyBindingData.keyBindingB) || Event.current.keyCode == keyBindingData.keyBindingB;
                __result = __instance.ModifierData().AllModifierKeysDown(__instance, resultA, resultB);
            }
        }
    }

<<<<<<< HEAD:Source/Hotkeys/Hotkeys/Patches_MultiKeys.cs
    [HarmonyPatch(typeof(KeyBindingDef), nameof(KeyBindingDef.IsDownEvent), MethodType.Getter)]
    public class Patch_IsDownEvent
=======
    [HarmonyPatch(typeof(KeyBindingDef), nameof(KeyBindingDef.IsDownEvent))]
    [HarmonyPatch(MethodType.Getter)]
    public class MultiKeyPatchIsDownEvent
>>>>>>> parent of 852b8cd... Reduced strings in harmony patches:Source/Hotkeys/Hotkeys/MultiKeyPatch.cs
    {
        static void Postfix(ref bool __result, KeyBindingDef __instance)
        {
            if (!__result || !Hotkeys.isInit || !Hotkeys.settings.useMultiKeys) { return; }

            KeyBindingData keyBindingData;
            if (KeyPrefs.KeyPrefsData.keyPrefs.TryGetValue(__instance, out keyBindingData))
            {
                bool resultA = Input.GetKey(keyBindingData.keyBindingA);
                bool resultB = Input.GetKey(keyBindingData.keyBindingB);
                __result = __instance.ModifierData().AllModifierKeysDown(__instance, resultA, resultB);
            }
        }
    }

<<<<<<< HEAD:Source/Hotkeys/Hotkeys/Patches_MultiKeys.cs
    [HarmonyPatch(typeof(KeyBindingDef), nameof(KeyBindingDef.JustPressed), MethodType.Getter)]
    public class Patch_JustPressed
=======
    [HarmonyPatch(typeof(KeyBindingDef), nameof(KeyBindingDef.JustPressed))]
    [HarmonyPatch(MethodType.Getter)]
    public class MultiKeyPatchJustPressed
>>>>>>> parent of 852b8cd... Reduced strings in harmony patches:Source/Hotkeys/Hotkeys/MultiKeyPatch.cs
    {
        // Kinda dirty maybe make separate harmony to patch later?
        static void Postfix(ref bool __result, KeyBindingDef __instance)
        {
            if (!__result || !Hotkeys.isInit || !Hotkeys.settings.useMultiKeys) { return; }

            KeyBindingData keyBindingData;
            if (KeyPrefs.KeyPrefsData.keyPrefs.TryGetValue(__instance, out keyBindingData))
            {
                bool resultA = Input.GetKeyDown(keyBindingData.keyBindingA);
                bool resultB = Input.GetKeyDown(keyBindingData.keyBindingB);
                __result = __instance.ModifierData().AllModifierKeysDown(__instance, resultA, resultB);
            }
        }
    }

<<<<<<< HEAD:Source/Hotkeys/Hotkeys/Patches_MultiKeys.cs
    [HarmonyPatch(typeof(KeyBindingDef), nameof(KeyBindingDef.IsDown), MethodType.Getter)]
    public class Patch_IsDown
=======
    [HarmonyPatch(typeof(KeyBindingDef), nameof(KeyBindingDef.IsDown))]
    [HarmonyPatch(MethodType.Getter)]
    public class MultiKeyPatchIsDown
>>>>>>> parent of 852b8cd... Reduced strings in harmony patches:Source/Hotkeys/Hotkeys/MultiKeyPatch.cs
    {
        static void Postfix(ref bool __result, KeyBindingDef __instance)
        {
            if (!__result || !Hotkeys.isInit || !Hotkeys.settings.useMultiKeys) { return; }

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


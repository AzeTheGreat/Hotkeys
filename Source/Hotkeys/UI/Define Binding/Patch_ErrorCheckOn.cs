using Harmony;
using UnityEngine;
using Verse;

namespace Hotkeys
{
    // Detour to use my ConflictingBindingsMethod
    [HarmonyPatch(typeof(KeyPrefsData), "ErrorCheckOn")]
    public class Patch_ErrorCheckOn
    {
        static bool Prefix(KeyPrefsData __instance, KeyBindingDef keyDef, KeyPrefs.BindingSlot slot)
        {
            if (!Hotkeys.settings.useMultiKeys) { return true; }

            KeyCode boundKeyCode = __instance.GetBoundKeyCode(keyDef, slot);
            if (boundKeyCode != KeyCode.None)
            {
                foreach (KeyBindingDef keyBindingDef in BindingConflicts.ConflictingBindings(keyDef, boundKeyCode, __instance))
                {
                    bool flag = boundKeyCode != keyDef.GetDefaultKeyCode(slot);
                    Log.Error(string.Concat(new object[]
                    {
                        "Key binding conflict: ",
                        keyBindingDef,
                        " and ",
                        keyDef,
                        " are both bound to ",
                        boundKeyCode,
                        ".",
                        (!flag) ? string.Empty : " Fixed automatically."
                    }), false);
                    if (flag)
                    {
                        if (slot == KeyPrefs.BindingSlot.A)
                        {
                            __instance.keyPrefs[keyDef].keyBindingA = keyDef.defaultKeyCodeA;
                        }
                        else
                        {
                            __instance.keyPrefs[keyDef].keyBindingB = keyDef.defaultKeyCodeB;
                        }
                        KeyPrefs.Save();
                    }
                }
            }

            return false;
        }
    }
}
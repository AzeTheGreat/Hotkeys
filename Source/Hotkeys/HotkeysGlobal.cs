using Harmony;
using Verse;
using System.Reflection;
using RimWorld;
using UnityEngine;
using System.Collections.Generic;
using Verse.Sound;
using RimWorld.Planet;
using System.Linq;

namespace Hotkeys
{
    public static class HotkeysGlobal
    {
        public static ExposableList<KeyCode> keysPressed;

        public static bool AllModifierKeysDown(KeyBindingDef keyDef)
        {
            ExposableList<KeyCode> keyCodes = new ExposableList<KeyCode>();
            bool allDown = true;

            if (HotkeysLate.settings.keyBindMods.TryGetValue(keyDef, out keyCodes))
            {
                foreach (var keyCode in keyCodes)
                {
                    if (!Input.GetKey(keyCode))
                    {
                        allDown = false;
                    }
                }
            }
            return allDown;
        }
    } 
}


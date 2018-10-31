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



        public static bool AllModifierKeysDown(KeyBindingDef keyDef, bool resultA, bool resultB)
        {

            ExposableList<KeyCode> keyCodes = new ExposableList<KeyCode>();
            bool allDownA = true;
            bool allDownB = true;

            if (HotkeysLate.settings.keyBindModsA.TryGetValue(keyDef.defName, out keyCodes))
            {
                foreach (var keyCode in keyCodes)
                {
                    if (!Input.GetKey(keyCode))
                    {
                        allDownA = false;
                    }
                }
            }

            if (HotkeysLate.settings.keyBindModsB.TryGetValue(keyDef.defName, out keyCodes))
            {
                foreach (var keyCode in keyCodes)
                {
                    if (!Input.GetKey(keyCode))
                    {
                        allDownB = false;
                    }
                }
            }

            return (allDownA && resultA) || (allDownB && resultB);
        }
    } 
}


using Harmony;
using Verse;
using RimWorld;
using UnityEngine;
using System.Collections.Generic;
using System;
using System.Linq;

namespace Hotkeys
{
    static class Extensions
    {
        public static KeyModData ModifierData(this KeyBindingDef keyDef)
        {
            Hotkeys_Save.saved.allKeyModifiers.TryGetValue(keyDef.defName, out KeyModData keyModData);

            //if (keyModData == null)
            //{
            //    keyModData = new KeyModData();
            //    HotkeysGlobal.allKeyModifiers[keyDef.defName] = keyModData;
            //    return keyModData;
            //}
            return keyModData;
        }
    }
}

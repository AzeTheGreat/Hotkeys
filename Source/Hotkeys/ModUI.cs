using Harmony;
using Verse;
using System.Reflection;
using RimWorld;
using UnityEngine;
using System.Collections.Generic;
using Verse.Sound;
using RimWorld.Planet;

namespace Hotkeys
{
    public class Hotkeys : Mod
    {
        public Hotkeys(ModContentPack content) : base(content)
        {
            var harmonyHotkeys = HarmonyInstance.Create("Hotkeys");
            harmonyHotkeys.PatchAll(Assembly.GetExecutingAssembly());
        }
    }
}



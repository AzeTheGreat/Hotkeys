﻿using Verse;

namespace Hotkeys
{
    // Static Constructor runs after all other startup processes to initialize mod
    [StaticConstructorOnStartup]
    public class InitializeMod
    {
        static InitializeMod()
        {
            Global.BuildOverlappingKeys();
            Hotkeys_Save.isInit = true;
        }
    }
}




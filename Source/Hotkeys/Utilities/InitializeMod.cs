using Verse;

namespace Hotkeys
{
    // Static Constructor runs after all other startup processes to initialize mod
    [StaticConstructorOnStartup]
    public class InitializeMod
    {
        static InitializeMod()
        {
            KeyMods.BuildOverlappingKeys();
            Hotkeys.isInit = true;
        }
    }
}




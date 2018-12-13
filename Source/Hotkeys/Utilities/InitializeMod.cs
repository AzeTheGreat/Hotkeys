using Verse;
using System.Collections.Generic;

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

        public static void RemoveKeyDefs(List<KeyBindingDef> ToRemove)
        {
            List<KeyBindingDef> allKeyDefs = DefDatabase<KeyBindingDef>.AllDefsListForReading;

            foreach (var keyDef in ToRemove)
            {
                allKeyDefs.RemoveAll(x => x == keyDef);
            }

            List<KeyBindingDef> allOldDefs = allKeyDefs.ListFullCopy();

            DefDatabase<KeyBindingDef>.Clear();

            foreach (var keyDef in allOldDefs)
            {
                DefDatabase<KeyBindingDef>.Add(keyDef);
            }
        }
    }
}




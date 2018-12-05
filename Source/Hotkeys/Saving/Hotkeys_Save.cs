using Harmony;
using Verse;
using System.Reflection;

namespace Hotkeys
{
    public class Hotkeys_Save : Mod
    {
        public static Hotkeys_SettingsSave saved;
        public static bool isInit = false;

        public Hotkeys_Save(ModContentPack content) : base(content)
        {
            // HARMONY
            var harmonyHotkeys = HarmonyInstance.Create("Hotkeys");
            HarmonyInstance.DEBUG = false;
            harmonyHotkeys.PatchAll(Assembly.GetExecutingAssembly());

            // INITIALIZE
            saved = GetSettings<Hotkeys_SettingsSave>();
        }
    }
}

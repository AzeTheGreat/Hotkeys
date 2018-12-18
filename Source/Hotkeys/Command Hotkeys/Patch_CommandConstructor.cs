using Harmony;
using Verse;


namespace Hotkeys
{
    [HarmonyPatch(typeof(Command), MethodType.Constructor)]
    class Patch_CommandConstructor
    {
        static void Postfix(Command __instance)
        {
            Patch_ApplyGizmoHotkeys.newCommands.Add(__instance);
        }

        public static void CheckStaticCommands()
        {
            foreach (DesignationCategoryDef defCat in DefDatabase<DesignationCategoryDef>.AllDefsListForReading)
            {
                foreach (Designator des in defCat.AllResolvedDesignators)
                {
                    Patch_ApplyGizmoHotkeys.newCommands.Add(des);
                }
            }
        }
    }
}

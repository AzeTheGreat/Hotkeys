using Harmony;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.Sound;
using System.Collections.Generic;


namespace Hotkeys
{
    [HarmonyPatch(typeof(Game), nameof(Game.UpdatePlay))]
    public class Patch_DirectHotkeys
    {
        private static List<KeyBindingDef> lastFrameKeyDefs = new List<KeyBindingDef>();

        static void Postfix()
        {
            lastFrameKeyDefs.Clear();
            List<KeyBindingDef> keyDefs = new List<KeyBindingDef>();
            foreach (object obj in Find.Selector.SelectedObjects)
            {
                Thing t =  obj as Thing;
                if (t != null)
                {
                    List<Designator> allDesignators = Find.ReverseDesignatorDatabase.AllDesignators;

                    foreach (var des in allDesignators)
                    {
                        if (des.CanDesignateThing(t).Accepted)
                        {
                            keyDefs.Add(des.hotKey);
                            lastFrameKeyDefs = keyDefs.ListFullCopy();
                        }
                    }
                }
            }
        }

        static void Prefix()
        {
            if (Event.current.type != EventType.KeyDown) { return; }
            if (!Hotkeys.settings.useDirectHotkeys) { return; }
            if (CheckSelectedKeys()) { return; }
            
            var directKeys = DirectKeys.directKeys;
                for (int i = 0; i < directKeys.Count; i++)
                {
                    if (directKeys[i].keyDef.JustPressed)
                    {
                        var designator = directKeys[i].Designator;
                        if (designator != null)
                        {
                            SoundDefOf.SelectDesignator.PlayOneShotOnCamera((Map)null);
                            Find.DesignatorManager.Select(designator);
                        }
                    }
                }
        }

        private static bool CheckSelectedKeys()
        {
            bool test = false;

            foreach (var keyDef in lastFrameKeyDefs)
            {
                Log.Message(keyDef.defName);
                if (keyDef.JustPressed)
                {
                    test = true;
                }
            }

            return test;
        }
    }
}




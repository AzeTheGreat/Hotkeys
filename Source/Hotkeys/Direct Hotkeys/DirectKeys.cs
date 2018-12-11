using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;

namespace Hotkeys
{
    [StaticConstructorOnStartup]
    static class DirectKeys
    {
        public static List<DirectKeyData> directKeys;
        public static bool gizmoTriggered = false;

        static DirectKeys()
        {
            directKeys = Hotkeys.settings.directKeys;
            BuildDirectKeyDefs();
        }

        public static void BuildDirectKeyDefs()
        {
            if (directKeys == null)
            {
                directKeys = new List<DirectKeyData>();
            }

            for (int i = 0; i < directKeys.Count; i++)
            {
                directKeys[i].CreateKeyDef(i);
            }
            KeyPrefs.Init();
        }
    }
}

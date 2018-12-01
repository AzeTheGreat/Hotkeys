using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace Hotkeys
{
    public class KeyModData : IExposable
    {
        public List<KeyCode> keyBindModsA;
        public List<KeyCode> keyBindModsB;
        public List<KeyCode> overlappingKeysA;
        public List<KeyCode> overlappingKeysB;

        public KeyModData()
        {
            keyBindModsA = new List<KeyCode>();
            keyBindModsB = new List<KeyCode>();
            overlappingKeysA = new List<KeyCode>();
            overlappingKeysB = new List<KeyCode>();
        }

        public bool AllModifierKeysDown(KeyBindingDef keyDef, bool resultA, bool resultB)
        {
            KeyPrefsData keyPrefData = KeyPrefs.KeyPrefsData.Clone();
            bool allDownA = true;
            bool allDownB = true;

            if (keyBindModsA != null)
            {
                foreach (KeyCode keyCode in keyBindModsA)
                {
                    if (!Input.GetKey(keyCode))
                    {
                        allDownA = false;
                    }
                }

                if (overlappingKeysA != null)
                {
                    foreach (KeyCode code in overlappingKeysA)
                    {
                        if (!keyBindModsA.Contains(code) && Input.GetKey(code))
                        {
                            allDownA = false;
                        }
                    }
                }
            }
            

            if (keyBindModsB != null)
            {
                foreach (KeyCode keyCode in keyBindModsB)
                {
                    if (!Input.GetKey(keyCode))
                    {
                        allDownA = false;
                    }
                }

                if (overlappingKeysB != null)
                {
                    foreach (KeyCode code in overlappingKeysB)
                    {
                        if (!keyBindModsB.Contains(code) && Input.GetKey(code))
                        {
                            allDownB = false;
                        }
                    }
                }
            }
            

            return (allDownA && resultA) || (allDownB && resultB);
        }

        public void ExposeData()
        {
            Scribe_Collections.Look(ref keyBindModsA, "Hotkeys_KeyBindModsA", LookMode.Value);
            Scribe_Collections.Look(ref keyBindModsB, "Hotkeys_KeyBindModsB", LookMode.Value);
        }
    }
}

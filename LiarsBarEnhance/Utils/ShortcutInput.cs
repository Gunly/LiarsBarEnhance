using BepInEx.Configuration;
using UnityEngine;

namespace LiarsBarEnhance.Utils
{
    internal class ShortcutInput
    {
        public static bool IsDown(ConfigEntry<KeyboardShortcut> entry)
        {
            return !KeyboardShortcut.Empty.Equals(entry.Value) && Input.GetKeyDown(entry.Value.MainKey);
        }

        public static bool IsUp(ConfigEntry<KeyboardShortcut> entry)
        {
            return !KeyboardShortcut.Empty.Equals(entry.Value) && Input.GetKeyUp(entry.Value.MainKey);
        }

        public static bool IsPressed(ConfigEntry<KeyboardShortcut> entry)
        {
            return !KeyboardShortcut.Empty.Equals(entry.Value) && Input.GetKey(entry.Value.MainKey);
        }
    }
}

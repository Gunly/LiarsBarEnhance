using BepInEx.Configuration;

using System.Linq;

using UnityEngine;

namespace LiarsBarEnhance.Utils
{
    public static class ShortcutInput
    {
        public static bool IsDown(this ConfigEntry<KeyboardShortcut> entry)
        {
            return !KeyboardShortcut.Empty.Equals(entry.Value) && Input.GetKeyDown(entry.Value.MainKey) && entry.Value.Modifiers.All((e) => Input.GetKey(e));
        }

        public static bool IsUp(this ConfigEntry<KeyboardShortcut> entry)
        {
            return !KeyboardShortcut.Empty.Equals(entry.Value) && Input.GetKeyUp(entry.Value.MainKey) && entry.Value.Modifiers.All((e) => Input.GetKey(e));
        }

        public static bool IsPressed(this ConfigEntry<KeyboardShortcut> entry)
        {
            return !KeyboardShortcut.Empty.Equals(entry.Value) && Input.GetKey(entry.Value.MainKey) && entry.Value.Modifiers.All((e) => Input.GetKey(e));
        }
    }
}

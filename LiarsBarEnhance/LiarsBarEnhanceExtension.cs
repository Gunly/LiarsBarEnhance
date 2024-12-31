using BepInEx.Configuration;

using System;
using System.Linq;

using UnityEngine;

namespace LiarsBarEnhance;

public static class LiarsBarEnhanceExtension
{
    public static bool IsDown(this ConfigEntry<KeyboardShortcut> entry)
    {
        return entry.Value.MainKey != KeyCode.None && Input.GetKeyDown(entry.Value.MainKey) && ModifierKeyTest(entry.Value);
    }

    public static bool IsUp(this ConfigEntry<KeyboardShortcut> entry)
    {
        return entry.Value.MainKey != KeyCode.None && Input.GetKeyUp(entry.Value.MainKey) && ModifierKeyTest(entry.Value);
    }

    public static bool IsPressed(this ConfigEntry<KeyboardShortcut> entry)
    {
        return entry.Value.MainKey != KeyCode.None && Input.GetKey(entry.Value.MainKey) && ModifierKeyTest(entry.Value);
    }

    private static readonly KeyCode[] modifierKeys = [KeyCode.LeftShift, KeyCode.LeftControl, KeyCode.LeftAlt, KeyCode.RightShift, KeyCode.RightControl, KeyCode.RightAlt];
    private static bool ModifierKeyTest(KeyboardShortcut key)
    {
        var modifiers = key.Modifiers;
        return modifiers.All(e => Input.GetKey(e)) && (modifierKeys.Contains(key.MainKey) || modifierKeys.Except(modifiers).All(e => !Input.GetKey(e)));
    }
}

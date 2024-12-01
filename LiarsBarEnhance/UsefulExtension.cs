using BepInEx.Configuration;

using LiarsBarEnhance.Utils;

using System;
using System.Linq;

using UnityEngine;

namespace LiarsBarEnhance;

public static class UsefulExtension
{
    public static T AddComponentIfNotExist<T>(this GameObject gameObject) where T : Component
    {
        if (gameObject.GetComponent<T>() == null)
            return gameObject.AddComponent<T>();
        return gameObject.GetComponent<T>();
    }

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

    public static Quaternion ToQuaternion(this Vector3 vector) => Quaternion.Euler(vector.x, vector.y, vector.z);

    public static float GetYaw(this CharController charController)
    {
        return FastMemberAccessor<CharController, float>.Get(charController, "_cinemachineTargetYaw");
    }
    public static void SetYaw(this CharController charController, float value)
    {
        FastMemberAccessor<CharController, float>.Set(charController, "_cinemachineTargetYaw", value);
    }
    public static void AddYaw(this CharController charController, float value)
    {
        charController.SetYaw(charController.GetYaw() + value);
    }
    public static float GetPitch(this CharController charController)
    {
        return FastMemberAccessor<CharController, float>.Get(charController, "_cinemachineTargetPitch");
    }
    public static void SetPitch(this CharController charController, float value)
    {
        FastMemberAccessor<CharController, float>.Set(charController, "_cinemachineTargetPitch", value);
    }
    public static void AddPitch(this CharController charController, float value)
    {
        charController.SetPitch(charController.GetPitch() + value);
    }

    public static bool PluginControl(this Manager manager) => !manager.GamePaused && !manager.Chatting;

    public static string GetEnumDescription<T>(this T val) where T : Enum
    {
        var field = val.GetType().GetField(val.ToString());
        var customAttribute = Attribute.GetCustomAttribute(field, typeof(System.ComponentModel.DescriptionAttribute));
        return customAttribute == null ? val.ToString() : ((System.ComponentModel.DescriptionAttribute)customAttribute).Description;
    }
}
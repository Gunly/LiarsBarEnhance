﻿using BepInEx.Configuration;

using LiarsBarEnhance.Utils;

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

    public static Quaternion ToQuaternion(this Vector3 vector)
    {
        return Quaternion.Euler(vector.x, vector.y, vector.z);
    }

    public static bool Paused(this CharController charController)
    {
        return FastMemberAccessor<CharController, Manager>.Get(charController, "manager").GamePaused;
    }

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
}
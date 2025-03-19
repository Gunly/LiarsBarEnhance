using HarmonyLib;

using LiarsBarEnhance.Utils;

using System;

using TMPro;

using UnityEngine;
using UnityEngine.UI;

namespace LiarsBarEnhance;

public static class UsefulExtension
{
    public static T AddComponentIfNotExist<T>(this GameObject gameObject) where T : Component
    {
        if (gameObject.GetComponent<T>() == null)
            return gameObject.AddComponent<T>();
        return gameObject.GetComponent<T>();
    }

    public static Quaternion ToQuaternion(this Vector3 vector) => Quaternion.Euler(vector.x, vector.y, vector.z);

    public static bool PluginControl(this Manager manager) => !manager.GamePaused && !manager.Chatting;

    public static V GetField<T, V>(this T instance, string fieldName) where T : class => FastMemberAccessor<T, V>.Get(instance, fieldName);
    public static void SetField<T, V>(this T instance, string fieldName, V value) where T : class => FastMemberAccessor<T, V>.Set(instance, fieldName, value);

    public static void CallMethod<T>(this T instance, string methodName, Type[] parameterTypes = null, object[] parameters = null) where T : class =>
        AccessTools.Method(typeof(T).Name + ":" + methodName, parameterTypes).Invoke(instance, parameters);

    public static TextMeshPro NameText(this PlayerStats instance) => instance.GetField<PlayerStats, TextMeshPro>("NameText");
    public static InputField inputField(this ChatNetwork instance) => instance.GetField<ChatNetwork, InputField>("inputField");
    public static Fonts fonts(this FontChanger instance) => instance.GetField<FontChanger, Fonts>("fonts");
    public static TextMeshProUGUI TipText(this LobbyController instance) => instance.GetField<LobbyController, TextMeshProUGUI>("TipText");
}
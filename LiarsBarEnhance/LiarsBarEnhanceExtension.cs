using BepInEx.Configuration;

using System;
using System.Linq;

using TMPro;

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
        return modifiers.All(Input.GetKey) && (modifierKeys.Contains(key.MainKey) || modifierKeys.Except(modifiers).All(e => !Input.GetKey(e)));
    }

    public static Manager manager(this CharController instance) => instance.GetField<CharController, Manager>("manager");
    public static PlayerStats playerStats(this CharController instance) => instance.GetField<CharController, PlayerStats>("playerStats");
    public static TextMeshPro RoundText<T>(this T instance) where T : CharController => instance.GetField<T, TextMeshPro>("RoundText");
    public static GameObject ZarKafaSprite(this DiceGamePlay instance) => instance.GetField<DiceGamePlay, GameObject>("ZarKafaSprite");
    public static GameObject ZarText(this DiceGamePlay instance) => instance.GetField<DiceGamePlay, GameObject>("ZarText");

    public static float MinX(this CharController instance) => instance.GetField<CharController, float>("MinX");
    public static float MaxX(this CharController instance) => instance.GetField<CharController, float>("MaxX");
    public static float MinY(this CharController instance) => instance.GetField<CharController, float>("MinY");
    public static float MaxY(this CharController instance) => instance.GetField<CharController, float>("MaxY");

    public static float GetYaw(this CharController instance) => instance.GetField<CharController, float>("_cinemachineTargetYaw");
    public static void SetYaw(this CharController instance, float value) => instance.SetField("_cinemachineTargetYaw", value);
    public static void AddYaw(this CharController instance, float value) => instance.SetYaw(instance.GetYaw() + value);
    public static float GetPitch(this CharController instance) => instance.GetField<CharController, float>("_cinemachineTargetPitch");
    public static void SetPitch(this CharController instance, float value) => instance.SetField("_cinemachineTargetPitch", value);
    public static void AddPitch(this CharController instance, float value) => instance.SetPitch(instance.GetPitch() + value);


    public static void Owned(this Dice instance, bool owned) => instance.SetField("Owned", owned);
    public static bool Owned(this Dice instance) => instance.GetField<Dice, bool>("Owned");
    public static MeshRenderer renderer(this Dice instance) => instance.GetField<Dice, MeshRenderer>("renderer");
    public static void CmdSetPlayerName(this PlayerObjectController instance, string name) => instance.CallMethod("CmdSetPlayerName", [typeof(string)], [name]);
    public static Transform RankParrentTable(this Statsui instance) => instance.GetField<Statsui, Transform>("RankParrentTable");
}

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

    public static Manager manager(this CharController instance) => instance.manager;
    public static PlayerStats playerStats(this CharController instance) => instance.playerStats;
    public static TextMeshPro RoundText(this BlorfGamePlay instance) => instance.RoundText;
    public static TextMeshPro RoundText(this BlorfGamePlayMatchMaking instance) => instance.RoundText;
    public static TextMeshPro RoundText(this ChaosGamePlay instance) => instance.RoundText;
    public static GameObject ZarKafaSprite(this DiceGamePlay instance) => instance.ZarKafaSprite;
    public static GameObject ZarText(this DiceGamePlay instance) => instance.ZarText;

    public static float MinX(this CharController instance) => instance.MinX;
    public static float MaxX(this CharController instance) => instance.MaxX;
    public static float MinY(this CharController instance) => instance.MinY;
    public static float MaxY(this CharController instance) => instance.MaxY;

    public static float GetYaw(this CharController instance) => instance._cinemachineTargetYaw;
    public static void SetYaw(this CharController instance, float value) => instance._cinemachineTargetYaw = value;
    public static void AddYaw(this CharController instance, float value) => instance._cinemachineTargetYaw += value;
    public static float GetPitch(this CharController instance) => instance._cinemachineTargetPitch;
    public static void SetPitch(this CharController instance, float value) => instance._cinemachineTargetPitch = value;
    public static void AddPitch(this CharController instance, float value) => instance._cinemachineTargetPitch += value;


    public static void Owned(this Dice instance, bool owned) => instance.Owned = owned;
    public static bool Owned(this Dice instance) => instance.Owned;
    public static MeshRenderer renderer(this Dice instance) => instance.renderer;
    public static void CmdSetPlayerName(this PlayerObjectController instance, string name) => instance.CmdSetPlayerName(name);
    public static Transform RankParrentTable(this Statsui instance) => instance.RankParrentTable;
}

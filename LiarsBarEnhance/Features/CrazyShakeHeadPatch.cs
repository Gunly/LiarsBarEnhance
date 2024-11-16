using HarmonyLib;

using LiarsBarEnhance.Utils;

using UnityEngine;
using CharControllerFloatMemberAccessor = LiarsBarEnhance.Utils.FastMemberAccessor<CharController, float>;

namespace LiarsBarEnhance.Features;

[HarmonyPatch]
public class CrazyShakeHeadPatch
{
    private const string CINEMACHINE_TARGET_YAW_FIELD_NAME = "_cinemachineTargetYaw";
    private const string CINEMACHINE_TARGET_PITCH_FIELD_NAME = "_cinemachineTargetPitch";

    private static float _savedYaw, _savedPitch;

    public static bool IsEnabled { get; private set; }

    public static void SetEnabled(CharController instance, bool value)
    {
        if (value == IsEnabled)
            return;
        IsEnabled = value;

        if (value)
        {
            _savedYaw = CharControllerFloatMemberAccessor.Get(instance, CINEMACHINE_TARGET_YAW_FIELD_NAME);
            _savedPitch = CharControllerFloatMemberAccessor.Get(instance, CINEMACHINE_TARGET_PITCH_FIELD_NAME);
        }
        else
        {
            CharControllerFloatMemberAccessor.Set(instance, CINEMACHINE_TARGET_YAW_FIELD_NAME, _savedYaw);
            CharControllerFloatMemberAccessor.Set(instance, CINEMACHINE_TARGET_PITCH_FIELD_NAME, _savedPitch);
        }
    }

    public static void ToggleEnabled(CharController charController)
        => SetEnabled(charController, !IsEnabled);

    [HarmonyPatch(typeof(CharController), "RotateInFrame")]
    [HarmonyPostfix]
    private static void RotateInFramePostfix(CharController __instance, float ___MinX, float ___MaxX, float ___MinY, float ___MaxY)
    {
        if (ShortcutInput.IsDown(Plugin.KeyViewCrazyShakeHead))
            ToggleEnabled(__instance);

        if (!IsEnabled)
            return;

        var limited = ShortcutInput.IsPressed(Plugin.KeyViewCrazyShakeHead);
        var x = Random.Range(limited ? ___MinX : 0, limited ? ___MaxX : 360);
        var y = Random.Range(limited ? ___MinY : 0, limited ? ___MaxY : 360);

        CharControllerFloatMemberAccessor.Set(__instance, CINEMACHINE_TARGET_YAW_FIELD_NAME, x);
        CharControllerFloatMemberAccessor.Set(__instance, CINEMACHINE_TARGET_PITCH_FIELD_NAME, y);

        __instance.HeadPivot.transform.localRotation = Quaternion.Euler(x, 0f, y);
    }
}
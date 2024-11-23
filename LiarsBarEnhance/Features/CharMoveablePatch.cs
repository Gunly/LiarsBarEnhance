using HarmonyLib;

using LiarsBarEnhance.Components;

using UnityEngine;

namespace LiarsBarEnhance.Features;

[HarmonyPatch]
public class CharMoveablePatch
{
    public static float CinemachineTargetRoll = 0f;

    [HarmonyPatch(typeof(CharController), nameof(CharController.Start))]
    [HarmonyPostfix]
    public static void StartPostfix(CharController __instance)
    {
        if (!__instance.isOwned) return;
        __instance.gameObject.AddComponent<FpController>();
        CinemachineTargetRoll = 0f;
        Debug.Log($"{nameof(CharMoveablePatch)}: {nameof(FpController)} added to {nameof(CharController)}");
    }

    [HarmonyPatch(typeof(CharController), nameof(CharController.Update))]
    [HarmonyPostfix]
    public static void UpdatePostfix(CharController __instance, float ____cinemachineTargetYaw, float ____cinemachineTargetPitch)
    {
        if (!__instance.isOwned) return;
        if (Plugin.KeyViewRemoveRotationLimit.IsDown() && !__instance.Paused())
        {
            Plugin.BooleanViewRemoveRotationLimit.Value = !Plugin.BooleanViewRemoveRotationLimit.Value;
        }
        if (Plugin.KeyViewClockwise.IsPressed()) CinemachineTargetRoll -= 2f;
        if (Plugin.KeyViewAnticlockwise.IsPressed()) CinemachineTargetRoll += 2f;
        __instance.HeadPivot.transform.localRotation = Quaternion.Euler(____cinemachineTargetYaw, CinemachineTargetRoll, ____cinemachineTargetPitch);
    }
}
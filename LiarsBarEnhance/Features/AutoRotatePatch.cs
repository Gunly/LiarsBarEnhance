using HarmonyLib;

using UnityEngine;

namespace LiarsBarEnhance.Features;

[HarmonyPatch]
public class AutoRotatePatch
{
    public static bool Rotating = false;

    [HarmonyPatch(typeof(CharController), nameof(CharController.Start))]
    [HarmonyPostfix]
    public static void StartPostfix(CharController __instance)
    {
        Rotating = false;
    }

    [HarmonyPatch(typeof(CharController), nameof(CharController.Update))]
    [HarmonyPostfix]
    public static void UpdatePostfix(CharController __instance)
    {
        if (!__instance.isOwned) return;
        if (Plugin.KeyRotateAuto.IsDown() && __instance.manager.PluginControl()) Rotating = !Rotating;
        if (!Rotating) return;
        var rotateSpeed = Plugin.FloatAutoRotateSpeed.Value * 6;
        if ((Plugin.DirectionRotateState.Value & RotateDirection.Pitch) != RotateDirection.None)
            __instance.transform.Rotate(rotateSpeed * Vector3.right, Space.Self);
        if ((Plugin.DirectionRotateState.Value & RotateDirection.Roll) != RotateDirection.None)
            __instance.transform.Rotate(rotateSpeed * Vector3.forward, Space.Self);
        if ((Plugin.DirectionRotateState.Value & RotateDirection.Yaw) != RotateDirection.None)
            __instance.transform.Rotate(rotateSpeed * Vector3.up, Space.Self);
        if ((Plugin.DirectionRotateState.Value & RotateDirection.HeadYaw) != RotateDirection.None)
            __instance._cinemachineTargetYaw += rotateSpeed;
        if ((Plugin.DirectionRotateState.Value & RotateDirection.HeadPitch) != RotateDirection.None)
            __instance._cinemachineTargetPitch += rotateSpeed;
        if ((Plugin.DirectionRotateState.Value & RotateDirection.HeadRoll) != RotateDirection.None)
            RemoveHeadRotationlimitPatch.CinemachineTargetRoll += rotateSpeed;
    }
}
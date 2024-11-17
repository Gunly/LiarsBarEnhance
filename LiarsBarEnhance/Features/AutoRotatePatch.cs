using HarmonyLib;

using UnityEngine;

namespace LiarsBarEnhance.Features;

[HarmonyPatch]
public class AutoRotatePatch
{
    private static bool rotating = false;

    [HarmonyPatch(typeof(CharController), nameof(CharController.Update))]
    [HarmonyPostfix]
    public static void UpdatePostfix(CharController __instance)
    {
        if (!__instance.isOwned) return;
        if (Plugin.KeyRotateAuto.IsDown()) rotating = !rotating;
        if (!rotating) return;
        var rotateSpeed = Plugin.FloatAutoRotateSpeed.Value * 6;
        if ((Plugin.DirectionRotateState.Value & RotateDirection.Pitch) != RotateDirection.None)
            __instance.transform.Rotate(rotateSpeed * Vector3.right, Space.Self);
        if ((Plugin.DirectionRotateState.Value & RotateDirection.Roll) != RotateDirection.None)
            __instance.transform.Rotate(rotateSpeed * Vector3.forward, Space.Self);
        if ((Plugin.DirectionRotateState.Value & RotateDirection.Yaw) != RotateDirection.None)
            __instance.transform.Rotate(rotateSpeed * Vector3.up, Space.Self);
        if ((Plugin.DirectionRotateState.Value & RotateDirection.HeadYaw) != RotateDirection.None)
            __instance.AddYaw(rotateSpeed);
        if ((Plugin.DirectionRotateState.Value & RotateDirection.HeadPitch) != RotateDirection.None)
            __instance.AddPitch(rotateSpeed);
    }
}
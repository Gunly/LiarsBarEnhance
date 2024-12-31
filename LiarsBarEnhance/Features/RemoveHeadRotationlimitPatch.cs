using HarmonyLib;

using UnityEngine;

namespace LiarsBarEnhance.Features;

[HarmonyPatch]
public class RemoveHeadRotationlimitPatch
{
    public static float CinemachineTargetRoll { get; set; }
    private static float yaw, pitch;
    [HarmonyPatch(typeof(CharController), nameof(CharController.Update))]
    [HarmonyPrefix]
    public static void UpdatePrefix(CharController __instance)
    {
        if (!__instance.isOwned) return;
        if (Plugin.BooleanViewRemoveRotationLimit.Value)
        {
            yaw = __instance._cinemachineTargetYaw;
            pitch = __instance._cinemachineTargetPitch;
        }

        if (!__instance.playerStats.Dead || !Plugin.BooleanViewRemoveRotationLimit.Value)
        {
            if (Plugin.KeyViewClockwise.IsPressed()) CinemachineTargetRoll -= 2f;
            if (Plugin.KeyViewAnticlockwise.IsPressed()) CinemachineTargetRoll += 2f;
        }
    }

    [HarmonyPatch(typeof(CharController), nameof(CharController.Update))]
    [HarmonyPostfix]
    public static void UpdatePostfix(CharController __instance)
    {
        if (__instance.isOwned && Plugin.BooleanViewRemoveRotationLimit.Value && __instance.manager.PluginControl())
        {
            var sensivty = PlayerPrefs.GetFloat("MouseSensivity", 50f);
            var x = Input.GetAxis("Mouse X") * Time.deltaTime * sensivty;
            var y = Input.GetAxis("Mouse Y") * Time.deltaTime * sensivty;
            yaw -= x;
            pitch += y;
            if (yaw < -180f) yaw += 360f;
            if (yaw > 180f) yaw -= 360f;
            if (pitch < -180f) pitch += 360f;
            if (pitch > 180f) pitch -= 360f;
            __instance._cinemachineTargetYaw = yaw;
            __instance._cinemachineTargetPitch = pitch;
            __instance.HeadPivot.transform.localRotation = new Vector3 (yaw, CinemachineTargetRoll, pitch).ToQuaternion();
        }
    }
}
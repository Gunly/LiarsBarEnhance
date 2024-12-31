using HarmonyLib;

using UnityEngine;

namespace LiarsBarEnhance.Features;

[HarmonyPatch]
public class CrazyShakeHeadPatch
{
    private static float savedYaw, savedPitch;
    private static bool isEnabled;

    [HarmonyPatch(typeof(CharController), nameof(CharController.Update))]
    [HarmonyPostfix]
    public static void UpdatePostfix(CharController __instance)
    {
        if (!__instance.isOwned) return;
        if (__instance.manager.PluginControl())
        {
            if (Plugin.KeyViewCrazyShakeHead.IsDown())
            {
                isEnabled = !isEnabled;
                if (isEnabled)
                {
                    savedYaw = __instance._cinemachineTargetYaw;
                    savedPitch = __instance._cinemachineTargetPitch;
                }
                else
                {
                    __instance._cinemachineTargetYaw = savedYaw;
                    __instance._cinemachineTargetPitch = savedPitch;
                }
            }
        }
        if (isEnabled)
        {
            var limited = Plugin.KeyViewCrazyShakeHead.IsPressed();
            var x = Random.Range(limited ? __instance.MinX : 0, limited ? __instance.MaxX : 360);
            var y = Random.Range(limited ? __instance.MinY : 0, limited ? __instance.MaxY : 360);

            __instance._cinemachineTargetYaw = x;
            __instance._cinemachineTargetPitch = y;
        }
        __instance.HeadPivot.transform.localRotation = Quaternion.Euler(__instance._cinemachineTargetYaw, RemoveHeadRotationlimitPatch.CinemachineTargetRoll, __instance._cinemachineTargetPitch);
    }
}
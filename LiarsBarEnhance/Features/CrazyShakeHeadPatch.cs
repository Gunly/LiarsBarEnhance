using HarmonyLib;

using UnityEngine;

namespace LiarsBarEnhance.Features;

[HarmonyPatch]
public class CrazyShakeHeadPatch
{
    private static float savedYaw, savedPitch;
    public static float CinemachineTargetRoll = 0f;
    private static bool isEnabled;

    [HarmonyPatch(typeof(CharController), nameof(CharController.Update))]
    [HarmonyPostfix]
    public static void UpdatePostfix(CharController __instance, Manager ___manager, ref float ____cinemachineTargetYaw, ref float ____cinemachineTargetPitch,
        float ___MinX, float ___MaxX, float ___MinY, float ___MaxY)
    {
        if (!__instance.isOwned) return;
        if (___manager.PluginControl())
        {
            if (Plugin.KeyViewCrazyShakeHead.IsDown())
            {
                isEnabled = !isEnabled;
                if (isEnabled)
                {
                    savedYaw = ____cinemachineTargetYaw;
                    savedPitch = ____cinemachineTargetPitch;
                }
                else
                {
                    ____cinemachineTargetYaw = savedYaw;
                    ____cinemachineTargetPitch = savedPitch;
                }
            }

            if (!Plugin.BooleanViewRemoveRotationLimit.Value)
            {
                if (Plugin.KeyViewClockwise.IsPressed()) CinemachineTargetRoll -= 2f;
                if (Plugin.KeyViewAnticlockwise.IsPressed()) CinemachineTargetRoll += 2f;
            }
        }
        if (isEnabled)
        {
            var limited = Plugin.KeyViewCrazyShakeHead.IsPressed();
            var x = Random.Range(limited ? ___MinX : 0, limited ? ___MaxX : 360);
            var y = Random.Range(limited ? ___MinY : 0, limited ? ___MaxY : 360);

            __instance.SetYaw(x);
            __instance.SetPitch(y);
        }
        __instance.HeadPivot.transform.localRotation = Quaternion.Euler(____cinemachineTargetYaw, CinemachineTargetRoll, ____cinemachineTargetPitch);
    }
}
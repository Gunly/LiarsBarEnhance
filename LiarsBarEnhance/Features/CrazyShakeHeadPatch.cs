using HarmonyLib;

using UnityEngine;

namespace LiarsBarEnhance.Features;

[HarmonyPatch]
public class CrazyShakeHeadPatch
{
    private static float savedYaw, savedPitch;
    private static bool isEnabled;

    [HarmonyPatch(typeof(CharController), "Update")]
    [HarmonyPostfix]
    public static void UpdatePostfix(CharController __instance)
    {
        if (!__instance.isOwned) return;
        if (__instance.manager().PluginControl())
        {
            if (Plugin.KeyViewCrazyShakeHead.IsDown())
            {
                isEnabled = !isEnabled;
                if (isEnabled)
                {
                    savedYaw = __instance.GetYaw();
                    savedPitch = __instance.GetPitch();
                }
                else
                {
                    __instance.SetYaw(savedYaw);
                    __instance.SetPitch(savedPitch);
                }
            }
        }
        if (isEnabled)
        {
            var limited = Plugin.KeyViewCrazyShakeHead.IsPressed();
            var x = Random.Range(limited ? __instance.MinX() : 0, limited ? __instance.MaxX() : 360);
            var y = Random.Range(limited ? __instance.MinY() : 0, limited ? __instance.MaxY() : 360);

            __instance.SetYaw(x);
            __instance.SetPitch(y);
        }
        __instance.HeadPivot.transform.localRotation = Quaternion.Euler(__instance.GetYaw(), RemoveHeadRotationlimitPatch.CinemachineTargetRoll, __instance.GetPitch());
    }
}
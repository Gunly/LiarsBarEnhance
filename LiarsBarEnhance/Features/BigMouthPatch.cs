using HarmonyLib;

using UnityEngine;

namespace LiarsBarEnhance.Features;

[HarmonyPatch]
public class BigMouthPatch
{
    private static float mouthOpen = 0f;

    [HarmonyPatch(typeof(FaceAnimator), nameof(FaceAnimator.Start))]
    [HarmonyPostfix]
    public static void StartPostfix(FaceAnimator __instance)
    {
        if (!__instance.isOwned) return;
        Plugin.FloatBigMouthAngle.SettingChanged += (_, _) =>
        {
            mouthOpen = 1f;
        };
    }

    [HarmonyPatch(typeof(FaceAnimator), nameof(FaceAnimator.Update))]
    [HarmonyPostfix]
    public static void UpdatePostfix(FaceAnimator __instance)
    {
        if (!__instance.isOwned) return;

        if (mouthOpen > 0f || Plugin.KeyCustomBigMouth.IsPressed())
        {
            if (mouthOpen > 0f) mouthOpen -= Time.deltaTime;
            __instance.Mouth.transform.localEulerAngles = new Vector3(0f, 0f, -Plugin.FloatBigMouthAngle.Value);
        }
    }
}
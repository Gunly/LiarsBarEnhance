using HarmonyLib;

using UnityEngine;

namespace LiarsBarEnhance.Features;

[HarmonyPatch]
public class BigMouthPatch
{
    public static float MouthOpen { get; set; }

    [HarmonyPatch(typeof(FaceAnimator), nameof(FaceAnimator.Update))]
    [HarmonyPostfix]
    public static void UpdatePostfix(FaceAnimator __instance)
    {
        if (!__instance.isOwned) return;

        if (MouthOpen > 0f || Plugin.KeyCustomBigMouth.IsPressed())
        {
            if (MouthOpen > 0f) MouthOpen -= Time.deltaTime;
            __instance.Mouth.transform.localEulerAngles = new Vector3(0f, 0f, -Plugin.FloatBigMouthAngle.Value);
        }
    }
}
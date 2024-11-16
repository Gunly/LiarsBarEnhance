using HarmonyLib;

using LiarsBarEnhance.Components;
using LiarsBarEnhance.Utils;

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
        __instance.gameObject.AddComponent<FpController>();
        Debug.Log($"{nameof(CharMoveablePatch)}: {nameof(FpController)} added to {nameof(CharController)}");
    }

    [HarmonyPatch(typeof(CharController), nameof(CharController.Update))]
    [HarmonyPostfix]
    public static void UpdatePostfix(CharController __instance, float ____cinemachineTargetYaw, float ____cinemachineTargetPitch)
    {
        if (__instance.isOwned)
        {
            if (ShortcutInput.IsPressed(Plugin.KeyViewClockwise))
                CinemachineTargetRoll -= 1f;
            if (ShortcutInput.IsPressed(Plugin.KeyViewAnticlockwise))
                CinemachineTargetRoll += 1f;
            __instance.HeadPivot.transform.localRotation = Quaternion.Euler(____cinemachineTargetYaw, CinemachineTargetRoll, ____cinemachineTargetPitch);
        }
    }
}
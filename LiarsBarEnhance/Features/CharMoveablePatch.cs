using HarmonyLib;

using LiarsBarEnhance.Components;

using UnityEngine;

namespace LiarsBarEnhance.Features;

[HarmonyPatch]
public class CharMoveablePatch
{
    [HarmonyPatch(typeof(CharController), nameof(CharController.Start))]
    [HarmonyPostfix]
    public static void StartPostfix(CharController __instance)
    {
        if (!__instance.isOwned) return;
        __instance.gameObject.AddComponent<FpController>();
        CrazyShakeHeadPatch.CinemachineTargetRoll = 0f;
        Debug.Log($"{nameof(CharMoveablePatch)}: {nameof(FpController)} added to {nameof(CharController)}");
    }
}
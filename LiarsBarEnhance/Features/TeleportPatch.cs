﻿using HarmonyLib;

namespace LiarsBarEnhance.Features;

[HarmonyPatch]
public class TeleportPatch
{
    [HarmonyPatch(typeof(CharController), nameof(CharController.Update))]
    [HarmonyPostfix]
    public static void UpdatePostfix(CharController __instance)
    {
        if (!__instance.isOwned) return;
        for (var i = 0; i < Plugin.InitPositionNumValue; i++)
        {
            if (Plugin.KeyPosition[i].Value.IsDown() && !__instance.Paused())
            {
                __instance.transform.localPosition = Plugin.VectorPosition[i].Value;
                __instance.transform.localRotation = Plugin.VectorRotation[i].Value.ToQuaternion();
                break;
            }
        }
    }
}
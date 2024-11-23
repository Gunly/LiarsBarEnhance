﻿using HarmonyLib;

using UnityEngine;

namespace LiarsBarEnhance.Features;

[HarmonyPatch]
public class CharScalePatch
{
    private static Vector3 initScale;

    [HarmonyPatch(typeof(CharController), nameof(CharController.Start))]
    [HarmonyPostfix]
    public static void StartPostfix(CharController __instance)
    {
        if (!__instance.isOwned) return;
        initScale = __instance.transform.localScale;
        Plugin.FloatCustomPlayerScale.SettingChanged += (s, a) =>
        {
            var scale = (Plugin.FloatCustomPlayerScale.Value - 0.5f) * 8f;
            if (scale > 0f) scale += 1f;
            else scale = 1 / (1 - scale);
            __instance.transform.localScale = initScale * scale;
        };
    }
}
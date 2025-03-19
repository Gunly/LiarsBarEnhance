using HarmonyLib;

using UnityEngine;

namespace LiarsBarEnhance.Features;

[HarmonyPatch]
public class CharScalePatch
{
    private static Vector3 initScale;

    [HarmonyPatch(typeof(CharController), "Start")]
    [HarmonyPostfix]
    public static void StartPostfix(CharController __instance)
    {
        if (!__instance.isOwned) return;
        initScale = __instance.transform.localScale;
        ChScale(__instance.transform);
        Plugin.FloatCustomPlayerScale.SettingChanged += (_, _) => ChScale(__instance.transform);
    }

    private static void ChScale(Transform transform)
    {
        var scale = (Plugin.FloatCustomPlayerScale.Value - 0.5f) * 8f;
        if (scale > 0f) scale += 1f;
        else scale = 1 / (1 - scale);
        transform.localScale = initScale * scale;
    }
}
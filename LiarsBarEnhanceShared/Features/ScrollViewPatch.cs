using HarmonyLib;

using UnityEngine;
using UnityEngine.UI;

namespace LiarsBarEnhance.Features;

[HarmonyPatch]
public class ScrollViewPatch
{
    [HarmonyPatch(typeof(StateController), nameof(StateController.OnEnable))]
    [HarmonyPostfix]
    public static void OnEnablePostfix(StateController __instance)
    {
        var scrollView = __instance.gameObject.GetComponent<RectTransform>().Find("pnl_left/Scroll View");
        var panelLeft = scrollView?.GetComponent<ScrollRect>();
        if (panelLeft != null) panelLeft.scrollSensitivity = 60f;
    }
}
using HarmonyLib;

using LiarsBarEnhance.Components;
using LiarsBarEnhance.Utils;
using TMPro;
using UnityEngine;

namespace LiarsBarEnhance.Features;

[HarmonyPatch]
public class ShowSelfTopInfoPatch
{
    [HarmonyPatch(typeof(BlorfGamePlay), "UpdateCall")]
    [HarmonyPostfix]
    public static void UpdateCallPostfix(BlorfGamePlay __instance, PlayerStats ___playerStats, GameObject ___KartKafaSprite, TextMeshPro ___RoundText, int ___currentrevoler)
    {
        if (Plugin.BooleanCustomShowSelfInfo.Value)
        {
            ___RoundText.text = "(" + ___currentrevoler + "|6)";
            ___RoundText.gameObject.SetActive(true);
            if (___playerStats.HaveTurn)
            {
                ___KartKafaSprite.SetActive(true);
            }
        }
    }

    [HarmonyPatch(typeof(DiceGamePlay), "UpdateCall")]
    [HarmonyPostfix]
    public static void UpdateCallPostfix(DiceGamePlay __instance, PlayerStats ___playerStats, GameObject ___ZarKafaSprite)
    {
        if (Plugin.BooleanCustomShowSelfInfo.Value)
        {
            if (___playerStats.HaveTurn)
            {
                ___ZarKafaSprite.SetActive(true);
            }
        }
    }
}
using HarmonyLib;

using TMPro;
using UnityEngine;

namespace LiarsBarEnhance.Features;

[HarmonyPatch]
public class ShowSelfTopInfoPatch
{
    [HarmonyPatch(typeof(BlorfGamePlay), "UpdateCall")]
    [HarmonyPostfix]
    public static void UpdateCallPostfix(BlorfGamePlay __instance, PlayerStats ___playerStats, GameObject ___KartKafaSprite,
        TextMeshPro ___RoundText, int ___currentrevoler, int ___revolverbulllet)
    {
        if (Plugin.BooleanCustomShowSelfInfo.Value)
        {
#if CHEATRELEASE
            ___RoundText.text = $"({___currentrevoler}|{(Plugin.BooleanCheatBlorf.Value && Plugin.BooleanCheatBlorfHealth.Value ? ___revolverbulllet + 1 : 6)})";
#else
            ___RoundText.text = $"({___currentrevoler}|6)";
#endif
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
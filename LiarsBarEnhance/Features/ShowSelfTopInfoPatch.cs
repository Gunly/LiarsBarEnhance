using HarmonyLib;

using TMPro;
using UnityEngine;

namespace LiarsBarEnhance.Features;

[HarmonyPatch]
public class ShowSelfTopInfoPatch
{
    [HarmonyPatch(typeof(BlorfGamePlay), "UpdateCall")]
    [HarmonyPostfix]
    public static void UpdateCallPostfix(BlorfGamePlay __instance, PlayerStats ___playerStats, GameObject ___KartKafaSprite, TextMeshPro ___RoundText)
    {
        if (Plugin.BooleanCustomShowSelfInfo.Value)
        {
#if CHEATRELEASE
            ___RoundText.text = $"({__instance.Networkcurrentrevoler}|{(Plugin.BooleanCheatDeck.Value && Plugin.BooleanCheatDeckHealth.Value ? __instance.Networkrevolverbulllet + 1 : 6)})";
#else
            ___RoundText.text = $"({__instance.Networkcurrentrevoler}|6)";
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

    [HarmonyPatch(typeof(ChaosGamePlay), "UpdateCall")]
    [HarmonyPostfix]
    public static void UpdateCallPostfix(ChaosGamePlay __instance, PlayerStats ___playerStats, GameObject ___KartKafaSprite, TextMeshPro ___RoundText)
    {
        if (Plugin.BooleanCustomShowSelfInfo.Value)
        {
#if CHEATRELEASE
            ___RoundText.text = $"({__instance.Networkcurrentrevoler}|{(Plugin.BooleanCheatDeck.Value && Plugin.BooleanCheatDeckHealth.Value ? __instance.Networkrevolverbulllet + 1 : 6)})";
#else
            ___RoundText.text = $"({__instance.Networkcurrentrevoler}|6)";
#endif
            ___RoundText.gameObject.SetActive(true);
            if (___playerStats.HaveTurn)
            {
                ___KartKafaSprite.SetActive(true);
            }
        }
    }
}
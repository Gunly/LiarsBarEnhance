#if CHEATRELEASE
using HarmonyLib;

using System;

using UnityEngine;

namespace LiarsBarEnhance.Features;

[HarmonyPatch]
public class DiceCheatPatch
{
    public static readonly int[] diceCounts = new int[6];

    [HarmonyPatch(typeof(Dice), "Update")]
    [HarmonyPostfix]
    public static void UpdatePostfix(MeshRenderer ___renderer)
    {
        if (Plugin.BooleanCheatDice.Value)
        {
            ___renderer.material = Manager.Instance.zar1;
        }
    }

    [HarmonyPatch(typeof(DiceGamePlay), "UpdateCall")]
    [HarmonyPostfix]
    public static void UpdateCallPostfix(DiceGamePlay __instance, GameObject ___ZarText, Manager ___manager, PlayerStats ___playerStats)
    {
        if (Plugin.BooleanCheatDice.Value && !__instance.isOwned && Plugin.KeyCheatDiceShow.IsPressed() && !___manager.GamePaused && !___manager.Chatting)
        {
            ___ZarText.SetActive(value: true);
        }

        if (!___playerStats.Dead && Plugin.BooleanCheatDiceTotalDice.Value)
        {
            foreach (var value in __instance.DiceValues)
            {
                if (value != 1 || ___manager.DiceGame.DiceMode == DiceGamePlayManager.dicemode.Basic)
                {
                    diceCounts[value - 1]++;
                }
                else
                {
                    for (var i = 0; i < diceCounts.Length; i++) diceCounts[i]++;
                }
            }
        }

        if (__instance.isOwned)
        {
            for (var i = 0; i < __instance.DiceValues.Count; i++)
            {
                if (Plugin.KeyCheatChangeCardDice[4 - i].IsDown() && !___manager.GamePaused && !___manager.Chatting)
                {
                    if (__instance.DiceValues[i] < 6) __instance.DiceValues[i]++;
                    else __instance.DiceValues[i] = 1;
                }

            }
        }
    }

    [HarmonyPatch(typeof(DiceGamePlayManager), "Update")]
    [HarmonyPostfix]
    public static void UpdatePostfix()
    {
        if (Plugin.BooleanCheatDiceTotalDice.Value) Array.Fill(diceCounts, 0);
    }
}
#endif
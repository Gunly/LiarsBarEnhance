#if CHEATRELEASE
using HarmonyLib;

using System;
using System.Linq;

using UnityEngine;

namespace LiarsBarEnhance.Features;

[HarmonyPatch]
public class DiceCheatPatch
{
    public static readonly int[] diceCounts = new int[6];
    private static int first = 4;

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
        if (Plugin.BooleanCheatDice.Value && !__instance.isOwned && Plugin.KeyCheatDiceShow.IsPressed())
        {
            ___ZarText.SetActive(value: true);
        }

        if (___playerStats.Slot < first) first = ___playerStats.Slot;
        if (___playerStats.Slot == first) Array.Fill(diceCounts, 0);
        if (___playerStats.Dead) return;
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
        if (diceCounts.Any(i => i > 20))
        {
            first = 4;
        }
    }
}
#endif
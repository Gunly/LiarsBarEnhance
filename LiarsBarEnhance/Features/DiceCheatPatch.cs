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
    public static void UpdateCallPostfix(DiceGamePlay __instance, GameObject ___ZarText, Manager ___manager)
    {
        if (Plugin.BooleanCheatDice.Value && !__instance.isOwned && Plugin.KeyCheatDiceShow.IsPressed() && ___manager.PluginControl())
        {
            ___ZarText.SetActive(value: true);
        }
        if (__instance.isOwned)
        {
            if (Plugin.BooleanCheatDiceTotalDice.Value)
            {
                Array.Fill(diceCounts, 0);
                foreach (var player in ___manager.Players)
                {
                    if (player.Dead) continue;
                    foreach (var diceValue in player.GetComponent<DiceGamePlay>().DiceValues)
                    {
                        if (diceValue != 1 || ___manager.DiceGame.DiceMode == DiceGamePlayManager.dicemode.Basic)
                        {
                            diceCounts[diceValue - 1]++;
                        }
                        else
                        {
                            for (var i = 0; i < diceCounts.Length; i++) diceCounts[i]++;
                        }
                    }
                }
            }

            for (var i = 0; i < __instance.DiceValues.Count; i++)
            {
                if (Plugin.KeyCheatChangeCardDice[4 - i].IsDown() && ___manager.PluginControl())
                {
                    if (__instance.DiceValues[i] < 6) __instance.DiceValues[i]++;
                    else __instance.DiceValues[i] = 1;
                }
            }
        }
    }
}
#endif
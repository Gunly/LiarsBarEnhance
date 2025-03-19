#if CHEATRELEASE
using HarmonyLib;

using System;

namespace LiarsBarEnhance.Features;

[HarmonyPatch]
public class DiceCheatPatch
{
    public static readonly int[] diceCounts = new int[6];

    [HarmonyPatch(typeof(Dice), "Update")]
    [HarmonyPostfix]
    public static void UpdatePostfix(Dice __instance)
    {
        if (Plugin.BooleanCheatDice.Value)
        {
            __instance.renderer.material = Manager.Instance.zar1;
        }
    }

    [HarmonyPatch(typeof(DiceGamePlay), "UpdateCall")]
    [HarmonyPostfix]
    public static void UpdateCallPostfix(DiceGamePlay __instance)
    {
        if (Plugin.BooleanCheatDice.Value && !__instance.isOwned && Plugin.KeyCheatDiceShow.IsPressed() && __instance.manager.PluginControl())
        {
            __instance.ZarText.SetActive(value: true);
        }
        if (__instance.isOwned)
        {
            if (Plugin.BooleanCheatDiceTotalDice.Value)
            {
                Array.Fill(diceCounts, 0);
                foreach (var player in __instance.manager.Players)
                {
                    if (!player || player.Dead) continue;
                    foreach (var diceValue in player.GetComponent<DiceGamePlay>().DiceValues)
                    {
                        if (diceValue != 1 || __instance.manager.DiceGame.DiceMode == DiceGamePlayManager.dicemode.Basic)
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
                if (Plugin.KeyCheatChangeCardDice[4 - i].IsDown() && __instance.manager.PluginControl())
                {
                    if (__instance.DiceValues[i] < 6) __instance.DiceValues[i]++;
                    else __instance.DiceValues[i] = 1;
                }
            }
        }
    }
}
#endif
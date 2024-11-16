#if CHEATRELEASE
using HarmonyLib;
using LiarsBarEnhance.Utils;
using UnityEngine;

namespace LiarsBarEnhance.Features;

[HarmonyPatch]
public class DiceCheatPatch
{
    [HarmonyPatch(typeof(Dice), "Update")]
    [HarmonyPostfix]
    public static void UpdatePostfix(Dice __instance)
    {
        if (Plugin.BooleanCheatDice.Value)
        {
            FastMemberAccessor<Dice, MeshRenderer>.Get(__instance, "renderer").material = Manager.Instance.zar1;
        }
    }

    [HarmonyPatch(typeof(DiceGamePlay), "UpdateCall")]
    [HarmonyPostfix]
    public static void UpdateCallPostfix(DiceGamePlay __instance)
    {
        if (Plugin.BooleanCheatDice.Value && !__instance.isOwned && ShortcutInput.IsPressed(Plugin.KeyCheatDiceShow))
        {
            FastMemberAccessor<DiceGamePlay, GameObject>.Get(__instance, "ZarText").SetActive(value: true);
        }
    }
}
#endif
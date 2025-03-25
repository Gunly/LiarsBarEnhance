using HarmonyLib;

namespace LiarsBarEnhance.Features;

[HarmonyPatch]
public class ShowSelfTopInfoPatch
{
    [HarmonyPatch(typeof(PlayerStats), "Update")]
    [HarmonyPrefix]
    public static bool UpdatePrefix(PlayerStats __instance)
    {
        if (__instance.isOwned)
        {
            __instance.NameText().transform.parent.gameObject.SetActive(!__instance.Winner && Plugin.BooleanCustomShowSelfInfo.Value);
        }
        if (__instance.Health == 0 && __instance.isServer)
        {
            __instance.NetworkDead = true;
        }
        return false;
    }

    [HarmonyPatch(typeof(BlorfGamePlay), "UpdateCall")]
    [HarmonyPostfix]
    public static void UpdateCallPostfix(BlorfGamePlay __instance)
    {
        if (Plugin.BooleanCustomShowSelfInfo.Value)
        {
#if CHEATRELEASE
            __instance.RoundText().text = $"({__instance.Networkcurrentrevoler}|{(Plugin.BooleanCheatDeck.Value && Plugin.BooleanCheatDeckHealth.Value ? __instance.Networkrevolverbulllet + 1 : 6)})";
#else
            __instance.RoundText().text = $"({__instance.Networkcurrentrevoler}|6)";
#endif
            __instance.RoundText().gameObject.SetActive(true);
            if (__instance.playerStats().HaveTurn)
            {
                __instance.KartKafaSprite.SetActive(true);
            }
        }
    }

    [HarmonyPatch(typeof(DiceGamePlay), "UpdateCall")]
    [HarmonyPostfix]
    public static void UpdateCallPostfix(DiceGamePlay __instance)
    {
        if (Plugin.BooleanCustomShowSelfInfo.Value)
        {
            if (__instance.playerStats().HaveTurn)
            {
                __instance.ZarKafaSprite().SetActive(true);
            }
        }
    }

    [HarmonyPatch(typeof(ChaosGamePlay), "UpdateCall")]
    [HarmonyPostfix]
    public static void UpdateCallPostfix(ChaosGamePlay __instance)
    {
        if (Plugin.BooleanCustomShowSelfInfo.Value)
        {
#if CHEATRELEASE
            __instance.RoundText().text = $"({__instance.Networkcurrentrevoler}|{(Plugin.BooleanCheatDeck.Value && Plugin.BooleanCheatDeckHealth.Value ? __instance.Networkrevolverbulllet + 1 : 6)})";
#else
            __instance.RoundText().text = $"({__instance.Networkcurrentrevoler}|6)";
#endif
            __instance.RoundText().gameObject.SetActive(true);
            if (__instance.playerStats().HaveTurn)
            {
                __instance.KartKafaSprite.SetActive(true);
            }
        }
    }

    [HarmonyPatch(typeof(BlorfGamePlayMatchMaking), "UpdateCall")]
    [HarmonyPostfix]
    public static void UpdateCallPostfix(BlorfGamePlayMatchMaking __instance)
    {
        if (Plugin.BooleanCustomShowSelfInfo.Value)
        {
#if CHEATRELEASE
            __instance.RoundText().text = $"({__instance.Networkcurrentrevoler}|{(Plugin.BooleanCheatDeck.Value && Plugin.BooleanCheatDeckHealth.Value ? __instance.Networkrevolverbulllet + 1 : 6)})";
#else
            __instance.RoundText().text = $"({__instance.Networkcurrentrevoler}|6)";
#endif
            __instance.RoundText().gameObject.SetActive(true);
            if (__instance.playerStats().HaveTurn)
            {
                __instance.KartKafaSprite.SetActive(true);
            }
        }
    }
}
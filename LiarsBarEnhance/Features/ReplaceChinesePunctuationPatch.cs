using HarmonyLib;

using LiarsBarEnhance.Utils;

using TMPro;
using UnityEngine.Localization.Settings;

namespace LiarsBarEnhance.Features;

[HarmonyPatch]
public class ReplaceChinesePunctuationPatch
{
    [HarmonyPatch(typeof(LobbyController), "UserCode_ShowLoadingScreen")]
    [HarmonyPostfix]
    public static void TipTextPostfix(LobbyController __instance, TextMeshProUGUI ___TipText)
    {
        if (__instance.Mode == CustomNetworkManager.GameMode.LiarsDeck)
        {
            if (LocalizationSettings.SelectedLocale.LocaleName == "Chinese (Simplified) (zh-Hans)")
            {
                ___TipText.text = "骗子牌 在这个游戏中, 你最好的朋友用谎言赌上了你的性命, 而你却依然面带微笑";
            }
        }
        else if (__instance.Mode == CustomNetworkManager.GameMode.LiarsDice)
        {
            if (LocalizationSettings.SelectedLocale.LocaleName == "Chinese (Simplified) (zh-Hans)")
            {
                ___TipText.text = "说谎者的骰子: 在这个游戏中, 你会发现总是帮你移动的朋友也是刚刚骗你喝下自己毒药的人";
            }
        }
    }
}
using HarmonyLib;

using TMPro;

using UnityEngine.Localization.Settings;

namespace LiarsBarEnhance.Features;

[HarmonyPatch]
public class ChineseNameFixPatch
{
    [HarmonyPatch(typeof(FontChanger), nameof(FontChanger.Update))]
    [HarmonyPostfix]
    public static void UpdatePostfix(FontChanger __instance)
    {
        __instance.GetComponent<TMP_Text>().font = __instance.fonts.getCurrentFont(__instance.deffaultfont);
    }

    [HarmonyPatch(typeof(BlorfGamePlayManager), nameof(BlorfGamePlayManager.Start))]
    [HarmonyPostfix]
    public static void BlorfGamePlayManagerStartPostfix(BlorfGamePlayManager __instance)
    {
        __instance.LastBidName1.gameObject.AddComponentIfNotExist<FontChanger>();
        __instance.LastbidName2.gameObject.AddComponentIfNotExist<FontChanger>();
    }

    [HarmonyPatch(typeof(DiceGamePlayManager), nameof(DiceGamePlayManager.Start))]
    [HarmonyPostfix]
    public static void DiceGamePlayManagerStartPostfix(DiceGamePlayManager __instance)
    {
        __instance.TurnNameText.gameObject.AddComponentIfNotExist<FontChanger>();
        __instance.CalledLiarText.gameObject.AddComponentIfNotExist<FontChanger>();
        __instance.ShowsText.gameObject.AddComponentIfNotExist<FontChanger>();
        __instance.LoserName.gameObject.AddComponentIfNotExist<FontChanger>();
    }

    [HarmonyPatch(typeof(ChaosGamePlayManager), nameof(ChaosGamePlayManager.Start))]
    [HarmonyPostfix]
    public static void ChaosGamePlayManagerStartPostfix(ChaosGamePlayManager __instance)
    {
        __instance.LastBidName1.gameObject.AddComponentIfNotExist<FontChanger>();
        __instance.LastbidName2.gameObject.AddComponentIfNotExist<FontChanger>();
    }

    [HarmonyPatch(typeof(VoiceChatPrefab), nameof(VoiceChatPrefab.Update))]
    [HarmonyPostfix]
    public static void VoiceChatPrefabUpdatePostfix(VoiceChatPrefab __instance)
    {
        __instance.NameText.gameObject.AddComponentIfNotExist<FontChanger>();
    }

    [HarmonyPatch(typeof(LobbyController), nameof(LobbyController.UserCode_ShowLoadingScreen))]
    [HarmonyPostfix]
    public static void TipTextPostfix(LobbyController __instance)
    {
        if (__instance.Mode == CustomNetworkManager.GameMode.LiarsDeck)
        {
            if (LocalizationSettings.SelectedLocale.LocaleName == "Chinese (Simplified) (zh-Hans)")
            {
                __instance.TipText.text = "骗子牌: 在这个游戏中, 你最好的朋友用谎言赌上了你的性命, 而你却依然面带微笑";
            }
        }
        else if (__instance.Mode == CustomNetworkManager.GameMode.LiarsDice)
        {
            if (LocalizationSettings.SelectedLocale.LocaleName == "Chinese (Simplified) (zh-Hans)")
            {
                __instance.TipText.text = "说谎者的骰子: 在这个游戏中, 你会发现总是帮你移动的朋友也是刚刚骗你喝下自己毒药的人";
            }
        }
        else if (__instance.Mode == CustomNetworkManager.GameMode.LiarsChaos)
        {
            if (LocalizationSettings.SelectedLocale.LocaleName == "Chinese (Simplified) (zh-Hans)")
            {
                __instance.TipText.text = "最终, 每个人都是骗子——只是有些人玩得更好";
            }
        }
    }
}
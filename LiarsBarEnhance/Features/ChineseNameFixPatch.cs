using HarmonyLib;

using TMPro;

namespace LiarsBarEnhance.Features;

[HarmonyPatch]
public class ChineseNameFixPatch
{
    [HarmonyPatch(typeof(FontChanger), "Update")]
    [HarmonyPostfix]
    public static void UpdatePostfix(FontChanger __instance, Fonts ___fonts)
    {
        __instance.GetComponent<TMP_Text>().font = ___fonts.getCurrentFont(__instance.deffaultfont);
    }

    [HarmonyPatch(typeof(BlorfGamePlayManager), "Start")]
    [HarmonyPostfix]
    public static void BlorfGamePlayManagerStartPostfix(BlorfGamePlayManager __instance)
    {
        __instance.LastBidName1.gameObject.AddComponentIfNotExist<FontChanger>();
        __instance.LastbidName2.gameObject.AddComponentIfNotExist<FontChanger>();
    }

    [HarmonyPatch(typeof(DiceGamePlayManager), "Start")]
    [HarmonyPostfix]
    public static void DiceGamePlayManagerStartPostfix(DiceGamePlayManager __instance)
    {
        __instance.TurnNameText.gameObject.AddComponentIfNotExist<FontChanger>();
        __instance.CalledLiarText.gameObject.AddComponentIfNotExist<FontChanger>();
        __instance.ShowsText.gameObject.AddComponentIfNotExist<FontChanger>();
        __instance.LoserName.gameObject.AddComponentIfNotExist<FontChanger>();
    }

    [HarmonyPatch(typeof(VoiceChatPrefab), "Start")]
    [HarmonyPostfix]
    public static void VoiceChatPrefabStartPostfix(VoiceChatPrefab __instance)
    {
        __instance.NameText.gameObject.AddComponentIfNotExist<FontChanger>();
    }
}
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
    }

    [HarmonyPatch(typeof(VoiceChatPrefab), "Start")]
    [HarmonyPostfix]
    public static void VoiceChatPrefabStartPostfix(VoiceChatPrefab __instance)
    {
        __instance.NameText.gameObject.AddComponentIfNotExist<FontChanger>();
    }
}
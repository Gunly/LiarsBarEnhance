using HarmonyLib;

using TMPro;

namespace LiarsBarEnhance.Features;

[HarmonyPatch]
public class RemoveNameLengthLimitPatch
{
    [HarmonyPatch(typeof(PlayerStats), "Start")]
    [HarmonyPostfix]
    public static void StartPostfix(PlayerStats __instance, TextMeshPro ___NameText)
    {
        ___NameText.text = $"<sprite={__instance.GetComponent<CharController>().level}>{__instance.PlayerName}";
    }
}
using HarmonyLib;

namespace LiarsBarEnhance.Features;

[HarmonyPatch]
public class RemoveNameLengthLimitPatch
{
    [HarmonyPatch(typeof(PlayerStats), nameof(PlayerStats.Update))]
    [HarmonyPostfix]
    public static void UpdatePostfix(PlayerStats __instance)
    {
        __instance.NameText.text = $"<sprite={__instance.GetComponent<CharController>().level}>{__instance.PlayerName}";
    }
}
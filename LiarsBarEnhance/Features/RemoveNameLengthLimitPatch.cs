using HarmonyLib;
using TMPro;

namespace LiarsBarEnhance.Features;

[HarmonyPatch]
public class RemoveNameLengthLimitPatch
{
    [HarmonyPatch(typeof(PlayerStats), "Update")]
    [HarmonyPrefix]
    public static bool UpdatePrefix(PlayerStats __instance, TextMeshPro ___NameText)
    {
        ___NameText.text = $"<sprite={__instance.GetComponent<CharController>().level}>{__instance.PlayerName}";
        ___NameText.transform.parent.gameObject.SetActive(Plugin.BooleanCustomShowSelfInfo.Value);
        if (__instance.Health == 0 && __instance.isServer)
        {
            __instance.NetworkDead = true;
        }
        return false;
    }
}
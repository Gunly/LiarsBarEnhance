using HarmonyLib;

using UnityEngine;

namespace LiarsBarEnhance.Features;

[HarmonyPatch]
public class ChatProPatch
{
    [HarmonyPatch(typeof(ChatNetwork), "Send")]
    [HarmonyPrefix]
    public static bool SendPrefix(ChatNetwork __instance)
    {
        if (!Input.GetKeyDown(KeyCode.Return))
            return false;
        var message = $"<color=#FDE2AA>[{__instance.GetComponent<PlayerObjectController>().PlayerName}]</color>:>{__instance.inputField().text}";
        __instance.SendMessage(message);
        __instance.inputField().text = string.Empty;
        return false;
    }

    [HarmonyPatch(typeof(ChatNetwork), "Sansur")]
    [HarmonyPrefix]
    public static bool SansurPrefix(ref string __result, string message)
    {
        __result = message;
        return false;
    }
}
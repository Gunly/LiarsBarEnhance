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
        if (Input.GetKeyDown(KeyCode.Return) && !string.IsNullOrWhiteSpace(__instance.inputField().text))
        {
            var text = __instance.inputField().text;
            string senderName;
            if (text.StartsWith("/r") && text.Contains(':'))
            {
                senderName = text[2..text.IndexOf(":")];
                text = text[(text.IndexOf(":") + 1)..];
            }
            else
            {
                senderName = __instance.GetComponent<PlayerObjectController>().PlayerName;
            }

            var message = $"<color=#FDE2AA>[{senderName}]</color>:{text}";
            __instance.CallMethod("CmdSendMessage", [typeof(string)], [message]);
            __instance.inputField().text = string.Empty;
        }
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
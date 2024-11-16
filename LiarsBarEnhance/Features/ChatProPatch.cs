using HarmonyLib;

using LiarsBarEnhance.Utils;

using UnityEngine;
using UnityEngine.UI;

namespace LiarsBarEnhance.Features;

[HarmonyPatch]
public class ChatProPatch
{
    [HarmonyPatch(typeof(ChatNetwork), nameof(ChatNetwork.Send))]
    [HarmonyPrefix]
    public static void SendPrefix(ref bool __runOriginal, ChatNetwork __instance, InputField ___inputField)
    {
        if (!Input.GetKeyDown(KeyCode.Return))
            return;

        __runOriginal = false;
        var text = FastMemberAccessor<ChatNetwork, InputField>.Get(__instance, "inputField").text;
        string senderName;
        if (text.StartsWith("/r"))
        {
            senderName = text[2..text.IndexOf(":")];
            text = text[(text.IndexOf(":") + 1)..];
        }
        else
        {
            senderName = __instance.GetComponent<PlayerObjectController>().PlayerName;
        }

        var message = $"<color=#{Plugin.StringCustomNameColor.Value}>[{senderName}]</color>:<color=#{Plugin.StringCustomMessageColor.Value}>{text}</color>";
        AccessTools.Method(typeof(ChatNetwork), "CmdSendMessage", [typeof(string)]).Invoke(__instance, [message]);
        ___inputField.text = string.Empty;
    }

    [HarmonyPatch(typeof(ChatNetwork), nameof(ChatNetwork.Sansur))]
    [HarmonyPrefix]
    public static void SansurPrefix(ref bool __runOriginal, ref string __result, string message)
    {
        __runOriginal = false;
        __result = message;
    }
}
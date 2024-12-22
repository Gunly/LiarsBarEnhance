using HarmonyLib;

using UnityEngine;
using UnityEngine.UI;

namespace LiarsBarEnhance.Features;

[HarmonyPatch]
public class ChatProPatch
{
    [HarmonyPatch(typeof(ChatNetwork), nameof(ChatNetwork.Send))]
    [HarmonyPrefix]
    public static bool SendPrefix(ChatNetwork __instance, InputField ___inputField)
    {
        if (!Input.GetKeyDown(KeyCode.Return))
            return false;
        var message = $"<color=#FDE2AA>[{__instance.GetComponent<PlayerObjectController>().PlayerName}]</color>:>{___inputField.text}";
        AccessTools.Method(typeof(ChatNetwork), "CmdSendMessage", [typeof(string)]).Invoke(__instance, [message]);
        ___inputField.text = string.Empty;
        return false;
    }

    [HarmonyPatch(typeof(ChatNetwork), nameof(ChatNetwork.Sansur))]
    [HarmonyPrefix]
    public static bool SansurPrefix(ref string __result, string message)
    {
        __result = message;
        return false;
    }
}
﻿using HarmonyLib;

namespace LiarsBarEnhance.Features;

[HarmonyPatch]
public class CustomNamePatch
{
    [HarmonyPatch(typeof(PlayerObjectController), nameof(PlayerObjectController.OnStartAuthority))]
    [HarmonyPrefix]
    public static bool OnStartAuthorityPrefix(PlayerObjectController __instance)
    {
        if (!__instance.isOwned) return true;
        if (!Plugin.StringCustomName.Equals(""))
        {
            __instance.NetworkPlayerName = Plugin.StringCustomName.Value;
            __instance.name = "LocalGamePlayer";
            AccessTools.Method("PlayerObjectController:CmdSetPlayerName", [typeof(string)]).Invoke(__instance, [Plugin.StringCustomName.Value]);
            LobbyController.Instance.FindLocalPlayer();
            LobbyController.Instance.UpdateLobbyName();
            return false;
        }
        return true;
    }

    [HarmonyPatch(typeof(PlayerStats), "Start")]
    [HarmonyPrefix]
    public static void StartPrefix(PlayerStats __instance)
    {
        if (!__instance.isOwned) return;
        Plugin.StringCustomName.SettingChanged += (sender, args) =>
        {
            __instance.NetworkPlayerName = Plugin.StringCustomName.Value;
        };
    }
}
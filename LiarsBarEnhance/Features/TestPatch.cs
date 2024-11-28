using HarmonyLib;

using Mirror;

using UnityEngine;

namespace LiarsBarEnhance.Features;

[HarmonyPatch]
public class TestPatch
{
    [HarmonyPatch(typeof(CharController), nameof(CharController.Start))]
    [HarmonyPostfix]
    public static void StartPostfix(CharController __instance)
    {
        if (Plugin.BooleanTestGiraffe.Value)
        {
            var ntrs = __instance.gameObject.GetComponents<NetworkTransformReliable>();
            foreach (var ntr in ntrs)
            {
                ntr.syncPosition = true;
                ntr.syncScale = true;
            }
        }
    }

    [HarmonyPatch(typeof(LobbyController), nameof(LobbyController.ReturnMenu))]
    [HarmonyPrefix]
    public static void StartPrefix(LobbyController __instance, CustomNetworkManager ___managerr)
    {
        if (Input.GetKey(KeyCode.LeftShift))
        {
            ___managerr.StopClient();
        }
    }
}
using HarmonyLib;

using UnityEngine;

namespace LiarsBarEnhance.Features;

[HarmonyPatch]
public class NoCooldownPatch
{
    [HarmonyPatch(typeof(CharController), "Update")]
    [HarmonyPostfix]
    public static void UpdateCallPostfix(CharController __instance)
    {
        if (Plugin.BooleanGameNoCooldown.Value && __instance.isOwned)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                __instance.CallMethod("PlayEmote1Sfx", [typeof(int)], [1]);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                __instance.CallMethod("PlayEmote1Sfx", [typeof(int)], [2]);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                __instance.CallMethod("PlayEmote1Sfx", [typeof(int)], [3]);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha4))
            {
                __instance.CallMethod("PlayEmote2Sfx");
            }
            else if (Input.GetKeyDown(KeyCode.Alpha5))
            {
                __instance.CallMethod("PlayEmote3Sfx");
            }
        }
    }
}
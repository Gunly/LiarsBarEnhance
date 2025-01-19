using HarmonyLib;

namespace LiarsBarEnhance.Features;

[HarmonyPatch]
public class AutoReadyPatch
{
    [HarmonyPatch(typeof(PlayerObjectController), nameof(PlayerObjectController.Start))]
    [HarmonyPostfix]
    public static void StartPostfix(PlayerObjectController __instance)
    {
        if (__instance.isOwned && Plugin.BooleanGameAutoReady.Value)
        {
            __instance.SetReady(true);
        }
    }

    [HarmonyPatch(typeof(PlayerObjectController), nameof(PlayerObjectController.Update))]
    [HarmonyPostfix]
    public static void UpdatePostfix(PlayerObjectController __instance)
    {
        if (__instance.isOwned && Plugin.BooleanGameAutoReady.Value && !__instance.Loaded && !__instance.NetworkReady)
        {
            __instance.SetReady(true);
        }
    }
}
using HarmonyLib;

namespace LiarsBarEnhance.Features;

[HarmonyPatch]
public class DefaultSkinPatch
{
    [HarmonyPatch(typeof(PlayerObjectController), "Start")]
    [HarmonyPostfix]
    public static void StartPostfix(PlayerObjectController __instance)
    {
        if (__instance.isOwned && Plugin.DefaultSkin.Value != 0)
        {
            if (__instance.isServer)
            {
                __instance.NetworkPlayerSkin = (int)Plugin.DefaultSkin.Value;
            }
            else
            {
                for (var i = 0; i < (int)Plugin.DefaultSkin.Value; i++)
                {
                    __instance.SetSkin();
                }
            }
        }
    }
}
using HarmonyLib;

using Mirror;

namespace LiarsBarEnhance.Features;

[HarmonyPatch]
public class GiraffePatch
{
    [HarmonyPatch(typeof(CharController), "Start")]
    [HarmonyPostfix]
    public static void StartPostfix(CharController __instance)
    {
        if (Plugin.BooleanTestGiraffe.Value)
        {
            var ntrs = __instance.GetComponents<NetworkTransformReliable>();
            foreach (var ntr in ntrs)
            {
                ntr.syncPosition = true;
                ntr.syncScale = true;
            }
        }
    }
}
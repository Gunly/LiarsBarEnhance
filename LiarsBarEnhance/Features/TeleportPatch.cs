using HarmonyLib;

using UnityEngine;

namespace LiarsBarEnhance.Features;

[HarmonyPatch]
public class TeleportPatch
{
    [HarmonyPatch(typeof(CharController), nameof(CharController.Update))]
    [HarmonyPostfix]
    public static void UpdatePostfix(CharController __instance)
    {
        if (!__instance.isOwned) return;
        for (var i = 0; i < Plugin.InitPositionNumValue; i++)
        {
            if (Plugin.KeyPosition[i].IsDown() && __instance.manager.PluginControl())
            {
                __instance.transform.localPosition = new Vector3(Plugin.VectorPosition[i][0].Value, Plugin.VectorPosition[i][1].Value, Plugin.VectorPosition[i][2].Value);
                __instance.transform.localRotation = new Vector3(Plugin.VectorRotation[i][0].Value, Plugin.VectorRotation[i][1].Value, Plugin.VectorRotation[i][2].Value).ToQuaternion();
                break;
            }
        }
    }
}
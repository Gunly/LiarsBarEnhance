using Cinemachine;

using HarmonyLib;

using UnityEngine;

namespace LiarsBarEnhance.Features;

[HarmonyPatch]
public class FOVPatch
{
    private static CinemachineVirtualCamera cam;
    public static float Fov = 60f;

    [HarmonyPatch(typeof(CharController), nameof(CharController.Start))]
    [HarmonyPostfix]
    public static void StartPostfix(CharController __instance)
    {
        if (!__instance.isOwned) return;
        string[] cameraPaths = ["Base HumanHead/Virtual Camera", "RHINO_Head/Virtual Camera", "YAKUZA_Head/Virtual Camera"];
        foreach (var cameraPath in cameraPaths)
        {
            cam = __instance.HeadPivot.Find(cameraPath)?.GetComponent<CinemachineVirtualCamera>();
            if (cam == null) continue;
            cam.m_Lens.FieldOfView = Fov = Plugin.FloatViewField.Value;
            break;
        }
    }

    [HarmonyPatch(typeof(CharController), nameof(CharController.Update))]
    [HarmonyPostfix]
    public static void UpdatePostfix(CharController __instance, PlayerStats ___playerStats, Manager ___manager)
    {
        if (!__instance.isOwned || cam == null) return;
        if (Plugin.BooleanViewField.Value && ___manager.PluginControl())
        {
            var mouseScroll = Input.GetAxis("Mouse ScrollWheel");
            if (mouseScroll != 0f)
            {
                Plugin.FloatViewField.Value -= 50f * mouseScroll;
            }
        }
        var d = Plugin.FloatViewField.Value - Fov;
        if (d == 0f) return;
        if (Mathf.Abs(d) < 0.1f)
        {
            Fov = Plugin.FloatViewField.Value;
        }
        else if (d > 0f)
        {
            Fov += Mathf.Max(0.1f, d / 5);
        }
        else
        {
            Fov += Mathf.Min(-0.1f, d / 5);
        }
        if (!___playerStats.Dead)
        {
            cam.m_Lens.FieldOfView = Fov;
        }
        else
        {
            for (var i = 0; i < ___manager.SpectatorCameraParrent.transform.childCount; i++)
            {
                ___manager.SpectatorCameraParrent.transform.GetChild(i).gameObject.GetComponent<CinemachineVirtualCamera>().m_Lens.FieldOfView = Fov;
            }
        }
    }
}
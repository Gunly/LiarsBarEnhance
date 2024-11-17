using Cinemachine;

using HarmonyLib;

using System;

using UnityEngine;

namespace LiarsBarEnhance.Features;

[HarmonyPatch]
public class FOVPatch
{
    private static CinemachineVirtualCamera cam;
    private static float fov = 60f;

    [HarmonyPatch(typeof(CharController), nameof(CharController.Start))]
    [HarmonyPostfix]
    public static void StartPostfix(CharController __instance)
    {
        if (!__instance.isOwned) return;
        try
        {
            cam = __instance.HeadPivot.Find("Base HumanHead/Virtual Camera").GetComponent<CinemachineVirtualCamera>();
        }
        catch (Exception)
        {
            cam = __instance.HeadPivot.Find("RHINO_Head/Virtual Camera").GetComponent<CinemachineVirtualCamera>();
        }
        fov = cam.m_Lens.FieldOfView;
        cam.m_Lens.FieldOfView = Plugin.FloatViewField.Value;
    }

    [HarmonyPatch(typeof(CharController), nameof(CharController.Update))]
    [HarmonyPostfix]
    public static void UpdatePostfix(CharController __instance)
    {
        if (!__instance.isOwned) return;
        if (Plugin.BooleanViewMouseViewField.Value)
        {
            var mouseScroll = Input.GetAxis("Mouse ScrollWheel");
            if (mouseScroll != 0f)
            {
                Plugin.FloatViewField.Value += -50f * mouseScroll;
            }
        }
        var d = Plugin.FloatViewField.Value - fov;
        if (d == 0f) return;
        if (Mathf.Abs(d) < 0.02f)
        {
            fov = Plugin.FloatViewField.Value;
        }
        else if (d > 0f)
        {
            fov += Mathf.Max(0.1f, d / 5);
        }
        else
        {
            fov += Mathf.Min(-0.1f, d / 5);
        }
        if (!CharMoveablePatch.PlayerStats.Dead)
        {
            cam.m_Lens.FieldOfView = fov;
        }
        else
        {
            for (var i = 0; i < CharMoveablePatch.Manager.SpectatorCameraParrent.transform.childCount; i++)
            {
                CharMoveablePatch.Manager.SpectatorCameraParrent.transform.GetChild(i).gameObject.GetComponent<CinemachineVirtualCamera>().m_Lens.FieldOfView = fov;
            }
        }
    }
}
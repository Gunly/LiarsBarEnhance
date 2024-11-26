using Cinemachine;

using HarmonyLib;

using System;

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
        try
        {
            cam = __instance.HeadPivot.Find("Base HumanHead/Virtual Camera").GetComponent<CinemachineVirtualCamera>();
        }
        catch (Exception)
        {
            cam = __instance.HeadPivot.Find("RHINO_Head/Virtual Camera").GetComponent<CinemachineVirtualCamera>();
        }
        Fov = cam.m_Lens.FieldOfView;
        cam.m_Lens.FieldOfView = Plugin.FloatViewField.Value;
    }

    [HarmonyPatch(typeof(CharController), nameof(CharController.Update))]
    [HarmonyPostfix]
    public static void UpdatePostfix(CharController __instance, PlayerStats ___playerStats, Manager ___manager)
    {
        if (!__instance.isOwned) return;
        if (Plugin.BooleanViewField.Value && !___manager.GamePaused && !___manager.Chatting)
        {
            var mouseScroll = Input.GetAxis("Mouse ScrollWheel");
            if (mouseScroll != 0f)
            {
                Plugin.FloatViewField.Value -= 50f * mouseScroll;
            }
        }
        var d = Plugin.FloatViewField.Value - Fov;
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
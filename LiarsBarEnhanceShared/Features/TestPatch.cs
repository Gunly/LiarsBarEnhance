using HarmonyLib;

using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace LiarsBarEnhance.Features;

[HarmonyPatch]
public class TestPatch
{
    [HarmonyPatch(typeof(LobbySlot), "SetGameLayerRecursive")]
    [HarmonyPrefix]
    public static bool SetGameLayerRecursivePrefix(GameObject _go)
    {
        return _go != null;
    }

    [HarmonyPatch(typeof(UIAlwaysSelect), "Update")]
    [HarmonyPrefix]
    public static bool UpdatePrefix(EventSystem ___currentEventSystem, ref GameObject ___currentlySelected)
    {
        if (___currentEventSystem.currentSelectedGameObject != null && ___currentlySelected != ___currentEventSystem.currentSelectedGameObject)
        {
            ___currentlySelected = ___currentEventSystem.currentSelectedGameObject;
        }

        if (___currentEventSystem.currentSelectedGameObject == null)
        {
            if (___currentlySelected == null)
            {
                ___currentlySelected = ___currentEventSystem.firstSelectedGameObject;
            }
            ___currentlySelected?.GetComponent<Selectable>().Select();
        }
        return false;
    }
}
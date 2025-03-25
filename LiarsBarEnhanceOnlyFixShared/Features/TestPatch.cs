using HarmonyLib;

using UnityEngine;
//using UnityEngine.UI;

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

    //[HarmonyPatch(typeof(UIAlwaysSelect), "Update")]
    //[HarmonyPrefix]
    //public static bool UpdatePrefix(UIAlwaysSelect __instance)
    //{
    //    if (__instance.currentEventSystem.currentSelectedGameObject != null && __instance.currentlySelected != __instance.currentEventSystem.currentSelectedGameObject)
    //    {
    //        __instance.currentlySelected = __instance.currentEventSystem.currentSelectedGameObject;
    //    }

    //    if (__instance.currentEventSystem.currentSelectedGameObject == null)
    //    {
    //        if (__instance.currentlySelected == null)
    //        {
    //            __instance.currentlySelected = __instance.currentEventSystem.firstSelectedGameObject;
    //        }
    //        __instance.currentlySelected?.GetComponent<Selectable>().Select();
    //    }
    //    return false;
    //}
}
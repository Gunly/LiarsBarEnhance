using HarmonyLib;

using Mirror;

using System.Linq;

using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace LiarsBarEnhance.Features;

[HarmonyPatch]
public class TestPatch
{
    [HarmonyPatch(typeof(CharController), nameof(CharController.Start))]
    [HarmonyPostfix]
    public static void StartPostfix(CharController __instance)
    {
        if (Plugin.BooleanTestGiraffe.Value)
        {
            var ntrs = __instance.gameObject.GetComponents<NetworkTransformReliable>();
            foreach (var ntr in ntrs)
            {
                ntr.syncPosition = true;
                ntr.syncScale = true;
            }
        }
    }

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

    private static readonly string[] allowResetSettingsState = ["state_settings", "substate_display", "substate_gameplay", "substate_graphics", "substate_audiolanguages"];
    [HarmonyPatch(typeof(ChangeSettings), "Update")]
    [HarmonyPrefix]
    public static bool UpdatePrefix(ChangeSettings __instance)
    {
        if (Input.GetKeyDown(KeyCode.F7) && SceneManager.GetActiveScene().name == "SteamTest")
        {
            var stateManager = __instance.gameObject.GetComponent<StateManager>();
            var name = stateManager.ActiveCanvas.gameObject.name;
            if (allowResetSettingsState.Contains(name))
            {
                AccessTools.Method("ChangeSettings:ResetSettings").Invoke(__instance, []);
            }
        }
        return false;
    }
}
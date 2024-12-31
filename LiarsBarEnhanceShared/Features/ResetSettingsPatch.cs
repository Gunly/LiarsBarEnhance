using HarmonyLib;

using System.Linq;

using UnityEngine;
using UnityEngine.SceneManagement;

namespace LiarsBarEnhance.Features;

[HarmonyPatch]
public class ResetSettingsPatch
{
    private static readonly string[] allowResetSettingsState = ["state_settings", "substate_display", "substate_gameplay", "substate_graphics", "substate_audiolanguages"];
    [HarmonyPatch(typeof(ChangeSettings), nameof(ChangeSettings.Update))]
    [HarmonyPrefix]
    public static bool UpdatePrefix(ChangeSettings __instance)
    {
        if (Input.GetKeyDown(KeyCode.F7) && SceneManager.GetActiveScene().name == "SteamTest")
        {
            var stateManager = __instance.gameObject.GetComponent<StateManager>();
            var name = stateManager.ActiveCanvas.gameObject.name;
            if (allowResetSettingsState.Contains(name))
            {
                __instance.ResetSettings();
            }
        }
        return false;
    }
}
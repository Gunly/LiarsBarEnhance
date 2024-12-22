using BepInEx;
using BepInEx.Logging;

using HarmonyLib;

using LiarsBarEnhance.Features;

using UnityEngine;
using UnityEngine.SceneManagement;

namespace LiarsBarEnhanceOnlyFix;

[BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
public class Plugin : BaseUnityPlugin
{
    internal new static ManualLogSource Logger;
    private void Awake()
    {
        Logger = base.Logger;

        Harmony.CreateAndPatchAll(typeof(ChatProPatch), nameof(ChatProPatch));
        Harmony.CreateAndPatchAll(typeof(ChineseNameFixPatch), nameof(ChineseNameFixPatch));
        Harmony.CreateAndPatchAll(typeof(DisableTmpWarningsPatch), nameof(DisableTmpWarningsPatch));
        Harmony.CreateAndPatchAll(typeof(RemoveNameLengthLimitPatch), nameof(RemoveNameLengthLimitPatch));
        Harmony.CreateAndPatchAll(typeof(ResetSettingsPatch), nameof(ResetSettingsPatch));
        Harmony.CreateAndPatchAll(typeof(ScrollViewPatch), nameof(ScrollViewPatch));
        Harmony.CreateAndPatchAll(typeof(TestPatch), nameof(TestPatch));
        Harmony.CreateAndPatchAll(typeof(LobbyFilterPatch), nameof(LobbyFilterPatch));

        var StringGameLobbyFilterWords = Config.Bind("Game", "LobbyFilterWords", "透视|改牌|透牌|修改|枪数|无敌|低价|稳定|免费|加q|加群|售后|看片|网址|国产|少妇|" +
            "@[Qq]\\d{5,}|@[a-zA-Z0-9]{3,}\\.[a-zA-Z]{2,}", "大厅过滤词");
        LobbyFilterPatch.FilterWords = () => StringGameLobbyFilterWords.Value;
        StringGameLobbyFilterWords.SettingChanged += (_, _) => LobbyFilterPatch.FilterWordsChanged();

        Logger.LogInfo($"Plugin {PluginInfo.PLUGIN_GUID} is loaded!");
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F9)) SceneManager.LoadScene("SteamTest");
    }
}
﻿using BepInEx;
using BepInEx.Logging;
using BepInEx.Unity.IL2CPP;

using HarmonyLib;

using LiarsBarEnhance.Features;

using UnityEngine;
using UnityEngine.SceneManagement;

namespace LiarsBarEnhanceOnlyFix;

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
public class Plugin : BasePlugin
{
    internal new static ManualLogSource Log;

    public override void Load()
    {
        LiarsBarEnhanceBehaviour.Plugin = this;
        IL2CPPChainloader.AddUnityComponent(typeof(LiarsBarEnhanceBehaviour));
    }

    private class LiarsBarEnhanceBehaviour : MonoBehaviour
    {
        internal static Plugin Plugin;

        private void Start() => Plugin.Start();
        private void Update() => Plugin.Update();
        //private void LateUpdate() => Plugin.LateUpdate();
        //private void OnGUI() => Plugin.OnGUI();
    }

    public void Start()
    {
        Harmony.CreateAndPatchAll(typeof(ChatProPatch), nameof(ChatProPatch));
        Harmony.CreateAndPatchAll(typeof(ChineseNameFixPatch), nameof(ChineseNameFixPatch));
        Harmony.CreateAndPatchAll(typeof(DisableTmpWarningsPatch), nameof(DisableTmpWarningsPatch));
        Harmony.CreateAndPatchAll(typeof(LobbyFilterPatch), nameof(LobbyFilterPatch));
        Harmony.CreateAndPatchAll(typeof(RemoveNameLengthLimitPatch), nameof(RemoveNameLengthLimitPatch));
        Harmony.CreateAndPatchAll(typeof(ResetSettingsPatch), nameof(ResetSettingsPatch));
        Harmony.CreateAndPatchAll(typeof(ScrollViewPatch), nameof(ScrollViewPatch));
        Harmony.CreateAndPatchAll(typeof(TestPatch), nameof(TestPatch));

        var StringGameLobbyFilterWords = Config.Bind("Game", "LobbyFilterWords", "透视|改牌|透牌|修改|枪数|无敌|低价|稳定|免费|开g|加q|加群|售后|看片|网址|国产|少妇|" +
            "@[Qq]\\d{5,}|@[a-zA-Z0-9]{3,}\\.[a-zA-Z]{2,}", "大厅过滤词");
        LobbyFilterPatch.FilterWords = () => StringGameLobbyFilterWords.Value;
        StringGameLobbyFilterWords.SettingChanged += (_, _) => LobbyFilterPatch.FilterWordsChanged();

        Log = base.Log;
        Log.LogInfo($"Plugin {MyPluginInfo.PLUGIN_GUID} is loaded!");
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F9)) SceneManager.LoadScene("SteamTest");
    }
}
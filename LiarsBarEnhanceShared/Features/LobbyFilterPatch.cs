using HarmonyLib;

using System.Collections.Generic;
using System.Text.RegularExpressions;

using UnityEngine;

namespace LiarsBarEnhance.Features;

[HarmonyPatch]
public class LobbyFilterPatch
{
    private static readonly List<string> filterList = [];

    public static System.Func<string> FilterWords;

    [HarmonyPatch(typeof(LobbyListManager), nameof(LobbyListManager.Awake))]
    [HarmonyPostfix]
    public static void AwakePostfix()
    {
        getFilterWords();
    }

    public static void FilterWordsChanged()
    {
        getFilterWords();
    }

    private static void getFilterWords()
    {
        filterList.Clear();
        if (FilterWords != null && FilterWords().Length > 0)
        {
            foreach (var word in FilterWords().Split('|'))
            {
                filterList.Add(word);
            }
        }
    }

    [HarmonyPatch(typeof(LobbyListManager), nameof(LobbyListManager.DisplayLobbies))]
    [HarmonyPostfix]
    public static void DisplayLobbiesPostfix(LobbyListManager __instance)
    {
        if (filterList.Count == 0) return;
        List<GameObject> lobbies = [];
        foreach (var listOfLobby in __instance.listOfLobbies)
        {
            var lobbyDataEntry = listOfLobby.GetComponent<LobbyDataEntry>();
            foreach (var word in filterList)
            {
                if (word.StartsWith("@"))
                {
                    if (Regex.IsMatch(lobbyDataEntry.LobbyName, word[1..]))
                    {
                        lobbies.Add(listOfLobby);
                    }
                }
                else if (lobbyDataEntry.LobbyName.Contains(word))
                {
                    lobbies.Add(listOfLobby);
                }
            }
        }
        foreach (var lobby in lobbies)
        {
            __instance.listOfLobbies.Remove(lobby);
            Object.Destroy(lobby.gameObject);
        }
    }
}
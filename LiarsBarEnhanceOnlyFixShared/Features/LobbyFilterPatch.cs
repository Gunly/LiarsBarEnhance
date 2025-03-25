using HarmonyLib;

using System.Collections.Generic;
using System.Text.RegularExpressions;

using UnityEngine;

namespace LiarsBarEnhance.Features;

[HarmonyPatch]
public class LobbyFilterPatch
{
    private static readonly List<string> filterList = [];

    public static void GetFilterWords()
    {
        filterList.Clear();
        if (Plugin.StringGameLobbyFilterWords.Value != null && Plugin.StringGameLobbyFilterWords.Value.Length > 0)
        {
            foreach (var word in Plugin.StringGameLobbyFilterWords.Value.Split('|'))
            {
                filterList.Add(word);
            }
        }
    }

    [HarmonyPatch(typeof(LobbyListManager), "DisplayLobbies")]
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
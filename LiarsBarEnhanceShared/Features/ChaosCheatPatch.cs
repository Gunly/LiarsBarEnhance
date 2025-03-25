#if CHEATRELEASE
using HarmonyLib;

using System.Collections.Generic;

using UnityEngine;

namespace LiarsBarEnhance.Features;

[HarmonyPatch]
public class ChaosCheatPatch
{
    private static readonly Dictionary<Card, bool> cardFliped = [];

    [HarmonyPatch(typeof(ChaosGamePlay), "UpdateCall")]
    [HarmonyPostfix]
    public static void UpdateCallPostfix(ChaosGamePlay __instance)
    {
        if (!Plugin.BooleanCheatDeck.Value) return;
        if (__instance.isOwned)
        {
            for (var i = 0; i < __instance.Cards.Count; i++)
            {
                var cardObject = __instance.Cards[i];
                if (!cardObject.gameObject.activeSelf) continue;
                if (Plugin.KeyCheatChangeCardDice[i].IsDown() && __instance.manager().PluginControl())
                {
                    var card = cardObject.GetComponent<Card>();
                    if (card.cardtype == 4)
                    {
                        card.cardtype = 1;
                    }
                    else
                    {
                        card.cardtype++;
                    }
                    card.SetCard();
                }
            }
        }
        else
        {
            for (var i = 0; i < __instance.Cards.Count; i++)
            {
                var cardObject = __instance.Cards[i];
                if (!cardObject.gameObject.activeSelf) continue;
                var card = cardObject.GetComponent<Card>();
                card.activecard = true;
                if (!cardFliped.ContainsKey(card)) cardFliped.Add(card, false);
                if (card.Devil)
                {
                    card.GetComponent<MeshRenderer>().material = Manager.Instance.devil;
                    if (Manager.Instance.BlorfGame.RoundCard == 1)
                    {
                        card.GetComponent<MeshFilter>().sharedMesh = Manager.Instance.BlorfGame.Card1;
                    }
                    else if (Manager.Instance.BlorfGame.RoundCard == 2)
                    {
                        card.GetComponent<MeshFilter>().sharedMesh = Manager.Instance.BlorfGame.Card2;
                    }
                    else if (Manager.Instance.BlorfGame.RoundCard == 3)
                    {
                        card.GetComponent<MeshFilter>().sharedMesh = Manager.Instance.BlorfGame.Card3;
                    }
                }
                else
                {
                    card.GetComponent<MeshRenderer>().material = card.normal;
                }
                if (__instance.manager().PluginControl())
                {
                    if (Plugin.KeyCheatBlorfFlip.IsPressed())
                    {
                        if (!cardFliped[card])
                        {
                            cardObject.transform.localScale = new Vector3(-100 * Plugin.FloatCheatCardSize.Value, 100 * Plugin.FloatCheatCardSize.Value, -100 * Plugin.FloatCheatCardSize.Value);
                            cardObject.transform.Translate(Vector3.up * ((Plugin.FloatCheatCardSize.Value - 1f) / 10.8f), Space.Self);
                            cardFliped[card] = true;
                        }
                    }
                    else
                    {
                        if (cardFliped[card])
                        {
                            cardObject.transform.localScale = new Vector3(100, 100, 100);
                            cardObject.transform.Translate(Vector3.down * ((Plugin.FloatCheatCardSize.Value - 1f) / 10.8f), Space.Self);
                            cardFliped[card] = false;
                        }
                    }
                }
            }
        }
    }

    [HarmonyPatch(typeof(CharController), "Start")]
    [HarmonyPostfix]
    public static void StartPostfix(CharController __instance)
    {
        if (__instance.isOwned && __instance is ChaosGamePlay chaosGame)
        {
            Plugin.IntCheatBlorfRevoler.Value = 0;
            Plugin.IntCheatBlorfHealth.SettingChanged += (_, _) =>
            {
                if (Plugin.BooleanCheatDeck.Value)
                    chaosGame.Networkrevolverbulllet = Plugin.IntCheatBlorfHealth.Value - 1;
            };
            Plugin.IntCheatBlorfRevoler.SettingChanged += (_, _) =>
            {
                if (Plugin.BooleanCheatDeck.Value)
                    chaosGame.Networkcurrentrevoler = Plugin.IntCheatBlorfRevoler.Value;
            };
        }
    }

    [HarmonyPatch(typeof(ChaosGamePlay), nameof(ChaosGamePlay.Networkrevolverbulllet), MethodType.Setter)]
    [HarmonyPostfix]
    public static void NetworkrevolverbullletPostfix(ChaosGamePlay __instance)
    {
        if (!__instance.isOwned) return;
        if (Plugin.IntCheatBlorfHealth.Value != __instance.Networkrevolverbulllet + 1)
            Plugin.IntCheatBlorfHealth.Value = __instance.Networkrevolverbulllet + 1;
    }

    [HarmonyPatch(typeof(ChaosGamePlay), nameof(ChaosGamePlay.Networkcurrentrevoler), MethodType.Setter)]
    [HarmonyPostfix]
    public static void NetworkcurrentrevolerPostfix(ChaosGamePlay __instance)
    {
        if (!__instance.isOwned) return;
        if (Plugin.IntCheatBlorfRevoler.Value != __instance.Networkcurrentrevoler)
            Plugin.IntCheatBlorfRevoler.Value = __instance.Networkcurrentrevoler;
    }
}
#endif
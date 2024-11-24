#if CHEATRELEASE
using HarmonyLib;

using Mirror;

using System.Collections.Generic;

using UnityEngine;

namespace LiarsBarEnhance.Features;

[HarmonyPatch]
public class BlorfCheatPatch
{
    private static readonly Dictionary<Card, Vector3> cardPositons = [], cardRotations = [], cardScales = [];

    [HarmonyPatch(typeof(Card), "Update")]
    [HarmonyPrefix]
    public static void UpdatePrefix(Card __instance)
    {
        if (Plugin.BooleanCheatBlorf.Value)
        {
            var n = __instance.transform.root.GetComponent<NetworkIdentity>();
            if (n == null)
            {
                Reset(__instance);
                return;
            }
            if (!n.isOwned)
            {
                __instance.activecard = true;
                if (__instance.Devil)
                {
                    __instance.GetComponent<MeshRenderer>().material = Manager.Instance.devil;
                    if (Manager.Instance.BlorfGame.RoundCard == 1)
                    {
                        __instance.GetComponent<MeshFilter>().sharedMesh = Manager.Instance.BlorfGame.Card1;
                    }
                    else if (Manager.Instance.BlorfGame.RoundCard == 2)
                    {
                        __instance.GetComponent<MeshFilter>().sharedMesh = Manager.Instance.BlorfGame.Card2;
                    }
                    else if (Manager.Instance.BlorfGame.RoundCard == 3)
                    {
                        __instance.GetComponent<MeshFilter>().sharedMesh = Manager.Instance.BlorfGame.Card3;
                    }
                }
                else
                {
                    __instance.GetComponent<MeshRenderer>().material = __instance.normal;
                }
                if (!cardPositons.ContainsKey(__instance))
                {
                    cardPositons.Add(__instance, __instance.transform.localPosition);
                    cardRotations.Add(__instance, __instance.transform.localEulerAngles);
                    cardScales.Add(__instance, __instance.transform.localScale);
                }
                if (Plugin.KeyCheatBlorfFlip.IsDown())
                {
                    Reset(__instance);
                    __instance.transform.Translate(Vector3.up * ((Plugin.FloatCheatCardSize.Value - 1f) / 10.8f), Space.Self);
                    __instance.transform.Rotate(180f, 0f, 0f, Space.Self);
                    __instance.transform.localScale = cardScales[__instance] * Plugin.FloatCheatCardSize.Value;
                }
                if (Plugin.KeyCheatBlorfFlip.IsUp())
                {
                    Reset(__instance);
                }
            }
        }
    }

    private static void Reset(Card card)
    {
        if (cardPositons.ContainsKey(card)) card.transform.localPosition = cardPositons[card];
        if (cardRotations.ContainsKey(card)) card.transform.localEulerAngles = cardRotations[card];
        if (cardScales.ContainsKey(card)) card.transform.localScale = cardScales[card];
    }

    private static bool handlerset = false;
    [HarmonyPatch(typeof(BlorfGamePlay), "UpdateCall")]
    [HarmonyPostfix]
    public static void UpdateCallPostfix(BlorfGamePlay __instance, bool ___revolverset)
    {
        if (__instance.isOwned && Plugin.BooleanCheatBlorf.Value && !handlerset && ___revolverset)
        {
            handlerset = true;
            Plugin.IntCheatBlorfHealth.Value = __instance.Networkrevolverbulllet;
            Plugin.IntCheatBlorfHealth.SettingChanged += (sender, args) => __instance.Networkrevolverbulllet = Plugin.IntCheatBlorfHealth.Value - 1;
        }
    }
}
#endif
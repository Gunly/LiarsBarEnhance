#if CHEATRELEASE
using HarmonyLib;
using LiarsBarEnhance.Utils;
using Mirror;
using System.Collections.Generic;
using UnityEngine;

namespace LiarsBarEnhance.Features;

[HarmonyPatch]
public class BlorfCheatPatch
{
    private static readonly Dictionary<Card, Vector3> CardPositons = [], CardRotations = [], CardScales = [];

    [HarmonyPatch(typeof(Card), "Update")]
    [HarmonyPrefix]
    public static void UpdatePrefix(Card __instance)
    {
        if (Plugin.BooleanCheatCard.Value && __instance.gameObject.activeInHierarchy)
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
                }
                else
                {
                    __instance.GetComponent<MeshRenderer>().material = __instance.normal;
                }
                if (!CardPositons.ContainsKey(__instance))
                {
                    CardPositons.Add(__instance, __instance.transform.localPosition);
                    CardRotations.Add(__instance, __instance.transform.localEulerAngles);
                    CardScales.Add(__instance, __instance.transform.localScale);
                }
                if (ShortcutInput.IsDown(Plugin.KeyCheatDeckFlip))
                {
                    Reset(__instance);
                    __instance.transform.Translate(Vector3.up * ((Plugin.FloatCheatCardSize.Value - 1f) / 10.8f), Space.Self);
                    __instance.transform.Rotate(180f, 0f, 0f, Space.Self);
                    __instance.transform.localScale = CardScales[__instance] * Plugin.FloatCheatCardSize.Value;
                }
                if (ShortcutInput.IsUp(Plugin.KeyCheatDeckFlip))
                {
                    Reset(__instance);
                }
            }
        }
    }

    private static void Reset(Card card)
    {
        if (CardPositons.ContainsKey(card)) card.transform.localPosition = CardPositons[card];
        if (CardRotations.ContainsKey(card)) card.transform.localEulerAngles = CardRotations[card];
        if (CardScales.ContainsKey(card)) card.transform.localScale = CardScales[card];
    }
}
#endif
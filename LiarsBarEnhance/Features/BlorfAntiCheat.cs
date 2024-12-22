using System.Collections;
using System.Collections.Generic;

using HarmonyLib;

using UnityEngine;

namespace LiarsBarEnhance.Features;

[HarmonyPatch]
public class BlorfAntiCheat
{
    private static readonly List<int> realCardTypes = [];
    private static int currentcard = -1;

    [HarmonyPatch(typeof(BlorfGamePlay), "UserCode_RandomCards__Int32__Int32__Int32__Int32__Int32")]
    [HarmonyPrefix]
    public static void RandomCardsPrefix(
        BlorfGamePlay __instance,
        ref bool __runOriginal,
        int card1,
        int card2,
        int card3,
        int card4,
        int card5
    )
    {
        if (!__instance.isOwned) return;
        __runOriginal = false;
        __instance.CardTypes = [4, 4, 4, 4, 4];
        realCardTypes.Clear();
        realCardTypes.Add(card1);
        realCardTypes.Add(card2);
        realCardTypes.Add(card3);
        realCardTypes.Add(card4);
        realCardTypes.Add(card5);
        AccessTools.Method(typeof(BlorfGamePlay), "SetCardsCmd").Invoke(__instance, null);
    }

    [HarmonyPatch(typeof(BlorfGamePlay), "UserCode_SetCardsRpc")]
    [HarmonyPrefix]
    public static bool SetCardsRpcPrefix(BlorfGamePlay __instance)
    {
        if (!__instance.isOwned) return true;
        for (var i = 0; i < 5; i++)
        {
            var card = __instance.Cards[i].GetComponent<Card>();
            currentcard = i; //currentcard存储迭代次数
            card.Devil = false;
            card.gameObject.layer = 0;
            card.cardtype = __instance.CardTypes[i];
            if (realCardTypes[i] == -1) //这里本来应该是判断this.CardTypes是否是-1，为了保持手牌的真实改为用RealCardTypes判断
            {
                card.Devil = true;
            }
            card.Selected = false;
            card.gameObject.SetActive(true);
            card.SetCard();
        }
        currentcard = -1;
        return false;
    }

    [HarmonyPatch(typeof(Card), "SetCard")]
    [HarmonyPrefix]
    public static bool SetCardPrefix(Card __instance)
    {
        if (currentcard == -1) return true;
        if (__instance.Devil) //使用currentcard判断卡牌类型，以此正确设置真实的卡牌
        {
            __instance.GetComponent<MeshRenderer>().material = Manager.Instance.devil;
            __instance.GetComponent<MeshFilter>().sharedMesh = Manager.Instance.BlorfGame.RoundCard switch
            {
                1 => Manager.Instance.BlorfGame.Card1,
                2 => Manager.Instance.BlorfGame.Card2,
                _ => Manager.Instance.BlorfGame.Card3
            };
        }
        else
        {
            __instance.GetComponent<MeshFilter>().sharedMesh = realCardTypes[currentcard] switch
            {
                1 => Manager.Instance.BlorfGame.Card1,
                2 => Manager.Instance.BlorfGame.Card2,
                3 => Manager.Instance.BlorfGame.Card3,
                _ => Manager.Instance.BlorfGame.Card4
            };
        }
        return false;
    }

    [HarmonyPatch(typeof(BlorfGamePlay), "ThrowCards")]
    [HarmonyPrefix]
    public static void ThrowCardsPrefix(BlorfGamePlay __instance, ref bool __runOriginal, ref bool ___throwed)
    {
        __runOriginal = false;
        var waitforthreecardreset = AccessTools.Method(
            typeof(BlorfGamePlay),
            "waitforthreecardreset"
        );
        var ThrowCardsCmd = AccessTools.Method(typeof(BlorfGamePlay), "ThrowCardsCmd");
        var WaitforCheck = AccessTools.Method(typeof(BlorfGamePlay), "WaitforCheck");
        var WaitforCardThrow = AccessTools.Method(typeof(BlorfGamePlay), "WaitforCardThrow");

        __instance.StartCoroutine((IEnumerator)waitforthreecardreset.Invoke(__instance, null));
        ___throwed = true;
        __instance.animator.SetBool("Throw", true);
        List<int> list = [];
        for (var i = 0; i < __instance.Cards.Count; i++)
        {
            if (__instance.Cards[i].gameObject.activeSelf && __instance.Cards[i].GetComponent<Card>().Selected)
            {
                list.Add(realCardTypes[i]);
            }
        }
        __instance.StartCoroutine((IEnumerator)WaitforCardThrow.Invoke(__instance, null));
        ThrowCardsCmd.Invoke(__instance, [list]);
        __instance.StartCoroutine((IEnumerator)WaitforCheck.Invoke(__instance, null));
    }
}

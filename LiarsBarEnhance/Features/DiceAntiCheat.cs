using System.Collections.Generic;
using HarmonyLib;
using Mirror;
using UnityEngine;

namespace LiarsBarEnhance.Features;

[HarmonyPatch]
public class DiceAntiCheat
{
    private static SyncList<int> RealDiceValues = new SyncList<int>();

    [HarmonyPatch(typeof(DiceGamePlay), "Shake")]
    [HarmonyPostfix]
    public static void ShakePostfix(DiceGamePlay __instance)
    {
        RealDiceValues.Clear();
        for (int i = 0; i < __instance.TotalDices; i++)
        {
            RealDiceValues.Add(Random.Range(1, 7)); //生成真骰子
        }
    }

    [HarmonyPatch(typeof(DiceGamePlay), "UpdateCall")]
    [HarmonyPostfix]
    public static void UpdateCallPostfix(DiceGamePlay __instance)
    {
        if (!__instance.isOwned)
            return;  //如果不判断isOwned别人的骰子也会渲染成自己的真骰子
        var dicerendersField = AccessTools.Field(typeof(DiceGamePlay), "dicerenders");
        var dicerenders = dicerendersField.GetValue(__instance) as List<Dice>;

        var manager =
            AccessTools.Field(typeof(CharController), "manager").GetValue(__instance) as Manager;

        if (RealDiceValues.Count > 0)
        {
            for (int i = 0; i < dicerenders.Count; i++)
            {
                dicerenders[i].Face = RealDiceValues[i]; //将假骰子替换渲染为真骰子
            }
        }
        dicerendersField.SetValue(__instance, dicerenders);
        if (manager.DiceGame.CalledLiar) //CallSpotOn也是用的CalledLiar成员,所以判断CalledLiar就可以了
        {
            __instance.DiceValues.Clear();
            for (int i = 0; i < __instance.TotalDices; i++) //因为DiceValues是readonly所以只能用这种蹩脚的方式替换
            {
                __instance.DiceValues.Add(RealDiceValues[i]); //在结算前把DiceValues改为真骰子
            }
        }
    }
}

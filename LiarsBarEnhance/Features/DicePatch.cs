using HarmonyLib;

using UnityEngine;

namespace LiarsBarEnhance.Features;

[HarmonyPatch]
public class DicePatch
{
    [HarmonyPatch(typeof(Dice), nameof(Dice.Start))]
    [HarmonyPostfix]
    public static void StartPostfix(Dice __instance)
    {
        __instance.Owned = __instance.transform.root.GetComponent<CharController>().isOwned;
        if (Manager.Instance != null)
        {
            if (__instance.Owned)
            {
                __instance.renderer.material = Manager.Instance.zar1;
            }
            else
            {
                __instance.renderer.material = Manager.Instance.zar2;
            }
        }
    }

    [HarmonyPatch(typeof(Dice), nameof(Dice.Update))]
    [HarmonyPrefix]
    public static bool UpdatePrefix(Dice __instance)
    {
        if (__instance.Owned)
        {
            switch (__instance.Face)
            {
                case 1:
                    __instance.transform.localEulerAngles = new Vector3(0f, 0f, 0f);
                    break;
                case 2:
                    __instance.transform.localEulerAngles = new Vector3(0f, 0f, -90f);
                    break;
                case 3:
                    __instance.transform.localEulerAngles = new Vector3(90f, 0f, 0f);
                    break;
                case 4:
                    __instance.transform.localEulerAngles = new Vector3(-90f, 0f, 0f);
                    break;
                case 5:
                    __instance.transform.localEulerAngles = new Vector3(0f, 0f, 90f);
                    break;
                case 6:
                    __instance.transform.localEulerAngles = new Vector3(0f, 0f, 180f);
                    break;
            }
        }
        __instance.dice.sprite = __instance.DiceIcons[__instance.Face - 1];
        return false;
    }
}
using HarmonyLib;

using UnityEngine;

namespace LiarsBarEnhance.Features;

[HarmonyPatch]
public class DicePatch
{
    [HarmonyPatch(typeof(Dice), "Start")]
    [HarmonyPostfix]
    public static void StartPostfix(Dice __instance, ref bool ___Owned, MeshRenderer ___renderer)
    {
        ___Owned = __instance.transform.root.GetComponent<CharController>().isOwned;
        if (Manager.Instance != null)
        {
            if (___Owned)
            {
                ___renderer.material = Manager.Instance.zar1;
            }
            else
            {
                ___renderer.material = Manager.Instance.zar2;
            }
        }
    }

    [HarmonyPatch(typeof(Dice), "Update")]
    [HarmonyPrefix]
    public static bool UpdatePrefix(Dice __instance, int ___Face, bool ___Owned)
    {
        if (___Owned)
        {
            switch (___Face)
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
        __instance.dice.sprite = __instance.DiceIcons[___Face - 1];
        return false;
    }
}
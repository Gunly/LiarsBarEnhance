using HarmonyLib;

using UnityEngine;
using LiarsBarEnhance.Utils;

namespace LiarsBarEnhance.Features;

[HarmonyPatch]
public class SelectableLevelPatch
{
    [HarmonyPatch(typeof(Statsui), "Update")]
    [HarmonyPostfix]
    public static void UpdatePostfix(Statsui __instance)
    {
        if (Input.GetMouseButtonDown(1) && __instance.RankParrentTable().gameObject.activeInHierarchy)
        {
            (float distance, RectTransform rank) best = (float.PositiveInfinity, null);
            for (var i = 0; i < __instance.RankParrentTable().childCount; i++)
            {
                var rank = __instance.RankParrentTable().GetChild(i).GetComponent<RectTransform>();
                if (!RectTransformUtility.RectangleContainsScreenPoint(rank, Input.mousePosition) ||
                    !RectTransformUtility.ScreenPointToLocalPointInRectangle(rank, Input.mousePosition, null, out var localPos))
                    continue;

                var distance = localPos.magnitude;
                if (distance < best.distance)
                    best = (distance, rank);
            }

            if (best.rank == null || !int.TryParse(best.rank.name, out var selectedLevel))
                return;

            PlayerLevelHelper.SetLevel((DatabaseManager.Levels)selectedLevel);
            DatabaseManager.instance.SaveDataUser();
        }
    }
}
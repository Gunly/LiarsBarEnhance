using HarmonyLib;

using LiarsBarEnhance.Utils;

using UnityEngine;
using UnityEngine.UI;

namespace LiarsBarEnhance.Components;

public class SelectableLevelController : MonoBehaviour
{
    private RectTransform rankParentTable;

    private void Start()
    {
        var statsUI = GetComponent<Statsui>();
        rankParentTable = (RectTransform)Traverse.Create(statsUI).Field("RankParrentTable").GetValue<Transform>();

        var button = rankParentTable.parent.Find("rankpage").gameObject.AddComponent<Button>();
        button.interactable = true;
        button.colors = new ColorBlock
        {
            normalColor = Color.white,
            highlightedColor = Color.white,
            pressedColor = Color.white,
            selectedColor = Color.white,
            disabledColor = Color.white,
            colorMultiplier = 1f,
            fadeDuration = 0f
        };
        button.onClick.AddListener(OnRankPageButtonClicked);

        Plugin.Logger.LogDebug($"[{typeof(SelectableLevelController)}] Registered button for rank page");
    }

    private void OnRankPageButtonClicked()
    {
        (float distance, RectTransform rank) best = (float.PositiveInfinity, null);
        for (var i = 0; i < rankParentTable.childCount; i++)
        {
            var rank = rankParentTable.GetChild(i).GetComponent<RectTransform>();
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
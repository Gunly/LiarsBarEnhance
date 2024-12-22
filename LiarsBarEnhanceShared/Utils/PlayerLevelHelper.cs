using System;
using System.Collections.Generic;
using HarmonyLib;

namespace LiarsBarEnhance.Utils;

public static class PlayerLevelHelper
{
    private static readonly List<int> xpNeededList = [];
    private static readonly Action calculateNeeds;

    static PlayerLevelHelper()
    {
        var xpSave = DatabaseManager.instance.XP;
        var dbManager = DatabaseManager.instance;
        calculateNeeds = (Action)AccessTools.Method(typeof(DatabaseManager), "CalculateNeeds")
            .CreateDelegate(typeof(Action), dbManager);

        xpNeededList.Add(0);
        SetXp(0);
        calculateNeeds();
        while (dbManager.needs != 0)
        {
            xpNeededList.Add(xpNeededList[^1] + dbManager.needs);
            SetXp(xpNeededList[^1]);
            calculateNeeds();
        }

        SetXp(xpSave);
        calculateNeeds();
    }

    public static void SetXp(int xp)
        => FastMemberAccessor<DatabaseManager, int>.Set(DatabaseManager.instance, "xp", xp);

    public static void SetLevel(DatabaseManager.Levels level, int xpMore = 0)
        => SetXp(xpNeededList[(int)level] + xpMore);

    public static void CalculateNeeds()
        => calculateNeeds();

    public static int GetNeedXp(DatabaseManager.Levels level)
        => xpNeededList[(int)level];
}
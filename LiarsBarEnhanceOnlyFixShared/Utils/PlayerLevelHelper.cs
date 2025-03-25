using System.Collections.Generic;

namespace LiarsBarEnhance.Utils;

public static class PlayerLevelHelper
{
    private static readonly List<int> xpNeededList = [];

    static PlayerLevelHelper()
    {
        var xpSave = DatabaseManager.instance.XP;
        var dbManager = DatabaseManager.instance;

        xpNeededList.Add(0);
        SetXp(0);
        CalculateNeeds();
        while (dbManager.needs != 0)
        {
            xpNeededList.Add(xpNeededList[^1] + dbManager.needs);
            SetXp(xpNeededList[^1]);
            dbManager.CallMethod("CalculateNeeds");
        }

        SetXp(xpSave);
        CalculateNeeds();
    }

    public static void SetXp(int xp)
        => FastMemberAccessor<DatabaseManager, int>.Set(DatabaseManager.instance, "xp", xp);

    public static void SetLevel(DatabaseManager.Levels level, int xpMore = 0)
        => SetXp(xpNeededList[(int)level] + xpMore);

    public static void CalculateNeeds()
        => DatabaseManager.instance.CallMethod("CalculateNeeds");

    public static int GetNeedXp(DatabaseManager.Levels level)
        => xpNeededList[(int)level];
}
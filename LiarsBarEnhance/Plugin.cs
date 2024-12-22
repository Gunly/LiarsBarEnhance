using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;

using HarmonyLib;

using LiarsBarEnhance.Features;
using LiarsBarEnhance.Utils;

using System;

using UnityEngine;
using UnityEngine.SceneManagement;

namespace LiarsBarEnhance;

[BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
public class Plugin : BaseUnityPlugin
{
    internal new static ManualLogSource Logger;
    private static ConfigEntry<int> intPositionNum, intAnimationNum, intGammXP;
    public static ConfigEntry<int> IntHintPosX, IntHintPosY;
    public static int InitPositionNumValue, InitAnimationNumValue;
    public static ConfigEntry<KeyboardShortcut> KeyCustomBigMouth, KeyGameShowHint,
        KeyAnimCallLiar, KeyAnimDrink, KeyAnimReload, KeyAnimRoulet, KeyAnimShake, KeyAnimShow,
        KeyAnimSpotOn, KeyAnimThrow, KeyAnimTakeAim, KeyAnimFire, KeyAnimEmpty,
        KeyMoveForward, KeyMoveBack, KeyMoveLeft, KeyMoveRight, KeyMoveJump, KeyMoveSquat, KeyMoveResetPosition, KeyMoveFollowHeadShortcut,
        KeyViewCrazyShakeHead, KeyViewRemoveRotationLimit, KeyViewReset, KeyViewField,
        KeyViewForward, KeyViewBack, KeyViewLeft, KeyViewRight, KeyViewUp, KeyViewDown, KeyViewClockwise, KeyViewAnticlockwise,
        KeyRotateYaw, KeyRotatePitch, KeyRotateRoll, KeyRotateAuto,
        KeyGameReturnMenu;
    public static ConfigEntry<KeyboardShortcut>[] KeyPosition;
    public static ConfigEntry<KeyboardShortcut>[] KeyAnims;
    public static ConfigEntry<Vector3>[] VectorPosition, VectorRotation;
    public static ConfigEntry<float> FloatJumpHeight, FloatGravity, FloatMoveSpeed, FloatMoveHorizontalBodyRotate, FloatViewSpeed, FloatViewField,
        FloatAutoRotateSpeed, FloatCustomPlayerScale, FloatBigMouthAngle;
    public static ConfigEntry<bool> BooleanMoveFollowHead, BooleanViewRemoveRotationLimit, BooleanViewField,
        BooleanTestGiraffe, BooleanCustomShowSelfInfo;
    public static ConfigEntry<string> StringCustomName, StringGameLobbyFilterWords;
    public static ConfigEntry<string>[] StringAnims;
    public static ConfigEntry<RotateDirection> DirectionRotateState;
    public static ConfigEntry<HintType> HintTypeSelect;
#if CHEATRELEASE
    public static ConfigEntry<bool> BooleanCheatDeck, BooleanCheatDice, BooleanCheatDeckHealth, BooleanCheatBlorfLastRoundCard, BooleanCheatDiceTotalDice;
    public static ConfigEntry<KeyboardShortcut> KeyCheatBlorfFlip, KeyCheatDiceShow, KeyAnimRouletType;
    public static ConfigEntry<KeyboardShortcut>[] KeyCheatChangeCardDice;
    public static ConfigEntry<float> FloatCheatCardSize;
    public static ConfigEntry<int> IntCheatBlorfHealth, IntCheatBlorfRevoler;
    public static ConfigEntry<RouletType> RouletAnimType;
#endif

    private void Awake()
    {
        Logger = base.Logger;
        BindConfig();

        Harmony.CreateAndPatchAll(typeof(ChatProPatch), nameof(ChatProPatch));
        Harmony.CreateAndPatchAll(typeof(ChineseNameFixPatch), nameof(ChineseNameFixPatch));
        Harmony.CreateAndPatchAll(typeof(DisableTmpWarningsPatch), nameof(DisableTmpWarningsPatch));
        Harmony.CreateAndPatchAll(typeof(RemoveNameLengthLimitPatch), nameof(RemoveNameLengthLimitPatch));
        Harmony.CreateAndPatchAll(typeof(ResetSettingsPatch), nameof(ResetSettingsPatch));
        Harmony.CreateAndPatchAll(typeof(ScrollViewPatch), nameof(ScrollViewPatch));
        Harmony.CreateAndPatchAll(typeof(TestPatch), nameof(TestPatch));
        Harmony.CreateAndPatchAll(typeof(RemoveHeadRotationlimitPatch), nameof(RemoveHeadRotationlimitPatch));
        Harmony.CreateAndPatchAll(typeof(CrazyShakeHeadPatch), nameof(CrazyShakeHeadPatch));
        Harmony.CreateAndPatchAll(typeof(BigMouthPatch), nameof(BigMouthPatch));
        Harmony.CreateAndPatchAll(typeof(CharMoveablePatch), nameof(CharMoveablePatch));
        Harmony.CreateAndPatchAll(typeof(SelectableLevelPatch), nameof(SelectableLevelPatch));
        Harmony.CreateAndPatchAll(typeof(TeleportPatch), nameof(TeleportPatch));
        Harmony.CreateAndPatchAll(typeof(AutoRotatePatch), nameof(AutoRotatePatch));
        Harmony.CreateAndPatchAll(typeof(CharScalePatch), nameof(CharScalePatch));
        Harmony.CreateAndPatchAll(typeof(ShowSelfTopInfoPatch), nameof(ShowSelfTopInfoPatch));
        Harmony.CreateAndPatchAll(typeof(FOVPatch), nameof(FOVPatch));
        Harmony.CreateAndPatchAll(typeof(CustomNamePatch), nameof(CustomNamePatch));
        Harmony.CreateAndPatchAll(typeof(AnimationPatch), nameof(AnimationPatch));
        Harmony.CreateAndPatchAll(typeof(GiraffePatch), nameof(GiraffePatch));
        Harmony.CreateAndPatchAll(typeof(LobbyFilterPatch), nameof(LobbyFilterPatch));

#if CHEATRELEASE
        Harmony.CreateAndPatchAll(typeof(BlorfCheatPatch), nameof(BlorfCheatPatch));
        Harmony.CreateAndPatchAll(typeof(DiceCheatPatch), nameof(DiceCheatPatch));
        Harmony.CreateAndPatchAll(typeof(ChaosCheatPatch), nameof(ChaosCheatPatch));
#else
        Harmony.CreateAndPatchAll(typeof(BlorfAntiCheat), nameof(BlorfAntiCheat));
        Harmony.CreateAndPatchAll(typeof(DiceAntiCheat), nameof(DiceAntiCheat));
        Harmony.CreateAndPatchAll(typeof(DicePatch), nameof(DicePatch));
#endif

        Logger.LogInfo($"Plugin {PluginInfo.PLUGIN_GUID} is loaded!");
    }

    private void Update()
    {
        if (KeyGameReturnMenu.IsDown()) SceneManager.LoadScene("SteamTest");
    }

    private void BindConfig()
    {
        intPositionNum = Config.Bind("AAARepeatConfigNum", "PositionNum", 4, new ConfigDescription("可传送坐标数量(需要重启游戏)", new AcceptableValueRange<int>(0, 9)));
        intAnimationNum = Config.Bind("AAARepeatConfigNum", "AnimationNum", 0, new ConfigDescription("可使用动画数量(需要重启游戏)", new AcceptableValueRange<int>(0, 9)));
        InitPositionNumValue = intPositionNum.Value;
        InitAnimationNumValue = intAnimationNum.Value;

#if CHEATRELEASE
        BooleanCheatDeck = Config.Bind("Cheat", "CheatDeck", false, "Deck模式作弊");
        BooleanCheatDice = Config.Bind("Cheat", "CheatDice", false, "Dice模式作弊");
        KeyCheatBlorfFlip = Config.Bind("Cheat", "CardFlip", new KeyboardShortcut(KeyCode.LeftControl), "翻转放大其他玩家卡牌");
        KeyCheatDiceShow = Config.Bind("Cheat", "DiceShow", new KeyboardShortcut(KeyCode.LeftControl), "显示其他玩家骰子");
        FloatCheatCardSize = Config.Bind("Cheat", "CardSize", 1f, new ConfigDescription("放大大小", new AcceptableValueRange<float>(1f, 10f)));
        IntCheatBlorfHealth = Config.Bind("Cheat", "DeckHealth", 6, new ConfigDescription("Deck模式生命值(开始游戏后更改生效)", new AcceptableValueRange<int>(1, 50)));
        IntCheatBlorfRevoler = Config.Bind("Cheat", "DeckRevoler", 0, new ConfigDescription("开枪数", new AcceptableValueRange<int>(0, 49)));
        BooleanCheatDeckHealth = Config.Bind("Cheat", "ShowHealth", false, "显示生命值(第几枪实弹)");
        BooleanCheatBlorfLastRoundCard = Config.Bind("Cheat", "ShowRoundCard", false, "显示当前出牌(提示GUI)");
        BooleanCheatDiceTotalDice = Config.Bind("Cheat", "ShowTotalDice", false, "显示骰子总数(提示GUI)");
        KeyCheatChangeCardDice = new ConfigEntry<KeyboardShortcut>[5];
        for (var i = 0; i < KeyCheatChangeCardDice.Length; i++)
        {
            KeyCheatChangeCardDice[i] = Config.Bind("Cheat", $"Change{i + 1}", new KeyboardShortcut(KeyCode.None), $"改变第{i + 1}个牌/骰子");
        }
#endif

        KeyCustomBigMouth = Config.Bind("Custom", "BigMouth", new KeyboardShortcut(KeyCode.B), "张嘴");
        FloatBigMouthAngle = Config.Bind("Custom", "BigMouthAngle", 60f, new ConfigDescription("张嘴角度", new AcceptableValueRange<float>(-180f, 180f)));
        StringCustomName = Config.Bind("Custom", "CustomName", "", "自定义名称");
        BooleanCustomShowSelfInfo = Config.Bind("Custom", "ShowSelfInfo", true, "显示自身头顶信息");
        FloatCustomPlayerScale = Config.Bind("Custom", "PlayerScale", 0.5f, new ConfigDescription("玩家缩放", new AcceptableValueRange<float>(0f, 1f)));

        KeyGameShowHint = Config.Bind("Game", "HintShow", new KeyboardShortcut(KeyCode.Tab), "启用提示");
        KeyMoveResetPosition = Config.Bind("Game", "ResetPosition", new KeyboardShortcut(KeyCode.R), "重置坐标");
        KeyViewReset = Config.Bind("Game", "ResetView", new KeyboardShortcut(KeyCode.T), "重置视角");
        HintTypeSelect = Config.Bind("Game", "HintType", HintType.All, "选择需要的提示信息");
        IntHintPosX = Config.Bind("Game", "HintPosX", -300, new ConfigDescription("提示坐标X, 负数表示以屏幕宽度减去设置值))", new AcceptableValueRange<int>(-2000, 2000)));
        IntHintPosY = Config.Bind("Game", "HintPosY", 60, new ConfigDescription("提示坐标Y, 负数表示以屏幕高度减去设置值))", new AcceptableValueRange<int>(-1000, 1000)));
        KeyGameReturnMenu = Config.Bind("Game", "ReturnMenu", new KeyboardShortcut(KeyCode.F9), "回到主菜单(解决卡死)");
        intGammXP = Config.Bind("Game", "XP", 0, new ConfigDescription("经验值", new AcceptableValueRange<int>(0, 10000)));
        StringGameLobbyFilterWords = Config.Bind("Game", "LobbyFilterWords", "透视|改牌|透牌|修改|枪数|无敌|低价|稳定|免费|加q|加群|售后|看片|网址|国产|少妇|" +
            "@[Qq]\\d{5,}|@[a-zA-Z0-9]{3,}\\.[a-zA-Z]{2,}", "大厅过滤词");
        LobbyFilterPatch.FilterWords = () => StringGameLobbyFilterWords.Value;
        StringGameLobbyFilterWords.SettingChanged += (_, _) => LobbyFilterPatch.FilterWordsChanged();

        BooleanMoveFollowHead = Config.Bind("Move", "MoveFollowHead", true, "移动方向跟随头部视角");
        KeyMoveFollowHeadShortcut = Config.Bind("Move", "MoveFollowHeadShortcut", new KeyboardShortcut(KeyCode.H), "切换移动方向跟随头部视角快捷键");
        FloatMoveHorizontalBodyRotate = Config.Bind("Move", "MoveHorizontalBodyRotate", 0f, new ConfigDescription("左右移动时身体旋转角度", new AcceptableValueRange<float>(0f, 90f)));
        KeyMoveForward = Config.Bind("Move", "BodyForward", new KeyboardShortcut(KeyCode.UpArrow), "向前移动");
        KeyMoveBack = Config.Bind("Move", "BodyBack", new KeyboardShortcut(KeyCode.DownArrow), "向后移动");
        KeyMoveLeft = Config.Bind("Move", "BodyLeft", new KeyboardShortcut(KeyCode.LeftArrow), "向左移动");
        KeyMoveRight = Config.Bind("Move", "BodyRight", new KeyboardShortcut(KeyCode.RightArrow), "向右移动");
        KeyMoveJump = Config.Bind("Move", "BodyJump", new KeyboardShortcut(KeyCode.Keypad0), "跳跃(双击进入飞行模式, 飞行模式向上移动)");
        KeyMoveSquat = Config.Bind("Move", "BodySquat", new KeyboardShortcut(KeyCode.RightControl), "蹲下(飞行模式向下移动, 同时按跳跃和蹲下退出飞行模式)");
        FloatJumpHeight = Config.Bind("Move", "JumpHeight", 0.65f, new ConfigDescription("跳跃高度", new AcceptableValueRange<float>(0f, 10f)));
        FloatGravity = Config.Bind("Move", "Gravity", 9.8f, new ConfigDescription("重力加速度(仅对跳跃生效)", new AcceptableValueRange<float>(0f, 100f)));
        FloatMoveSpeed = Config.Bind("Move", "MoveSpeed", 4f, new ConfigDescription("移动速度", new AcceptableValueRange<float>(0f, 100f)));

        KeyViewCrazyShakeHead = Config.Bind("View", "CrazyShakeHead", new KeyboardShortcut(KeyCode.C), "疯狂摇头");
        BooleanViewField = Config.Bind("View", "MouseViewField", true, "滚轮调整视场角");
        FloatViewField = Config.Bind("View", "ViewField", 60f, new ConfigDescription("视场角", new AcceptableValueRange<float>(1f, 180f)));
        KeyViewField = Config.Bind("View", "ViewFieldKey", new KeyboardShortcut(KeyCode.Mouse2), "切换滚轮调整视场角");
        BooleanViewRemoveRotationLimit = Config.Bind("View", "RemoveRotationLimit", true, "移除视限制");
        KeyViewRemoveRotationLimit = Config.Bind("View", "RemoveRotationLimitShortcut", new KeyboardShortcut(KeyCode.G), "切换移除视角限制快捷键");
        KeyViewForward = Config.Bind("View", "ViewForward", new KeyboardShortcut(KeyCode.I), "头部/视角向前移动");
        KeyViewBack = Config.Bind("View", "ViewBack", new KeyboardShortcut(KeyCode.K), "头部/视角向后移动");
        KeyViewLeft = Config.Bind("View", "ViewLeft", new KeyboardShortcut(KeyCode.J), "头部/视角向左移动");
        KeyViewRight = Config.Bind("View", "ViewRight", new KeyboardShortcut(KeyCode.L), "头部/视角向右移动");
        KeyViewUp = Config.Bind("View", "ViewUp", new KeyboardShortcut(KeyCode.O), "头部/视角向上移动");
        KeyViewDown = Config.Bind("View", "ViewDown", new KeyboardShortcut(KeyCode.U), "头部/视角向下移动");
        KeyViewClockwise = Config.Bind("View", "ViewClockwise", new KeyboardShortcut(KeyCode.RightBracket), "头部/视角顺时针旋转");
        KeyViewAnticlockwise = Config.Bind("View", "ViewAnticlockwise", new KeyboardShortcut(KeyCode.LeftBracket), "头部/视角逆时针旋转");
        FloatViewSpeed = Config.Bind("View", "ViewSpeed", 3f, new ConfigDescription("视角移动速度", new AcceptableValueRange<float>(0f, 100f)));

        KeyRotateYaw = Config.Bind("Rotate", "RotateYaw", new KeyboardShortcut(KeyCode.Mouse1), "水平旋转(Yaw, 偏航角, 按住移动鼠标)");
        KeyRotatePitch = Config.Bind("Rotate", "RotatePitch", new KeyboardShortcut(KeyCode.Mouse2), "垂直旋转(Pitch, 俯仰角, 按住移动鼠标)");
        KeyRotateRoll = Config.Bind("Rotate", "RotateRoll", new KeyboardShortcut(KeyCode.Mouse0), "垂直旋转(Roll, 翻滚角, 按住移动鼠标)");
        KeyRotateAuto = Config.Bind("Rotate", "AutoRotate", new KeyboardShortcut(KeyCode.P), "自动旋转");
        DirectionRotateState = Config.Bind("Rotate", "RotateState", RotateDirection.Yaw, "自动旋转方向");
        FloatAutoRotateSpeed = Config.Bind("Rotate", "AutoRotateSpeed", 0.2f, new ConfigDescription("自动旋转速度(圈/秒)", new AcceptableValueRange<float>(-10f, 10f)));

        if (intPositionNum.Value > 0)
        {
            KeyPosition = new ConfigEntry<KeyboardShortcut>[intPositionNum.Value];
            VectorPosition = new ConfigEntry<Vector3>[intPositionNum.Value];
            VectorRotation = new ConfigEntry<Vector3>[intPositionNum.Value];
            Vector3[] DefaultPositions = [
                new(0.36f, 0.3f, -9.79f),
                new(1.69f, 0.3f, -8.46f),
                new(0.36f, 0.3f, -7.13f),
                new(-0.97f, 0.3f, -8.46f),
                new(16.26f, 0.39f, -37.49f),
                new(17.59f, 0.39f, -36.16f),
                new(16.26f, 0.39f, -34.83f),
                new(14.93f, 0.39f, -36.16f),
                new(0, 0f, 0f)
            ];
            Vector3[] DefaultRotations = [
                new(0f, 0f, 0f),
                new(0f, 270f, 0f),
                new(0f, 180f, 0f),
                new(0f, 90f, 0f),
                new(0f, 0f, 0f),
                new(0f, 270f, 0f),
                new(0f, 180f, 0f),
                new(0f, 90f, 0f),
                new(0f, 0f, 0f)
            ];
            for (var i = 0; i < intPositionNum.Value; i++)
            {
                KeyPosition[i] = Config.Bind("Position", $"Position{i + 1}", new KeyboardShortcut(KeyCode.Alpha1 + i), $"移动到坐标{i + 1}");
                VectorPosition[i] = Config.Bind("Position", $"Position{i + 1}Position", DefaultPositions[i], $"坐标{i + 1}坐标");
                VectorRotation[i] = Config.Bind("Position", $"Position{i + 1}Rotation", DefaultRotations[i], $"坐标{i + 1}角度");
            }
        }

        if (intAnimationNum.Value > 0)
        {
            KeyAnims = new ConfigEntry<KeyboardShortcut>[intAnimationNum.Value];
            StringAnims = new ConfigEntry<string>[intAnimationNum.Value];
            for (var i = 0; i < intAnimationNum.Value; i++)
            {
                KeyAnims[i] = Config.Bind("Anim", $"Anim{i + 1}", new KeyboardShortcut(KeyCode.None), $"动画{i + 1}快捷键");
                StringAnims[i] = Config.Bind("Anim", $"Anim{i + 1}Path", "", $"动画{i + 1}路径");
            }
        }
        KeyAnimCallLiar = Config.Bind("Anim", "CallLiar", new KeyboardShortcut(KeyCode.None), "Call Liar (Deck模式下看牌时可用)(所有模式)");
        KeyAnimSpotOn = Config.Bind("Anim", "SpotOn", new KeyboardShortcut(KeyCode.None), "Spot On (仅Dice)");
        KeyAnimThrow = Config.Bind("Anim", "Throw", new KeyboardShortcut(KeyCode.None), "扔牌 (看牌时按住)(Deck/Chaos)");
        KeyAnimShow = Config.Bind("Anim", "Show", new KeyboardShortcut(KeyCode.None), "展示骰子 (按住)(仅Dice)");
        KeyAnimRoulet = Config.Bind("Anim", "Roulet", new KeyboardShortcut(KeyCode.None), "开枪 (按下举枪, 松开开枪)(Deck/Chaos)");
#if CHEATRELEASE
        RouletAnimType = Config.Bind("Anim", "RouletType", RouletType.AnimOnly, "开枪动画效果\n(若不为仅动画, Chaos模式对他人时开枪与空枪等效)");
        KeyAnimRouletType = Config.Bind("Anim", "RouletTypeChange", new KeyboardShortcut(KeyCode.None), "切换开枪动画效果");
#endif
        KeyAnimDrink = Config.Bind("Anim", "Drink", new KeyboardShortcut(KeyCode.None), "喝酒 (仅Dice)");
        KeyAnimReload = Config.Bind("Anim", "Reload", new KeyboardShortcut(KeyCode.None), "装弹 (按住, 手里没牌时可用)(Deck/Chaos)");
        KeyAnimShake = Config.Bind("Anim", "Shake", new KeyboardShortcut(KeyCode.None), "摇骰子 (仅Dice)");
        KeyAnimTakeAim= Config.Bind("Anim", "Aim", new KeyboardShortcut(KeyCode.None), "举枪(对他人) (仅Chaos)");
        KeyAnimFire= Config.Bind("Anim", "Fire", new KeyboardShortcut(KeyCode.None), "开枪(对他人) (仅Chaos)");
        KeyAnimEmpty= Config.Bind("Anim", "Empty", new KeyboardShortcut(KeyCode.None), "空枪(对他人) (仅Chaos)");

        BooleanTestGiraffe = Config.Bind("Test", "Giraffe", false, "修复伸头(服务器和客户端都需要, 先开启再开始游戏)");

        intGammXP.SettingChanged += (_, _) =>
        {
            if (intGammXP.Value % 50 == 0)
            {
                PlayerLevelHelper.SetXp(intGammXP.Value);
            }
            else
            {
                intGammXP.Value = (int)((intGammXP.Value + 25f) / 50f) * 50;
            }
        };
    }
}

[Flags]
public enum RotateDirection
{
    None = 0,
    Pitch = 1,
    Yaw = 2,
    Roll = 4,
    HeadPitch = 8,
    HeadYaw = 16,
    HeadRoll = 32
}

[Flags]
public enum HintType
{
    None = 0,
    TitleName = 1,
    HintKey = 2,
    PluginInfo = 4,
    AnimKey = 8,
    DebugInfo = 16,
    All = TitleName | HintKey | PluginInfo | AnimKey | DebugInfo
}

#if CHEATRELEASE
public enum RouletType
{
    [Description("仅动画")]
    AnimOnly,
    [Description("开枪")]
    Roulet,
    [Description("自杀")]
    Suicide
}
#endif
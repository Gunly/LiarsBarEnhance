using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;

using HarmonyLib;

using LiarsBarEnhance.Features;

using System;

using UnityEngine;

namespace LiarsBarEnhance;

[BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
public class Plugin : BaseUnityPlugin
{
    internal new static ManualLogSource Logger;
    private static ConfigEntry<int> intPositionNum, intAnimationNum;
    public static int InitPositionNumValue, InitAnimationNumValue;
    public static ConfigEntry<KeyboardShortcut> KeyCustomBigMouth, KeyCustomShowHint,
        KeyMoveForward, KeyMoveBack, KeyMoveLeft, KeyMoveRight, KeyMoveJump, KeyMoveRun, KeyMoveSquat, KeyMoveResetPosition,
        KeyViewCrazyShakeHead, KeyViewRemoveRotationLimit, KeyViewForward, KeyViewBack, KeyViewLeft,
        KeyViewRight, KeyViewUp, KeyViewDown, KeyViewClockwise, KeyViewAnticlockwise,
        KeyRotateYaw, KeyRotatePitch, KeyRotateRoll, KeyRotateAuto;
    public static ConfigEntry<KeyboardShortcut>[] KeyPosition;
    public static ConfigEntry<KeyboardShortcut>[] KeyAnims;
    public static ConfigEntry<Vector3>[] VectorPosition, VectorRotation;
    public static ConfigEntry<float> FloatJumpHeight, FloatGravity, FloatMoveSpeed, FloatViewSpeed, FloatViewField,
        FloatAutoRotateSpeed, FloatCustomPlayerScale;
    public static ConfigEntry<bool> BooleanMoveFollowHead, BooleanViewRemoveRotationLimit, BooleanViewMouseViewField, BooleanResetView,
        BooleanTestGiraffe, BooleanCustomShowSelfInfo;
    public static ConfigEntry<string> StringCustomName, StringCustomNameColor, StringCustomMessageColor;
    public static ConfigEntry<string>[] StringAnims;
    public static ConfigEntry<RotateDirection> DirectionRotateState;
#if CHEATRELEASE
    public static ConfigEntry<KeyboardShortcut> KeyCheatDeckFlip, KeyCheatDeckScale, KeyCheatDiceShow;
    public static ConfigEntry<float> FloatCheatCardSize;
    public static ConfigEntry<bool> BooleanCheatCard, BooleanCheatDice;
#endif

    private void Awake()
    {
        Logger = base.Logger;
        BindConfig();

        Harmony.CreateAndPatchAll(typeof(RemoveHeadRotationlimitPatch), nameof(RemoveHeadRotationlimitPatch));
        Harmony.CreateAndPatchAll(typeof(CrazyShakeHeadPatch), nameof(CrazyShakeHeadPatch));
        Harmony.CreateAndPatchAll(typeof(ChatProPatch), nameof(ChatProPatch));
        Harmony.CreateAndPatchAll(typeof(CharMoveablePatch), nameof(CharMoveablePatch));
        Harmony.CreateAndPatchAll(typeof(BigMouthPatch), nameof(BigMouthPatch));
        Harmony.CreateAndPatchAll(typeof(ChineseNameFixPatch), nameof(ChineseNameFixPatch));
        Harmony.CreateAndPatchAll(typeof(RemoveNameLengthLimitPatch), nameof(RemoveNameLengthLimitPatch));
        Harmony.CreateAndPatchAll(typeof(SelectableLevelPatch), nameof(SelectableLevelPatch));
        Harmony.CreateAndPatchAll(typeof(CustomNamePatch), nameof(CustomNamePatch));
        Harmony.CreateAndPatchAll(typeof(ShowSelfTopInfoPatch), nameof(ShowSelfTopInfoPatch));
        Harmony.CreateAndPatchAll(typeof(DisableTmpWarningsPatch), nameof(DisableTmpWarningsPatch));
#if CHEATRELEASE
        Harmony.CreateAndPatchAll(typeof(BlorfCheatPatch), nameof(BlorfCheatPatch));
        Harmony.CreateAndPatchAll(typeof(DiceCheatPatch), nameof(DiceCheatPatch));
#else
        Harmony.CreateAndPatchAll(typeof(BlorfAntiCheat), nameof(BlorfAntiCheat));
        Harmony.CreateAndPatchAll(typeof(DiceAntiCheat), nameof(DiceAntiCheat));
#endif

        Logger.LogInfo($"Plugin {PluginInfo.PLUGIN_GUID} is loaded!");
    }

    private void BindConfig()
    {
        intPositionNum = Config.Bind("AAARepeatConfigNum", "PositionNum", 4, new ConfigDescription("可传送坐标数量(需要重启游戏)", new AcceptableValueRange<int>(0, 9)));
        intAnimationNum = Config.Bind("AAARepeatConfigNum", "AnimationNum", 0, new ConfigDescription("可使用动画数量(需要重启游戏)", new AcceptableValueRange<int>(0, 9)));
        InitPositionNumValue = intPositionNum.Value;
        InitAnimationNumValue = intAnimationNum.Value;

#if CHEATRELEASE
        BooleanCheatCard = Config.Bind("Cheat", "CheatDeck", false, "Deck模式作弊");
        BooleanCheatDice = Config.Bind("Cheat", "CheatDice", false, "Dice模式作弊");
        KeyCheatDeckFlip = Config.Bind("Cheat", "CardFlip", new KeyboardShortcut(KeyCode.LeftControl), "翻转放大其他玩家卡牌");
        FloatCheatCardSize = Config.Bind("Cheat", "CardSize", 1f, new ConfigDescription("放大大小", new AcceptableValueRange<float>(1f, 10f)));
        KeyCheatDiceShow = Config.Bind("Cheat", "DiceShow", new KeyboardShortcut(KeyCode.LeftControl), "显示其他玩家骰子");
#endif

        KeyCustomBigMouth = Config.Bind("Custom", "BigMouth", new KeyboardShortcut(KeyCode.O), "大嘴");
        KeyCustomShowHint = Config.Bind("Custom", "ShowHint", new KeyboardShortcut(KeyCode.Tab), "启用提示");
        StringCustomName = Config.Bind("Custom", "CustomName", "", "自定义名称");
        StringCustomNameColor = Config.Bind("Custom", "NameColor", "FDE2AA", "聊天名字颜色");
        StringCustomMessageColor = Config.Bind("Custom", "MessageColor", "FFFFFF", "聊天文本颜色");
        BooleanCustomShowSelfInfo = Config.Bind("Custom", "ShowSelfInfo", false, "显示自身头顶信息");
        FloatCustomPlayerScale = Config.Bind("Custom", "PlayerScale", 0.5f, new ConfigDescription("玩家缩放(自己)", new AcceptableValueRange<float>(0f, 1f)));

        KeyMoveResetPosition = Config.Bind("Move", "ResetPosition", new KeyboardShortcut(KeyCode.R), "重置坐标");
        BooleanResetView = Config.Bind("Move", "ResetView", false, "重置坐标时重置视角");
        BooleanMoveFollowHead = Config.Bind("Move", "MoveFollowHead", true, "移动方向跟随头部视角");
        KeyMoveForward = Config.Bind("Move", "BodyForward", new KeyboardShortcut(KeyCode.UpArrow), "向前移动");
        KeyMoveBack = Config.Bind("Move", "BodyBack", new KeyboardShortcut(KeyCode.DownArrow), "向后移动");
        KeyMoveLeft = Config.Bind("Move", "BodyLeft", new KeyboardShortcut(KeyCode.LeftArrow), "向左移动");
        KeyMoveRight = Config.Bind("Move", "BodyRight", new KeyboardShortcut(KeyCode.RightArrow), "向右移动");
        KeyMoveJump = Config.Bind("Move", "BodyJump", new KeyboardShortcut(KeyCode.Keypad0), "跳跃(双击进入飞行模式, 飞行模式向上移动)");
        KeyMoveRun = Config.Bind("Move", "BodyRun", new KeyboardShortcut(KeyCode.RightShift), "奔跑");
        KeyMoveSquat = Config.Bind("Move", "BodySquat", new KeyboardShortcut(KeyCode.RightControl), "蹲下(飞行模式向下移动, 同时按跳跃和蹲下退出飞行模式)");
        FloatJumpHeight = Config.Bind("Move", "JumpHeight", 0.65f, new ConfigDescription("跳跃高度", new AcceptableValueRange<float>(0f, 10f)));
        FloatGravity = Config.Bind("Move", "Gravity", 9.8f, new ConfigDescription("重力加速度(仅对跳跃生效)", new AcceptableValueRange<float>(0f, 100f)));
        FloatMoveSpeed = Config.Bind("Move", "MoveSpeed", 4f, new ConfigDescription("移动速度", new AcceptableValueRange<float>(0f, 100f)));

        KeyViewCrazyShakeHead = Config.Bind("View", "CrazyShakeHead", new KeyboardShortcut(KeyCode.I), "疯狂摇头");
        BooleanViewMouseViewField = Config.Bind("View", "MouseViewField", true, "滚轮调整视野范围");
        BooleanViewRemoveRotationLimit = Config.Bind("View", "RemoveRotationLimit", true, "移除视角限制");
        KeyViewRemoveRotationLimit = Config.Bind("View", "RemoveRotationLimitShortcut", new KeyboardShortcut(KeyCode.U), "切换移除视角限制快捷键");
        KeyViewForward = Config.Bind("View", "ViewForward", new KeyboardShortcut(KeyCode.Keypad8), "头部/视角向前移动");
        KeyViewBack = Config.Bind("View", "ViewBack", new KeyboardShortcut(KeyCode.Keypad5), "头部/视角向后移动");
        KeyViewLeft = Config.Bind("View", "ViewLeft", new KeyboardShortcut(KeyCode.Keypad4), "头部/视角向左移动");
        KeyViewRight = Config.Bind("View", "ViewRight", new KeyboardShortcut(KeyCode.Keypad6), "头部/视角向右移动");
        KeyViewUp = Config.Bind("View", "ViewUp", new KeyboardShortcut(KeyCode.Keypad7), "头部/视角向上移动");
        KeyViewDown = Config.Bind("View", "ViewDown", new KeyboardShortcut(KeyCode.Keypad1), "头部/视角向下移动");
        KeyViewClockwise = Config.Bind("View", "ViewClockwise", new KeyboardShortcut(KeyCode.Keypad3), "视角顺时针旋转");
        KeyViewAnticlockwise = Config.Bind("View", "ViewAnticlockwise", new KeyboardShortcut(KeyCode.Keypad2), "视角逆时针旋转");
        FloatViewSpeed = Config.Bind("View", "ViewSpeed", 3f, new ConfigDescription("视角移动速度", new AcceptableValueRange<float>(0f, 100f)));
        FloatViewField = Config.Bind("View", "ViewField", 60f, new ConfigDescription("视野范围", new AcceptableValueRange<float>(1f, 180f)));

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
            new(0.36f, 0.3f, -8.46f),
            new(0f, 0f, 0f),
            new(0f, 0f, 0f),
            new(0f, 0f, 0f),
            new(0f, 0f, 0f)
            ];
            Vector3[] DefaultRotations = [
                new(0f, 0f, 0f),
            new(0f, 270f, 0f),
            new(0f, 180f, 0f),
            new(0f, 90f, 0f),
            new(0f, 0f, 0f),
            new(0f, 0f, 0f),
            new(0f, 0f, 0f),
            new(0f, 0f, 0f),
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

        BooleanTestGiraffe = Config.Bind("Test", "Giraffe", false, "修复伸头(服务器和客户端都需要, 先开启再开始游戏)");
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
    HeadYaw = 16
}
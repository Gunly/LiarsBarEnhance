using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using BepInEx.Unity.IL2CPP;

using HarmonyLib;

using LiarsBarEnhance.Features;
using LiarsBarEnhance.Utils;

using System;
using System.Linq;
using System.Text.RegularExpressions;

using UnityEngine;
using UnityEngine.Localization.Settings;
using UnityEngine.SceneManagement;

namespace LiarsBarEnhance;

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
public class Plugin : BasePlugin
{
    internal new static ManualLogSource Log;
    private static bool bind = false, isChinese;
    public static ConfigEntry<int> IntPositionNum, IntAnimationNum, IntGammXP, IntHintPosX, IntHintPosY, IntDefaultSkin;
    public static ConfigEntry<KeyboardShortcut> KeyCustomBigMouth, KeyGameShowHint,
        KeyAnimCallLiar, KeyAnimDrink, KeyAnimReload, KeyAnimRoulet, KeyAnimShake, KeyAnimShow,
        KeyAnimSpotOn, KeyAnimThrow, KeyAnimTakeAim, KeyAnimFire, KeyAnimEmpty,
        KeyMoveForward, KeyMoveBack, KeyMoveLeft, KeyMoveRight, KeyMoveJump, KeyMoveSquat, KeyMoveResetPosition, KeyMoveFollowHeadShortcut,
        KeyViewCrazyShakeHead, KeyViewRemoveRotationLimit, KeyViewReset, KeyViewField,
        KeyViewForward, KeyViewBack, KeyViewLeft, KeyViewRight, KeyViewUp, KeyViewDown, KeyViewClockwise, KeyViewAnticlockwise,
        KeyRotateYaw, KeyRotatePitch, KeyRotateRoll, KeyRotateAuto,
        KeyGameReturnMenu;
    public static ConfigEntry<KeyboardShortcut>[] KeyPosition, KeyAnims;
    public static ConfigEntry<Vector3>[] VectorPosition, VectorRotation;
    public static ConfigEntry<string>[] StringAnims;
    public static ConfigEntry<float> FloatJumpHeight, FloatGravity, FloatMoveSpeed, FloatMoveHorizontalBodyRotate, FloatViewSpeed, FloatViewField,
        FloatAutoRotateSpeed, FloatCustomPlayerScale, FloatBigMouthAngle;
    public static ConfigEntry<bool> BooleanMoveFollowHead, BooleanViewRemoveRotationLimit, BooleanViewField,
        BooleanTestGiraffe, BooleanCustomShowSelfInfo, BooleanGameAutoReady;
    public static ConfigEntry<string> StringCustomName, StringGameLobbyFilterWords;
    public static ConfigEntry<RotateDirection> DirectionRotateState;
    public static ConfigEntry<HintType> HintTypeSelect;
    public static ConfigEntry<SkinName> DefaultSkin;
#if CHEATRELEASE
    public static ConfigEntry<bool> BooleanCheatDeck, BooleanCheatDice, BooleanCheatDeckHealth, BooleanCheatBlorfLastRoundCard, BooleanCheatDiceTotalDice;
    public static ConfigEntry<KeyboardShortcut> KeyCheatBlorfFlip, KeyCheatDiceShow, KeyAnimRouletType;
    public static ConfigEntry<KeyboardShortcut>[] KeyCheatChangeCardDice;
    public static ConfigEntry<float> FloatCheatCardSize;
    public static ConfigEntry<int> IntCheatBlorfHealth, IntCheatBlorfRevoler;
    public static ConfigEntry<RouletType> RouletAnimType;
#endif

    public override void Load()
    {
        LiarsBarEnhanceBehaviour.Plugin = this;
        IL2CPPChainloader.AddUnityComponent(typeof(LiarsBarEnhanceBehaviour));
    }
    private class LiarsBarEnhanceBehaviour : MonoBehaviour
    {
        internal static Plugin Plugin;

        private void Start() => Plugin.Start();
        private void Update() => Plugin.Update();
        //private void LateUpdate() => Plugin.LateUpdate();
        private void OnGUI() => Plugin.OnGUI();
    }
    private void Start()
    {
        TomlTypeConverter.AddConverter(typeof(Vector3), new TypeConverter
        {
            ConvertToString = (o, type) => ((Vector3)o).ToString(),
            ConvertToObject = (s, type) =>
            {
                var pattern = @"(-?\d+)(\.\d+)?";
                var numbers = Regex.Matches(s, pattern).Select((Match m) => float.Parse(m.Value)).ToList();
                if (numbers.Count < 3) return Vector3.zero;
                return new Vector3(numbers[0], numbers[1], numbers[2]);
            }
        });
        LocalizationSettings.add_SelectedLocaleChanged((Il2CppSystem.Action<UnityEngine.Localization.Locale>)SelectedLocaleChanged);

        PatchAll();
        Log = base.Log;
        Log.LogInfo($"Plugin {MyPluginInfo.PLUGIN_GUID} is loaded!");
    }

    private void Update()
    {
        if (!bind && LocalizationSettings.SelectedLocaleAsync.IsDone)
            SelectedLocaleChanged(LocalizationSettings.SelectedLocale);
        if (bind && KeyGameReturnMenu.IsDown())
            SceneManager.LoadScene("SteamTest");
    }

    private void OnGUI()
    {
        HintPatch.OnGUI();
    }

    private void SelectedLocaleChanged(UnityEngine.Localization.Locale locale)
    {
        var b = LocalizationSettings.SelectedLocale.LocaleName == "Chinese (Simplified) (zh-Hans)";
        if (bind && isChinese == b) return;
        isChinese = b;
        Config.Clear();
        Config.Reload();
        if (isChinese) BindConfig();
        else BindConfigEnglish();
        IntPositionNum.SettingChanged += (_, _) =>
        {
            Array.ForEach(KeyPosition, e => Config.Remove(e.Definition));
            Array.ForEach(VectorPosition, e => Config.Remove(e.Definition));
            Array.ForEach(VectorRotation, e => Config.Remove(e.Definition));
            Config.Reload();
            if (isChinese) BindConfigPosition();
            else BindConfigPositionEnglish();
        };
        IntAnimationNum.SettingChanged += (_, _) =>
        {
            Array.ForEach(KeyAnims, e => Config.Remove(e.Definition));
            Array.ForEach(StringAnims, e => Config.Remove(e.Definition));
            Config.Reload();
            if (isChinese) BindConfigAnim();
            else BindConfigAnimEnglish();
        };
        IntGammXP.SettingChanged += (_, _) =>
        {
            if (IntGammXP.Value % 50 == 0)
                PlayerLevelHelper.SetXp(IntGammXP.Value);
            else
                IntGammXP.Value = (int)((IntGammXP.Value + 25f) / 50f) * 50;
        };
        DefaultSkin.SettingChanged += (_, _) => IntDefaultSkin.Value = (int)DefaultSkin.Value;
        IntDefaultSkin.SettingChanged += (_, _) => DefaultSkin.Value = (SkinName)IntDefaultSkin.Value;
        FloatBigMouthAngle.SettingChanged += (_, _) => BigMouthPatch.MouthOpen = 1f;
        StringGameLobbyFilterWords.SettingChanged += (_, _) => LobbyFilterPatch.GetFilterWords();
        LobbyFilterPatch.GetFilterWords();
        IntHintPosX.SettingChanged += (_, _) => HintPatch.GuiShow = 1f;
        IntHintPosY.SettingChanged += (_, _) => HintPatch.GuiShow = 1f;
        HintTypeSelect.SettingChanged += (_, _) => HintPatch.GuiShow = 1f;
        HintPatch.SelectedLocaleChanged(isChinese);
        bind = true;
    }

    private void PatchAll()
    {
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
        //Harmony.CreateAndPatchAll(typeof(GiraffePatch), nameof(GiraffePatch));//TODO
        Harmony.CreateAndPatchAll(typeof(LobbyFilterPatch), nameof(LobbyFilterPatch));
        Harmony.CreateAndPatchAll(typeof(HintPatch), nameof(HintPatch));
        Harmony.CreateAndPatchAll(typeof(DefaultSkinPatch), nameof(DefaultSkinPatch));
        Harmony.CreateAndPatchAll(typeof(AutoReadyPatch), nameof(AutoReadyPatch));

#if CHEATRELEASE
        Harmony.CreateAndPatchAll(typeof(BlorfCheatPatch), nameof(BlorfCheatPatch));
        Harmony.CreateAndPatchAll(typeof(DiceCheatPatch), nameof(DiceCheatPatch));
        Harmony.CreateAndPatchAll(typeof(ChaosCheatPatch), nameof(ChaosCheatPatch));
        Harmony.CreateAndPatchAll(typeof(BlorfMatchMakingCheatPatch), nameof(BlorfMatchMakingCheatPatch));
#else
        Harmony.CreateAndPatchAll(typeof(DicePatch), nameof(DicePatch));
#endif
    }

    private void BindConfig()
    {
        IntPositionNum = Config.Bind("AAARepeatConfigNum", "PositionNum", 4, new ConfigDescription("可传送坐标数量", new AcceptableValueRange<int>(0, 9)));
        IntAnimationNum = Config.Bind("AAARepeatConfigNum", "AnimationNum", 0, new ConfigDescription("可使用动画数量", new AcceptableValueRange<int>(0, 9)));
        BindConfigPosition();
        BindConfigAnim();

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
        DefaultSkin = Config.Bind("Custom", "DefaultSkin", SkinName.Scubby, "默认角色，进入房间时自动选择");
        IntDefaultSkin = Config.Bind("Custom", "DefaultSkinInt", 0, new ConfigDescription("如果下拉框无法使用，用这个", new AcceptableValueRange<int>(0, Enum.GetValues<SkinName>().Length - 1)));

        KeyGameShowHint = Config.Bind("Game", "HintShow", new KeyboardShortcut(KeyCode.Tab), "启用提示");
        KeyMoveResetPosition = Config.Bind("Game", "ResetPosition", new KeyboardShortcut(KeyCode.R), "重置坐标");
        KeyViewReset = Config.Bind("Game", "ResetView", new KeyboardShortcut(KeyCode.T), "重置视角");
        HintTypeSelect = Config.Bind("Game", "HintType", HintType.All, "选择需要的提示信息");
        IntHintPosX = Config.Bind("Game", "HintPosX", -300, new ConfigDescription("提示坐标X, 负数表示以屏幕宽度减去设置值", new AcceptableValueRange<int>(-2000, 2000)));
        IntHintPosY = Config.Bind("Game", "HintPosY", 60, new ConfigDescription("提示坐标Y, 负数表示以屏幕高度减去设置值", new AcceptableValueRange<int>(-1000, 1000)));
        KeyGameReturnMenu = Config.Bind("Game", "ReturnMenu", new KeyboardShortcut(KeyCode.F9), "回到主菜单(解决卡死)");
        IntGammXP = Config.Bind("Game", "XP", 0, new ConfigDescription("经验值", new AcceptableValueRange<int>(0, 10000)));
        StringGameLobbyFilterWords = Config.Bind("Game", "LobbyFilterWords", "透视|改牌|透牌|修改|枪数|无敌|低价|稳定|免费|加q|加群|售后|看片|网址|国产|少妇|" +
            "@[Qq]\\d{5,}|@[a-zA-Z0-9]{3,}\\.[a-zA-Z]{2,}", "大厅过滤词");
        BooleanGameAutoReady = Config.Bind("Game", "AutoReady", false, "自动准备");

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
        KeyViewField = Config.Bind("View", "ViewFieldKey", new KeyboardShortcut(KeyCode.Mouse2), "切换滚轮调整视场角快捷键");
        BooleanViewRemoveRotationLimit = Config.Bind("View", "RemoveRotationLimit", true, "移除视角限制");
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
        KeyAnimTakeAim = Config.Bind("Anim", "Aim", new KeyboardShortcut(KeyCode.None), "举枪 (对他人) (仅Chaos)");
        KeyAnimFire = Config.Bind("Anim", "Fire", new KeyboardShortcut(KeyCode.None), "开枪 (对他人) (仅Chaos)");
        KeyAnimEmpty = Config.Bind("Anim", "Empty", new KeyboardShortcut(KeyCode.None), "空枪 (对他人) (仅Chaos)");

        //BooleanTestGiraffe = Config.Bind("Test", "Giraffe", false, "修复伸头(服务器和客户端都需要, 先开启再开始游戏)");
    }

    private void BindConfigPosition()
    {
        KeyPosition = new ConfigEntry<KeyboardShortcut>[IntPositionNum.Value];
        VectorPosition = new ConfigEntry<Vector3>[IntPositionNum.Value];
        VectorRotation = new ConfigEntry<Vector3>[IntPositionNum.Value];
        Vector3[] DefaultPositions = [
            new(0.36f, 0.3f, -9.79f),
                new(1.69f, 0.3f, -8.46f),
                new(0.36f, 0.3f, -7.13f),
                new(-0.97f, 0.3f, -8.46f),
                new(16.26f, 0.39f, -37.49f),
                new(17.59f, 0.39f, -36.16f),
                new(16.26f, 0.39f, -34.83f),
                new(14.93f, 0.39f, -36.16f),
                new(0f, 0f, 0f)
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
        for (var i = 0; i < IntPositionNum.Value; i++)
        {
            KeyPosition[i] = Config.Bind("Position", $"Position{i + 1}", new KeyboardShortcut(KeyCode.Alpha1 + i), $"移动到坐标{i + 1}");
            VectorPosition[i] = Config.Bind("Position", $"Position{i + 1}Position", DefaultPositions[i], $"坐标{i + 1}坐标");
            VectorRotation[i] = Config.Bind("Position", $"Position{i + 1}Rotation", DefaultRotations[i], $"坐标{i + 1}角度");
        }
    }

    private void BindConfigAnim()
    {
        KeyAnims = new ConfigEntry<KeyboardShortcut>[IntAnimationNum.Value];
        StringAnims = new ConfigEntry<string>[IntAnimationNum.Value];
        for (var i = 0; i < IntAnimationNum.Value; i++)
        {
            KeyAnims[i] = Config.Bind("Anim", $"Anim{i + 1}", new KeyboardShortcut(KeyCode.None), $"动画{i + 1}快捷键");
            StringAnims[i] = Config.Bind("Anim", $"Anim{i + 1}Path", "", $"动画{i + 1}路径");
        }
    }

    private void BindConfigEnglish()
    {
        IntPositionNum = Config.Bind("AAARepeatConfigNum", "PositionNum", 4, new ConfigDescription("Number of transferable coordinates", new AcceptableValueRange<int>(0, 9)));
        IntAnimationNum = Config.Bind("AAARepeatConfigNum", "AnimationNum", 0, new ConfigDescription("Number of animations available", new AcceptableValueRange<int>(0, 9)));
        BindConfigPositionEnglish();
        BindConfigAnimEnglish();

#if CHEATRELEASE
        BooleanCheatDeck = Config.Bind("Cheat", "CheatDeck", false, "Deck mode cheating");
        BooleanCheatDice = Config.Bind("Cheat", "CheatDice", false, "Dice mode cheating");
        KeyCheatBlorfFlip = Config.Bind("Cheat", "CardFlip", new KeyboardShortcut(KeyCode.LeftControl), "Flip and enlarge other players' cards");
        KeyCheatDiceShow = Config.Bind("Cheat", "DiceShow", new KeyboardShortcut(KeyCode.LeftControl), "Display dice from other players");
        FloatCheatCardSize = Config.Bind("Cheat", "CardSize", 1f, new ConfigDescription("Enlarge size", new AcceptableValueRange<float>(1f, 10f)));
        IntCheatBlorfHealth = Config.Bind("Cheat", "DeckHealth", 6, new ConfigDescription("Deck mode health (changes take effect after starting the game)", new AcceptableValueRange<int>(1, 50)));
        IntCheatBlorfRevoler = Config.Bind("Cheat", "DeckRevoler", 0, new ConfigDescription("Number of shots fired", new AcceptableValueRange<int>(0, 49)));
        BooleanCheatDeckHealth = Config.Bind("Cheat", "ShowHealth", false, "Display health (which live fire was fired)");
        BooleanCheatBlorfLastRoundCard = Config.Bind("Cheat", "ShowRoundCard", false, "Display current card play (prompt GUI)");
        BooleanCheatDiceTotalDice = Config.Bind("Cheat", "ShowTotalDice", false, "Display the total number of dice (prompt GUI)");
        KeyCheatChangeCardDice = new ConfigEntry<KeyboardShortcut>[5];
        for (var i = 0; i < KeyCheatChangeCardDice.Length; i++)
        {
            KeyCheatChangeCardDice[i] = Config.Bind("Cheat", $"Change{i + 1}", new KeyboardShortcut(KeyCode.None), $"Change the {i + 1} th card/dice");
        }
#endif

        KeyCustomBigMouth = Config.Bind("Custom", "BigMouth", new KeyboardShortcut(KeyCode.B), "Big mouth");
        FloatBigMouthAngle = Config.Bind("Custom", "BigMouthAngle", 60f, new ConfigDescription("Opening angle", new AcceptableValueRange<float>(-180f, 180f)));
        StringCustomName = Config.Bind("Custom", "CustomName", "", "Custom name");
        BooleanCustomShowSelfInfo = Config.Bind("Custom", "ShowSelfInfo", true, "Display personal overhead information");
        FloatCustomPlayerScale = Config.Bind("Custom", "PlayerScale", 0.5f, new ConfigDescription("Player Scale", new AcceptableValueRange<float>(0f, 1f)));
        DefaultSkin = Config.Bind("Custom", "DefaultSkin", SkinName.Scubby, "Default role, automatically selected when entering the room");
        IntDefaultSkin = Config.Bind("Custom", "DefaultSkinInt", 0, new ConfigDescription("If the dropdown menu cannot be used, use this", new AcceptableValueRange<int>(0, Enum.GetValues<SkinName>().Length - 1)));

        KeyGameShowHint = Config.Bind("Game", "HintShow", new KeyboardShortcut(KeyCode.Tab), "Enable prompt");
        KeyMoveResetPosition = Config.Bind("Game", "ResetPosition", new KeyboardShortcut(KeyCode.R), "Reset coordinates");
        KeyViewReset = Config.Bind("Game", "ResetView", new KeyboardShortcut(KeyCode.T), "Reset perspective");
        HintTypeSelect = Config.Bind("Game", "HintType", HintType.All, "Select the required prompt information");
        IntHintPosX = Config.Bind("Game", "HintPosX", -300, new ConfigDescription("Prompt coordinate X, negative number represents subtracting the set value from the screen width", new AcceptableValueRange<int>(-2000, 2000)));
        IntHintPosY = Config.Bind("Game", "HintPosY", 60, new ConfigDescription("Prompt coordinate Y, negative number represents subtracting the set value from the screen height", new AcceptableValueRange<int>(-1000, 1000)));
        KeyGameReturnMenu = Config.Bind("Game", "ReturnMenu", new KeyboardShortcut(KeyCode.F9), "Return to the main menu (resolve stuck)");
        IntGammXP = Config.Bind("Game", "XP", 0, new ConfigDescription("Experience value", new AcceptableValueRange<int>(0, 10000)));
        StringGameLobbyFilterWords = Config.Bind("Game", "LobbyFilterWords", "透视|改牌|透牌|修改|枪数|无敌|低价|稳定|免费|加q|加群|售后|看片|网址|国产|少妇|" +
            "@[Qq]\\d{5,}|@[a-zA-Z0-9]{3,}\\.[a-zA-Z]{2,}", "Lobby filtering keywords");
        BooleanGameAutoReady = Config.Bind("Game", "AutoReady", false, "Automatic preparation");

        BooleanMoveFollowHead = Config.Bind("Move", "MoveFollowHead", true, "Movement direction follows the perspective of the head");
        KeyMoveFollowHeadShortcut = Config.Bind("Move", "MoveFollowHeadShortcut", new KeyboardShortcut(KeyCode.H), "Quick key to switch movement direction and follow head view angle");
        FloatMoveHorizontalBodyRotate = Config.Bind("Move", "MoveHorizontalBodyRotate", 0f, new ConfigDescription("Body rotation angle when moving left and right", new AcceptableValueRange<float>(0f, 90f)));
        KeyMoveForward = Config.Bind("Move", "BodyForward", new KeyboardShortcut(KeyCode.UpArrow), "Move Forward");
        KeyMoveBack = Config.Bind("Move", "BodyBack", new KeyboardShortcut(KeyCode.DownArrow), "Move back");
        KeyMoveLeft = Config.Bind("Move", "BodyLeft", new KeyboardShortcut(KeyCode.LeftArrow), "Move left");
        KeyMoveRight = Config.Bind("Move", "BodyRight", new KeyboardShortcut(KeyCode.RightArrow), "Move right");
        KeyMoveJump = Config.Bind("Move", "BodyJump", new KeyboardShortcut(KeyCode.Keypad0), "Jump (double-click to enter airplane mode, airplane mode moves up)");
        KeyMoveSquat = Config.Bind("Move", "BodySquat", new KeyboardShortcut(KeyCode.RightControl), "Squat down (move down in airplane mode while pressing jump and squat to exit airplane mode)");
        FloatJumpHeight = Config.Bind("Move", "JumpHeight", 0.65f, new ConfigDescription("jump height", new AcceptableValueRange<float>(0f, 10f)));
        FloatGravity = Config.Bind("Move", "Gravity", 9.8f, new ConfigDescription("Gravity acceleration (only applicable to jumps)", new AcceptableValueRange<float>(0f, 100f)));
        FloatMoveSpeed = Config.Bind("Move", "MoveSpeed", 4f, new ConfigDescription("movement speed", new AcceptableValueRange<float>(0f, 100f)));

        KeyViewCrazyShakeHead = Config.Bind("View", "CrazyShakeHead", new KeyboardShortcut(KeyCode.C), "Crazy shaking head");
        BooleanViewField = Config.Bind("View", "MouseViewField", true, "Roller adjusts field of view angle");
        FloatViewField = Config.Bind("View", "ViewField", 60f, new ConfigDescription("viewing angle", new AcceptableValueRange<float>(1f, 180f)));
        KeyViewField = Config.Bind("View", "ViewFieldKey", new KeyboardShortcut(KeyCode.Mouse2), "Quick key to switch the scroll wheel to adjust the field of view angle");
        BooleanViewRemoveRotationLimit = Config.Bind("View", "RemoveRotationLimit", true, "Remove viewpoint restrictions");
        KeyViewRemoveRotationLimit = Config.Bind("View", "RemoveRotationLimitShortcut", new KeyboardShortcut(KeyCode.G), "Shortcut key for switching and removing viewpoint restrictions");
        KeyViewForward = Config.Bind("View", "ViewForward", new KeyboardShortcut(KeyCode.I), "Head/viewpoint moving forward");
        KeyViewBack = Config.Bind("View", "ViewBack", new KeyboardShortcut(KeyCode.K), "Head/viewpoint moving backward");
        KeyViewLeft = Config.Bind("View", "ViewLeft", new KeyboardShortcut(KeyCode.J), "Head/viewpoint moving left");
        KeyViewRight = Config.Bind("View", "ViewRight", new KeyboardShortcut(KeyCode.L), "Head/viewpoint moving right");
        KeyViewUp = Config.Bind("View", "ViewUp", new KeyboardShortcut(KeyCode.O), "Head/viewpoint moving upwards");
        KeyViewDown = Config.Bind("View", "ViewDown", new KeyboardShortcut(KeyCode.U), "Head/viewpoint moving downwards");
        KeyViewClockwise = Config.Bind("View", "ViewClockwise", new KeyboardShortcut(KeyCode.RightBracket), "Head/viewpoint clockwise rotation");
        KeyViewAnticlockwise = Config.Bind("View", "ViewAnticlockwise", new KeyboardShortcut(KeyCode.LeftBracket), "Head/viewpoint counterclockwise rotation");
        FloatViewSpeed = Config.Bind("View", "ViewSpeed", 3f, new ConfigDescription("Perspective movement speed", new AcceptableValueRange<float>(0f, 100f)));

        KeyRotateYaw = Config.Bind("Rotate", "RotateYaw", new KeyboardShortcut(KeyCode.Mouse1), "Horizontal rotation (Yaw angle, hold down and move the mouse)");
        KeyRotatePitch = Config.Bind("Rotate", "RotatePitch", new KeyboardShortcut(KeyCode.Mouse2), "Vertical rotation (Pitch angle, hold down and move the mouse)");
        KeyRotateRoll = Config.Bind("Rotate", "RotateRoll", new KeyboardShortcut(KeyCode.Mouse0), "Vertical rotation (Roll angle, hold down and move the mouse)");
        KeyRotateAuto = Config.Bind("Rotate", "AutoRotate", new KeyboardShortcut(KeyCode.P), "Automatic rotation");
        DirectionRotateState = Config.Bind("Rotate", "RotateState", RotateDirection.Yaw, "Automatic rotation direction");
        FloatAutoRotateSpeed = Config.Bind("Rotate", "AutoRotateSpeed", 0.2f, new ConfigDescription("Automatic rotation speed (revolutions per second)", new AcceptableValueRange<float>(-10f, 10f)));

        KeyAnimCallLiar = Config.Bind("Anim", "CallLiar", new KeyboardShortcut(KeyCode.None), "Call Liar (Available when looking at cards in Deck mode) (All modes)");
        KeyAnimSpotOn = Config.Bind("Anim", "SpotOn", new KeyboardShortcut(KeyCode.None), "Spot On (Only Dice)");
        KeyAnimThrow = Config.Bind("Anim", "Throw", new KeyboardShortcut(KeyCode.None), "Throw Card (Press and hold while looking at the cards)(Deck/Chaos)");
        KeyAnimShow = Config.Bind("Anim", "Show", new KeyboardShortcut(KeyCode.None), "Show Dice (Hold)(Only Dice)");
        KeyAnimRoulet = Config.Bind("Anim", "Roulet", new KeyboardShortcut(KeyCode.None), "Roulet (press to lift the gun, release to shoot)(Deck/Chaos)");
#if CHEATRELEASE
        RouletAnimType = Config.Bind("Anim", "RouletType", RouletType.AnimOnly, "Shooting animation effect\n(If it's not AnimOnly, Chaos mode is equivalent to shooting an empty gun when dealing with others)");
        KeyAnimRouletType = Config.Bind("Anim", "RouletTypeChange", new KeyboardShortcut(KeyCode.None), "Shortcut key for switching shooting animation effects");
#endif
        KeyAnimDrink = Config.Bind("Anim", "Drink", new KeyboardShortcut(KeyCode.None), "Drink (Only Dice)");
        KeyAnimReload = Config.Bind("Anim", "Reload", new KeyboardShortcut(KeyCode.None), "Reload (Hold, Can be used when there are no cards in hand)(Deck/Chaos)");
        KeyAnimShake = Config.Bind("Anim", "Shake", new KeyboardShortcut(KeyCode.None), "Shake (Only Dice)");
        KeyAnimTakeAim = Config.Bind("Anim", "Aim", new KeyboardShortcut(KeyCode.None), "Aim (To others) (Only Chaos)");
        KeyAnimFire = Config.Bind("Anim", "Fire", new KeyboardShortcut(KeyCode.None), "Fire (To others) (Only Chaos)");
        KeyAnimEmpty = Config.Bind("Anim", "Empty", new KeyboardShortcut(KeyCode.None), "Empty (To others) (Only Chaos)");

        //BooleanTestGiraffe = Config.Bind("Test", "Giraffe", false, "Fix the head extension (required for both server and client, start the game first)");
    }

    private void BindConfigPositionEnglish()
    {
        if (IntPositionNum.Value > 0)
        {
            KeyPosition = new ConfigEntry<KeyboardShortcut>[IntPositionNum.Value];
            VectorPosition = new ConfigEntry<Vector3>[IntPositionNum.Value];
            VectorRotation = new ConfigEntry<Vector3>[IntPositionNum.Value];
            Vector3[] DefaultPositions = [
                new(0.36f, 0.3f, -9.79f),
                new(1.69f, 0.3f, -8.46f),
                new(0.36f, 0.3f, -7.13f),
                new(-0.97f, 0.3f, -8.46f),
                new(16.26f, 0.39f, -37.49f),
                new(17.59f, 0.39f, -36.16f),
                new(16.26f, 0.39f, -34.83f),
                new(14.93f, 0.39f, -36.16f),
                new(0f, 0f, 0f)
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
            for (var i = 0; i < IntPositionNum.Value; i++)
            {
                KeyPosition[i] = Config.Bind("Position", $"Position{i + 1}", new KeyboardShortcut(KeyCode.Alpha1 + i), $"Move to coordinate {i + 1}");
                VectorPosition[i] = Config.Bind("Position", $"Position{i + 1}Position", DefaultPositions[i], $"Coordinates of {i + 1}");
                VectorRotation[i] = Config.Bind("Position", $"Position{i + 1}Rotation", DefaultRotations[i], $"Angle of coordinates {i + 1}");
            }
        }
    }

    private void BindConfigAnimEnglish()
    {
        if (IntAnimationNum.Value > 0)
        {
            KeyAnims = new ConfigEntry<KeyboardShortcut>[IntAnimationNum.Value];
            StringAnims = new ConfigEntry<string>[IntAnimationNum.Value];
            for (var i = 0; i < IntAnimationNum.Value; i++)
            {
                KeyAnims[i] = Config.Bind("Anim", $"Anim{i + 1}", new KeyboardShortcut(KeyCode.None), $"Animation {i + 1} shortcut key");
                StringAnims[i] = Config.Bind("Anim", $"Anim{i + 1}Path", "", $"Animation {i + 1} Path");
            }
        }
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
    AnimOnly,
    Roulet,
    Suicide
}
#endif

public enum SkinName
{
    Scubby = 0,
    Foxy = 1,
    Bristle = 2,
    Toar = 3,
    Cupcake = 4,
    Gerk = 5,
    Kudo = 6
}

using BepInEx.Configuration;

using HarmonyLib;

using System.Linq;
using System.Text;

using UnityEngine;

namespace LiarsBarEnhance.Features;

[HarmonyPatch]
public class HintPatch
{
    public static float GuiShow { get; set; }
    private static CharController charController;
    private static bool isChinese;
    private static string hintTitle;
    private static string on;
    private static string off;

    [HarmonyPatch(typeof(CharController), "Start")]
    [HarmonyPostfix]
    public static void StartPostfix(CharController __instance)
    {
        if (!__instance.isOwned) return;
        charController = __instance;
        hintTitle = HintTitle(__instance);
    }

    public static void SelectedLocaleChanged(bool isChinese)
    {
        HintPatch.isChinese = isChinese;
        if (isChinese)
        {
            on = "<color=lime>开</color>";
            off = "<color=red>关</color>";
        }
        else
        {
            on = "<color=lime>On</color>";
            off = "<color=red>Off</color>";
        }
    }

    public static void OnGUI()
    {
        if (charController && (Plugin.KeyGameShowHint.IsPressed() || GuiShow > 0f))
        {
            if (GuiShow > 0) GuiShow -= Time.deltaTime;
            var x = Plugin.IntHintPosX.Value;
            if (x < 0) x += Screen.width;
            var y = Plugin.IntHintPosY.Value;
            if (y < 0) y += Screen.height;
            GUI.Label(new Rect(x, y, 240, 480), isChinese ? buildHint() : buildHintEnglish(),
                new GUIStyle
                {
                    fontSize = 15,
                    normal = { textColor = Color.white }
                }
            );
        }
    }

    private static string HintTitle(CharController charController)
    {
        var mode = $"<color=#F5E37B>{charController.manager().mode}</color>";
        string ruleSet;
        if (charController.manager().mode == CustomNetworkManager.GameMode.LiarsDeck)
        {
            if (charController.playerStats().GetComponent<BlorfGamePlay>())
            {
                if (charController.manager().BlorfGame.DeckMode == BlorfGamePlayManager.deckmode.Basic)
                    ruleSet = $"<color=#3487AB>{BlorfGamePlayManager.deckmode.Basic}</color>";
                else
                    ruleSet = $"<color=#CD2C48>{BlorfGamePlayManager.deckmode.Devil}</color>";
            }
            else
            {
                ruleSet = $"<color=#FFFF84>MatchMaking</color>";
            }
        }
        else if (charController.manager().mode == CustomNetworkManager.GameMode.LiarsDice)
        {
            if (charController.manager().DiceGame.DiceMode == DiceGamePlayManager.dicemode.Basic)
                ruleSet = $"<color=#3487AB>{DiceGamePlayManager.dicemode.Basic}</color>";
            else
                ruleSet = $"<color=#DFAF4A>{DiceGamePlayManager.dicemode.Traditional}</color>";
        }
        else if (charController.manager().mode == CustomNetworkManager.GameMode.LiarsChaos)
        {
            return $"<color=#F5E37B>{CustomNetworkManager.GameMode.LiarsDeck}</color> - <color=#FF82FF>Chaos</color>";
        }
        else
        {
            ruleSet = "Unknow";
        }
        return $"{mode} - {ruleSet}";
    }

    private static string buildHint()
    {
        var sb = new StringBuilder();
        if ((Plugin.HintTypeSelect.Value & HintType.TitleName) != HintType.None)
        {
            sb.AppendLine(hintTitle);
            sb.AppendLine($"{charController.playerStats().PlayerName}{HintHealth()}");
            sb.AppendLine();
        }
        if ((Plugin.HintTypeSelect.Value & HintType.HintKey) != HintType.None)
        {
            sb.AppendLine($"显示提示: {HintKey(Plugin.KeyGameShowHint)}");
            sb.AppendLine($"疯狂转头: {HintKey(Plugin.KeyViewCrazyShakeHead)}");
            sb.AppendLine($"张嘴: {HintKey(Plugin.KeyCustomBigMouth)}");
            sb.AppendLine($"重置位置: {HintKey(Plugin.KeyMoveResetPosition)}");
            sb.AppendLine($"重置视角: {HintKey(Plugin.KeyViewReset)}");
            sb.AppendLine($"身体移动: {HintKey(Plugin.KeyMoveForward, Plugin.KeyMoveBack, Plugin.KeyMoveLeft, Plugin.KeyMoveRight)}");
            sb.AppendLine($"身体转动: {HintKey(Plugin.KeyRotateYaw, Plugin.KeyRotateRoll, Plugin.KeyRotatePitch)}");
            sb.AppendLine($"头部移动: {HintKey(Plugin.KeyViewForward, Plugin.KeyViewBack, Plugin.KeyViewLeft, Plugin.KeyViewRight, Plugin.KeyViewUp, Plugin.KeyViewDown)}");
            sb.AppendLine($"头部偏转: {HintKey(Plugin.KeyViewAnticlockwise, Plugin.KeyViewClockwise)}");
            sb.AppendLine($"跳跃: {HintKey(Plugin.KeyMoveJump)}");
            sb.AppendLine($"蹲下: {HintKey(Plugin.KeyMoveSquat)}");
            sb.AppendLine($"传送: {HintKey(Plugin.KeyPosition)}");
            sb.AppendLine();
        }
        if ((Plugin.HintTypeSelect.Value & HintType.PluginInfo) != HintType.None)
        {
            sb.AppendLine($"自动旋转({HintKey(Plugin.KeyRotateAuto)}): {(AutoRotatePatch.Rotating ? on : off)}");
            sb.AppendLine($"解除视角限制({HintKey(Plugin.KeyViewRemoveRotationLimit)}): {(Plugin.BooleanViewRemoveRotationLimit.Value ? on : off)}");
            sb.AppendLine($"移动方向跟随头部视角({HintKey(Plugin.KeyMoveFollowHeadShortcut)}): {(Plugin.BooleanMoveFollowHead.Value ? on : off)}");
            sb.AppendLine($"调整视场({HintKey(Plugin.KeyViewField)}, {FOVPatch.Fov:0.00}): {(Plugin.BooleanViewField.Value ? on : off)}");
            sb.AppendLine($"显示自身头顶信息: {(Plugin.BooleanCustomShowSelfInfo.Value ? on : off)}");
            sb.AppendLine();
        }
        if ((Plugin.HintTypeSelect.Value & HintType.AnimKey) != HintType.None)
        {
            sb.AppendLine($"Liar: {HintKey(Plugin.KeyAnimCallLiar)}  SpotOn: {HintKey(Plugin.KeyAnimSpotOn)}");
            sb.AppendLine($"扔牌: {HintKey(Plugin.KeyAnimThrow)}  开盅: {HintKey(Plugin.KeyAnimShow)}");
#if CHEATRELEASE
            sb.Append($"");
            sb.AppendLine($"自枪: {HintKey(Plugin.KeyAnimRoulet)}  喝酒: {HintKey(Plugin.KeyAnimDrink)}  " +
                $"类型({HintKey(Plugin.KeyAnimRouletType)}): {Plugin.RouletAnimType.Value}");
#else
            sb.AppendLine($"自枪: {HintKey(Plugin.KeyAnimRoulet)}  喝酒: {HintKey(Plugin.KeyAnimDrink)}");
#endif
            sb.AppendLine($"装弹: {HintKey(Plugin.KeyAnimReload)}  摇骰: {HintKey(Plugin.KeyAnimShake)}");
            sb.AppendLine($"举枪: {HintKey(Plugin.KeyAnimTakeAim)}  开枪: {HintKey(Plugin.KeyAnimFire)}  空枪: {HintKey(Plugin.KeyAnimEmpty)}");
            sb.AppendLine();
        }
        if ((Plugin.HintTypeSelect.Value & HintType.DebugInfo) != HintType.None)
        {
            sb.AppendLine($"Position:  X: {charController.transform.localPosition.x:0.00}  Y: {charController.transform.localPosition.y:0.00}  Z: {charController.transform.localPosition.z:0.00}");
            sb.AppendLine($"Rotation:  X: {charController.transform.localEulerAngles.x:0.00}  Y: {charController.transform.localEulerAngles.y:0.00}  Z: {charController.transform.localEulerAngles.z:0.00}");
            sb.AppendLine($"Pitch: {charController.GetPitch():0.00}  Yaw: {charController.GetYaw():0.00}  Roll: {RemoveHeadRotationlimitPatch.CinemachineTargetRoll:0.00}");
        }
#if CHEATRELEASE
        CheatText(sb);
#endif
        return sb.ToString();
    }

    private static string buildHintEnglish()
    {
        var sb = new StringBuilder();
        if ((Plugin.HintTypeSelect.Value & HintType.TitleName) != HintType.None)
        {
            sb.AppendLine(hintTitle);
            sb.AppendLine($"{charController.playerStats().PlayerName}{HintHealth()}");
            sb.AppendLine();
        }
        if ((Plugin.HintTypeSelect.Value & HintType.HintKey) != HintType.None)
        {
            sb.AppendLine($"Show Hint: {HintKey(Plugin.KeyGameShowHint)}");
            sb.AppendLine($"Crazy Shake Head: {HintKey(Plugin.KeyViewCrazyShakeHead)}");
            sb.AppendLine($"Big Mouth: {HintKey(Plugin.KeyCustomBigMouth)}");
            sb.AppendLine($"Reset Position: {HintKey(Plugin.KeyMoveResetPosition)}");
            sb.AppendLine($"Reset View: {HintKey(Plugin.KeyViewReset)}");
            sb.AppendLine($"Body Move: {HintKey(Plugin.KeyMoveForward, Plugin.KeyMoveBack, Plugin.KeyMoveLeft, Plugin.KeyMoveRight)}");
            sb.AppendLine($"Body Rotate: {HintKey(Plugin.KeyRotateYaw, Plugin.KeyRotateRoll, Plugin.KeyRotatePitch)}");
            sb.AppendLine($"Head Move: {HintKey(Plugin.KeyViewForward, Plugin.KeyViewBack, Plugin.KeyViewLeft, Plugin.KeyViewRight, Plugin.KeyViewUp, Plugin.KeyViewDown)}");
            sb.AppendLine($"Head Rotate: {HintKey(Plugin.KeyViewAnticlockwise, Plugin.KeyViewClockwise)}");
            sb.AppendLine($"Jump: {HintKey(Plugin.KeyMoveJump)}");
            sb.AppendLine($"Squat: {HintKey(Plugin.KeyMoveSquat)}");
            sb.AppendLine($"Teleport: {HintKey(Plugin.KeyPosition)}");
            sb.AppendLine();
        }
        if ((Plugin.HintTypeSelect.Value & HintType.PluginInfo) != HintType.None)
        {
            sb.AppendLine($"Auto Rotate({HintKey(Plugin.KeyRotateAuto)}): {(AutoRotatePatch.Rotating ? on : off)}");
            sb.AppendLine($"Remove View Limit({HintKey(Plugin.KeyViewRemoveRotationLimit)}): {(Plugin.BooleanViewRemoveRotationLimit.Value ? on : off)}");
            sb.AppendLine($"Move Direction Follow Head({HintKey(Plugin.KeyMoveFollowHeadShortcut)}): {(Plugin.BooleanMoveFollowHead.Value ? on : off)}");
            sb.AppendLine($"FOV({HintKey(Plugin.KeyViewField)}, {FOVPatch.Fov:0.00}): {(Plugin.BooleanViewField.Value ? on : off)}");
            sb.AppendLine($"Show Self Top Info: {(Plugin.BooleanCustomShowSelfInfo.Value ? on : off)}");
            sb.AppendLine();
        }
        if ((Plugin.HintTypeSelect.Value & HintType.AnimKey) != HintType.None)
        {
            sb.AppendLine($"Liar: {HintKey(Plugin.KeyAnimCallLiar)}  SpotOn: {HintKey(Plugin.KeyAnimSpotOn)}");
            sb.AppendLine($"Throw: {HintKey(Plugin.KeyAnimThrow)}  Show: {HintKey(Plugin.KeyAnimShow)}");
#if CHEATRELEASE
            sb.Append($"");
            sb.AppendLine($"Roulet: {HintKey(Plugin.KeyAnimRoulet)}  Drink: {HintKey(Plugin.KeyAnimDrink)}  " +
                $"Type({HintKey(Plugin.KeyAnimRouletType)}): {Plugin.RouletAnimType.Value}");
#else
            sb.AppendLine($"Roulet: {HintKey(Plugin.KeyAnimRoulet)}  Drink: {HintKey(Plugin.KeyAnimDrink)}");
#endif
            sb.AppendLine($"Reload: {HintKey(Plugin.KeyAnimReload)}  Shake: {HintKey(Plugin.KeyAnimShake)}");
            sb.AppendLine($"TakeAim: {HintKey(Plugin.KeyAnimTakeAim)}  Fire: {HintKey(Plugin.KeyAnimFire)}  Empty: {HintKey(Plugin.KeyAnimEmpty)}");
            sb.AppendLine();
        }
        if ((Plugin.HintTypeSelect.Value & HintType.DebugInfo) != HintType.None)
        {
            sb.AppendLine($"Position:  X: {charController.transform.localPosition.x:0.00}  Y: {charController.transform.localPosition.y:0.00}  Z: {charController.transform.localPosition.z:0.00}");
            sb.AppendLine($"Rotation:  X: {charController.transform.localEulerAngles.x:0.00}  Y: {charController.transform.localEulerAngles.y:0.00}  Z: {charController.transform.localEulerAngles.z:0.00}");
            sb.AppendLine($"Pitch: {charController.GetPitch():0.00}  Yaw: {charController.GetYaw():0.00}  Roll: {RemoveHeadRotationlimitPatch.CinemachineTargetRoll:0.00}");
        }
#if CHEATRELEASE
        CheatText(sb);
#endif
        return sb.ToString();
    }

    private static string HintHealth()
    {
        if (charController.manager().mode == CustomNetworkManager.GameMode.LiarsDeck)
        {
            var blorfGame = charController.playerStats().GetComponent<BlorfGamePlay>();
            if (blorfGame)
            {
#if CHEATRELEASE
                return $"({blorfGame.Networkcurrentrevoler}|{(Plugin.BooleanCheatDeck.Value && Plugin.BooleanCheatDeckHealth.Value ? blorfGame.Networkrevolverbulllet + 1 : 6)})";
#else
                return $"({blorfGame.Networkcurrentrevoler}|6)";
#endif
            }
            else
            {
                var blorfGameMatchMaking = charController.playerStats().GetComponent<BlorfGamePlayMatchMaking>();
#if CHEATRELEASE
                return $"({blorfGameMatchMaking.Networkcurrentrevoler}|{(Plugin.BooleanCheatDeck.Value && Plugin.BooleanCheatDeckHealth.Value ? blorfGameMatchMaking.Networkrevolverbulllet + 1 : 6)})";
#else
                return $"({blorfGameMatchMaking.Networkcurrentrevoler}|6)";
#endif
            }
        }
        else if (charController.manager().mode == CustomNetworkManager.GameMode.LiarsDice)
        {
            return $"({charController.playerStats().NetworkHealth}|2)";
        }
        else if (charController.manager().mode == CustomNetworkManager.GameMode.LiarsChaos)
        {
            var chaosGame = charController.playerStats().GetComponent<ChaosGamePlay>();
#if CHEATRELEASE
            return $"({chaosGame.Networkcurrentrevoler}|{(Plugin.BooleanCheatDeck.Value && Plugin.BooleanCheatDeckHealth.Value ? chaosGame.Networkrevolverbulllet + 1 : 6)})";
#else
            return $"({chaosGame.Networkcurrentrevoler}|6)";
#endif
        }
        else
        {
            return "";
        }
    }

    private static string HintKey(params ConfigEntry<KeyboardShortcut>[] entries)
    {
        return entries.Select(e => e.Value).Join(KeyboardShortcutString, "  ");
    }

    private static string KeyboardShortcutString(KeyboardShortcut shortcut)
    {
        if (shortcut.MainKey == KeyCode.None) return "<color=yellow>Not set</color>";
        if (!shortcut.Modifiers.Any()) return $"<color=yellow>{KeycodeString(shortcut.MainKey)}</color>";
        return $"<color=yellow>{shortcut.Modifiers.Join(KeycodeString, "+")}+{KeycodeString(shortcut.MainKey)}</color>";
    }

    private static string KeycodeString(KeyCode key)
    {
        if (key >= KeyCode.Alpha0 && key <= KeyCode.Alpha9)
        {
            return ((int)key - (int)KeyCode.Alpha0).ToString();
        }
        if (key >= KeyCode.Keypad0 && key <= KeyCode.Keypad9)
        {
            return "Num" + ((int)key - (int)KeyCode.Keypad0);
        }
        return key switch
        {
            KeyCode.Escape => "Esc",
            KeyCode.Quote => "'",
            KeyCode.Comma => ",",
            KeyCode.Minus => "-",
            KeyCode.Period => ".",
            KeyCode.Slash => "/",
            KeyCode.Semicolon => ";",
            KeyCode.Equals => "=",
            KeyCode.LeftBracket => "[",
            KeyCode.Backslash => "\\",
            KeyCode.RightBracket => "]",
            KeyCode.BackQuote => "`",
            KeyCode.CapsLock => "Caps",
            KeyCode.KeypadPeriod => "Num.",
            KeyCode.KeypadDivide => "Num/",
            KeyCode.KeypadMultiply => "Num*",
            KeyCode.KeypadMinus => "Num-",
            KeyCode.KeypadPlus => "Num+",
            KeyCode.UpArrow => "↑",
            KeyCode.DownArrow => "↓",
            KeyCode.RightArrow => "→",
            KeyCode.LeftArrow => "←",
            KeyCode.KeypadEnter => "NumEnter",
            KeyCode.RightShift => "RShift",
            KeyCode.LeftShift => "LShift",
            KeyCode.RightControl => "RCtrl",
            KeyCode.LeftControl => "LCtrl",
            KeyCode.RightAlt => "RAlt",
            KeyCode.LeftAlt => "LAlt",
            _ => key.ToString()
        };
    }

#if CHEATRELEASE
    private static void CheatText(StringBuilder sb)
    {
        sb.AppendLine();
        if (Plugin.BooleanCheatDeck.Value && Plugin.BooleanCheatBlorfLastRoundCard.Value && charController.manager.mode == CustomNetworkManager.GameMode.LiarsDeck)
        {
            var (LastRound, RoundCard) =
            charController.playerStats.GetComponent<BlorfGamePlay>() ?
            (charController.manager.BlorfGame.LastRound, charController.manager.BlorfGame.RoundCard) :
            (charController.manager.BlorfGameMatchMaking.LastRound, charController.manager.BlorfGameMatchMaking.RoundCard);
            foreach (var card in LastRound)
            {
                var type = card switch
                {
                    -1 => "Devil",
                    1 => "King",
                    2 => "Queen",
                    3 => "Ace",
                    4 => "Joker",
                    _ => card.ToString()
                };
                sb.AppendLine($"<color={(card == -1 || card == 4 || card == RoundCard ? "lime" : "red")}>{type}</color>");
            }
        }
        if (Plugin.BooleanCheatDice.Value && Plugin.BooleanCheatDiceTotalDice.Value && charController.manager.mode == CustomNetworkManager.GameMode.LiarsDice)
        {
            for (var i = 0; i < 6; i++)
            {
                sb.AppendLine($"{DiceCheatPatch.diceCounts[i],2}{(isChinese ? "个" : " of ")}{i + 1}");
            }
        }
        if (Plugin.BooleanCheatDeck.Value && Plugin.BooleanCheatBlorfLastRoundCard.Value && charController.manager.mode == CustomNetworkManager.GameMode.LiarsChaos)
        {
            foreach (var card in charController.manager.ChaosGame.LastRound)
            {
                var type = card switch
                {
                    1 => "King",
                    2 => "Queen",
                    3 => "Chaos",
                    4 => "Master",
                    _ => card.ToString()
                };
                sb.AppendLine($"<color={(card > 2 || card == charController.manager.ChaosGame.RoundCard ? "lime" : "red")}>{type}</color>");
            }
        }
    }
#endif
}

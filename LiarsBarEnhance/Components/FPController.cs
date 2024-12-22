using BepInEx.Configuration;

using HarmonyLib;

using LiarsBarEnhance.Features;
using LiarsBarEnhance.Utils;

using System;
using System.Linq;
using System.Text;

using UnityEngine;

namespace LiarsBarEnhance.Components
{
    public class FpController : MonoBehaviour
    {
        private CharController charController;
        private PlayerStats playerStats;
        private Manager manager;
        private float groundY;
        private float playerY;
        private bool inGround = true;
        private bool isFling = false;
        private float fullV = 0f;
        private Vector3 initHeadPosition;
        private Vector3 initBodyPosition;
        private Quaternion initBodyRotation;
        private float guiShow = 0f;

        private void Start()
        {
            charController = GetComponent<CharController>();
            playerStats = FastMemberAccessor<CharController, PlayerStats>.Get(charController, "playerStats");
            if (!charController.isOwned) return;
            manager = FastMemberAccessor<CharController, Manager>.Get(charController, "manager");

            initHeadPosition = charController.HeadPivot.transform.localPosition;
            initBodyPosition = charController.transform.localPosition;
            initBodyRotation = charController.transform.localRotation;
            groundY = charController.transform.position.y;
            playerY = groundY;

            hintTitle = HintTitle();
            void hintGuiEvent(object sender, EventArgs args) => guiShow = 1f;
            Plugin.IntHintPosX.SettingChanged += hintGuiEvent;
            Plugin.IntHintPosY.SettingChanged += hintGuiEvent;
            Plugin.HintTypeSelect.SettingChanged += hintGuiEvent;
        }

        private void Update()
        {
            if (!charController.isOwned) return;
            if (manager.PluginControl())
            {
                MoveHead();
                MouseRotate();
                Move();
            }
            if (Plugin.KeyMoveResetPosition.IsDown())
            {
                charController.transform.localPosition = initBodyPosition;
                charController.transform.localRotation = initBodyRotation;
                groundY = charController.transform.position.y;
                playerY = groundY;
                inGround = true;
                isFling = false;
                fullV = 0f;
                AnimationPatch.AnimFrame = -1;
            }
            if (Plugin.KeyViewReset.IsDown())
            {
                charController.HeadPivot.transform.localPosition = initHeadPosition;
                charController.SetYaw(0f);
                charController.SetPitch(0f);
                CrazyShakeHeadPatch.CinemachineTargetRoll = 0f;
                Plugin.FloatViewField.Value = 60f;
            }
            if (Plugin.KeyViewRemoveRotationLimit.IsDown())
            {
                Plugin.BooleanViewRemoveRotationLimit.Value = !Plugin.BooleanViewRemoveRotationLimit.Value;
            }
            if (Plugin.KeyMoveFollowHeadShortcut.IsDown())
            {
                Plugin.BooleanMoveFollowHead.Value = !Plugin.BooleanMoveFollowHead.Value;
            }
            if (Plugin.KeyViewField.IsDown())
            {
                Plugin.BooleanViewField.Value = !Plugin.BooleanViewField.Value;
            }
#if CHEATRELEASE
            if (Plugin.KeyAnimRouletType.IsDown())
            {
                Plugin.RouletAnimType.Value = Plugin.RouletAnimType.Value switch
                {
                    RouletType.AnimOnly => RouletType.Roulet,
                    RouletType.Roulet => RouletType.Suicide,
                    RouletType.Suicide => RouletType.AnimOnly,
                    _ => RouletType.AnimOnly
                };
            }
#endif
        }

        private void MoveHead()
        {
            if (!playerStats.Dead || !Plugin.BooleanViewRemoveRotationLimit.Value)
            {
                if (Plugin.KeyViewForward.IsPressed())
                    charController.HeadPivot.transform.Translate(Plugin.FloatViewSpeed.Value * Time.deltaTime * Vector3.up, Space.Self);//Y+
                if (Plugin.KeyViewBack.IsPressed())
                    charController.HeadPivot.transform.Translate(Plugin.FloatViewSpeed.Value * Time.deltaTime * Vector3.down, Space.Self);//Y-
                if (Plugin.KeyViewLeft.IsPressed())
                    charController.HeadPivot.transform.Translate(Plugin.FloatViewSpeed.Value * Time.deltaTime * Vector3.forward, Space.Self);//Z+
                if (Plugin.KeyViewRight.IsPressed())
                    charController.HeadPivot.transform.Translate(Plugin.FloatViewSpeed.Value * Time.deltaTime * Vector3.back, Space.Self);//Z-
                if (Plugin.KeyViewUp.IsPressed())
                    charController.HeadPivot.transform.Translate(Plugin.FloatViewSpeed.Value * Time.deltaTime * Vector3.left, Space.Self);//X-
                if (Plugin.KeyViewDown.IsPressed())
                    charController.HeadPivot.transform.Translate(Plugin.FloatViewSpeed.Value * Time.deltaTime * Vector3.right, Space.Self);//X+
            }
            else
            {
                var speccamindex = FastMemberAccessor<CharController, int>.Get(charController, "speccamindex");
                var trans = manager.SpectatorCameraParrent.transform.GetChild(speccamindex);
                if (Plugin.KeyViewForward.IsPressed())
                    trans.Translate(Plugin.FloatViewSpeed.Value * Time.deltaTime * Vector3.forward, Space.Self);
                if (Plugin.KeyViewBack.IsPressed())
                    trans.Translate(Plugin.FloatViewSpeed.Value * Time.deltaTime * Vector3.back, Space.Self);
                if (Plugin.KeyViewLeft.IsPressed())
                    trans.Translate(Plugin.FloatViewSpeed.Value * Time.deltaTime * Vector3.left, Space.Self);
                if (Plugin.KeyViewRight.IsPressed())
                    trans.Translate(Plugin.FloatViewSpeed.Value * Time.deltaTime * Vector3.right, Space.Self);
                if (Plugin.KeyViewUp.IsPressed())
                    trans.Translate(Plugin.FloatViewSpeed.Value * Time.deltaTime * Vector3.up, Space.Self);
                if (Plugin.KeyViewDown.IsPressed())
                    trans.Translate(Plugin.FloatViewSpeed.Value * Time.deltaTime * Vector3.down, Space.Self);
            }
        }

        private void MouseRotate()
        {
            var sensivty = PlayerPrefs.GetFloat("MouseSensivity", 50f);
            var x = Input.GetAxis("Mouse X") * Time.deltaTime * sensivty;
            var y = Input.GetAxis("Mouse Y") * Time.deltaTime * sensivty;
            var RotateYaw = Plugin.KeyRotateYaw.IsPressed();
            var RotatePitch = Plugin.KeyRotatePitch.IsPressed();
            var RotateRoll = Plugin.KeyRotateRoll.IsPressed();
            if (RotateYaw || RotateRoll)
            {
                if (RotateYaw)
                {
                    charController.transform.Rotate(x * Vector3.up, Space.Self);
                }
                if (RotateRoll)
                {
                    charController.transform.Rotate(-x * Vector3.forward, Space.Self);
                }
                if (!playerStats.Dead) charController.AddYaw(x);
            }
            else if (playerStats.Dead && !Plugin.BooleanViewRemoveRotationLimit.Value)
            {
                charController.AddYaw(-x);
            }
            if (RotatePitch)
            {
                charController.transform.Rotate(y * Vector3.right, Space.Self);
                if (!playerStats.Dead) charController.AddPitch(-y);
            }
            else if (playerStats.Dead && !Plugin.BooleanViewRemoveRotationLimit.Value)
            {
                charController.AddPitch(y);
            }
            var speccamindex = FastMemberAccessor<CharController, int>.Get(charController, "speccamindex");
            var trans = manager.SpectatorCameraParrent.transform.GetChild(speccamindex);
            if (playerStats.Dead && Plugin.BooleanViewRemoveRotationLimit.Value)
            {
                var rz = (Plugin.KeyViewAnticlockwise.IsPressed() ? 2f : 0f) + (Plugin.KeyViewClockwise.IsPressed() ? -2f : 0f);
                trans.Rotate(new Vector3(-y, x, rz), Space.Self);
            }
        }

        private void Move()
        {
            var squat = Plugin.KeyMoveSquat.IsPressed();
            var jump = Plugin.KeyMoveJump.IsPressed();
            var moveSpeed = Plugin.FloatMoveSpeed.Value * (squat ? 0.5f : 1f);

            if (isFling)
            {
                if (jump)
                {
                    charController.transform.Translate(moveSpeed * Time.deltaTime * Vector3.up, Space.Self);
                    playerY += moveSpeed * Time.deltaTime;
                }
                if (squat)
                {
                    charController.transform.Translate(moveSpeed * Time.deltaTime * Vector3.down, Space.Self);
                    playerY -= moveSpeed * Time.deltaTime;
                }
                if (jump && squat)
                {
                    isFling = false;
                    fullV = 0f;
                    if (playerY < groundY)
                    {
                        charController.transform.Translate((groundY - playerY) * Vector3.up, Space.Self);
                        playerY = groundY;
                        fullV = 0f;
                        inGround = true;
                    }
                    else
                    {
                        inGround = false;
                    }
                }
            }
            else
            {
                if (Plugin.KeyMoveSquat.IsDown())
                {
                    groundY -= 0.3f;
                    if (inGround && groundY < playerY)
                    {
                        inGround = false;
                    }
                }
                if (Plugin.KeyMoveSquat.IsUp())
                {
                    groundY += 0.3f;
                    if (groundY > playerY)
                    {
                        charController.transform.Translate((groundY - playerY) * Vector3.up, Space.Self);
                        playerY = groundY;
                        fullV = 0f;
                        inGround = true;
                    }
                }
                if (inGround)
                {
                    if (jump)
                    {
                        fullV = Mathf.Sqrt(2 * Plugin.FloatGravity.Value * Plugin.FloatJumpHeight.Value);
                        inGround = false;
                    }
                }
                else
                {
                    if (fullV > 1f && Plugin.KeyMoveJump.IsDown())
                    {
                        isFling = true;
                    }
                    else
                    {
                        var moveY = fullV * Time.deltaTime;
                        playerY += moveY;
                        fullV -= Plugin.FloatGravity.Value * Time.deltaTime;
                        charController.transform.Translate(moveY * Vector3.up, Space.Self);
                        if (playerY < groundY)
                        {
                            charController.transform.Translate((groundY - playerY) * Vector3.up, Space.Self);
                            playerY = groundY;
                            fullV = 0f;
                            inGround = true;
                        }
                    }
                }
            }
            if (Plugin.KeyMoveForward.IsPressed())
            {
                charController.transform.Translate(moveSpeed * Time.deltaTime * WalkBodyRotate(Vector3.forward, 0f), Space.Self);
            }
            if (Plugin.KeyMoveBack.IsPressed())
            {
                charController.transform.Translate(moveSpeed * Time.deltaTime * WalkBodyRotate(Vector3.back, 0f), Space.Self);
            }
            if (Plugin.KeyMoveLeft.IsPressed())
            {
                charController.transform.Translate(moveSpeed * Time.deltaTime * WalkBodyRotate(Vector3.left, -Plugin.FloatMoveHorizontalBodyRotate.Value), Space.Self);
            }
            if (Plugin.KeyMoveRight.IsPressed())
            {
                charController.transform.Translate(moveSpeed * Time.deltaTime * WalkBodyRotate(Vector3.right, Plugin.FloatMoveHorizontalBodyRotate.Value), Space.Self);
            }
        }

        private Vector3 WalkBodyRotate(Vector3 d, float toYaw)
        {
            if (!Plugin.BooleanMoveFollowHead.Value) return d;
            var yaw = charController.GetYaw();
            var rotateAngle = (toYaw - yaw) / 5f;
            var newYaw = yaw + rotateAngle;
            if (Mathf.Abs(rotateAngle) > 1f)
            {
                charController.transform.Rotate(rotateAngle * Vector3.up, Space.Self);
                charController.SetYaw(newYaw);
                return (Quaternion.AngleAxis(-newYaw, Vector3.up) * d);
            }
            else if (rotateAngle != 0f)
            {
                rotateAngle = toYaw - yaw;
                newYaw = yaw + rotateAngle;
                charController.transform.Rotate(rotateAngle * Vector3.up, Space.Self);
                charController.SetYaw(newYaw);
                return (Quaternion.AngleAxis(-newYaw, Vector3.up) * d);
            }
            else
            {
                return (Quaternion.AngleAxis(-toYaw, Vector3.up) * d);
            }
        }

        private string hintTitle;
        private readonly string on = "<color=lime>开</color>";
        private readonly string off = "<color=red>关</color>";
        private void OnGUI()
        {
            if (charController.isOwned && (Plugin.KeyGameShowHint.IsPressed() || guiShow > 0f))
            {
                if (guiShow > 0) guiShow -= Time.deltaTime;
                var x = Plugin.IntHintPosX.Value;
                if (x < 0) x += Screen.width;
                var y = Plugin.IntHintPosY.Value;
                if (y < 0) y += Screen.height;
                GUI.Label(new Rect(x, y, 240, 480), buildHint(),
                    new GUIStyle
                    {
                        fontSize = 15,
                        normal = { textColor = Color.white }
                    }
                );
            }
        }

        private string buildHint()
        {
            var sb = new StringBuilder();
            if ((Plugin.HintTypeSelect.Value & HintType.TitleName) != HintType.None)
            {
                sb.AppendLine(hintTitle);
                sb.AppendLine($"{playerStats.PlayerName}{HintHealth()}");
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
                    $"类型({HintKey(Plugin.KeyAnimRouletType)}): {Plugin.RouletAnimType.Value.GetEnumDescription()}");
#else
                sb.AppendLine($"自枪: {HintKey(Plugin.KeyAnimRoulet)}  喝酒: {HintKey(Plugin.KeyAnimDrink)}");
#endif
                sb.AppendLine($"装弹: {HintKey(Plugin.KeyAnimReload)}  摇骰: {HintKey(Plugin.KeyAnimShake)}");
                sb.AppendLine($"举枪: {HintKey(Plugin.KeyAnimTakeAim)}  开枪: {HintKey(Plugin.KeyAnimFire)}  空枪: {HintKey(Plugin.KeyAnimEmpty)}");
                sb.AppendLine();
            }
            if ((Plugin.HintTypeSelect.Value & HintType.DebugInfo) != HintType.None)
            {
                sb.AppendLine($"Position:  X: {transform.localPosition.x:0.00}  Y: {transform.localPosition.y:0.00}  Z: {transform.localPosition.z:0.00}");
                sb.AppendLine($"Rotation:  X: {transform.localEulerAngles.x:0.00}  Y: {transform.localEulerAngles.y:0.00}  Z: {transform.localEulerAngles.z:0.00}");
                sb.AppendLine($"Pitch: {charController.GetPitch():0.00}  Yaw: {charController.GetYaw():0.00}  Roll: {CrazyShakeHeadPatch.CinemachineTargetRoll:0.00}");
            }
#if CHEATRELEASE
            CheatText(sb);
#endif
            return sb.ToString();
        }

        private string HintTitle()
        {
            var mode = $"<color=#F5E37B>{manager.mode}</color>";
            string ruleSet;
            if (manager.mode == CustomNetworkManager.GameMode.LiarsDeck)
            {
                if (manager.BlorfGame.DeckMode == BlorfGamePlayManager.deckmode.Basic)
                    ruleSet = $"<color=#3487AB>{BlorfGamePlayManager.deckmode.Basic}</color>";
                else
                    ruleSet = $"<color=#CD2C48>{BlorfGamePlayManager.deckmode.Devil}</color>";
            }
            else if (manager.mode == CustomNetworkManager.GameMode.LiarsDice)
            {
                if (manager.DiceGame.DiceMode == DiceGamePlayManager.dicemode.Basic)
                    ruleSet = $"<color=#3487AB>{DiceGamePlayManager.dicemode.Basic}</color>";
                else
                    ruleSet = $"<color=#DFAF4A>{DiceGamePlayManager.dicemode.Traditional}</color>";
            }
            else if (manager.mode == CustomNetworkManager.GameMode.LiarsChaos)
            {
                return $"<color=#F5E37B>{CustomNetworkManager.GameMode.LiarsDeck}</color> - <color=#FF82FF>Chaos</color>";
            }
            else
            {
                ruleSet = "";
            }
            return $"{mode} - {ruleSet}";
        }

        private string HintHealth()
        {
            if (charController is BlorfGamePlay blorfGame)
            {
#if CHEATRELEASE
                return $"({blorfGame.Networkcurrentrevoler}|{(Plugin.BooleanCheatDeck.Value && Plugin.BooleanCheatDeckHealth.Value ? blorfGame.Networkrevolverbulllet + 1 : 6)})";
#else
                return $"({blorfGame.Networkcurrentrevoler}|6)";
#endif
            }
            else if (charController is DiceGamePlay diceGame)
            {
                return $"({playerStats.NetworkHealth}|2)";
            }
            else if (charController is ChaosGamePlay chaosGame)
            {
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

        private string HintKey(params ConfigEntry<KeyboardShortcut>[] entries)
        {
            return entries.Select(e => e.Value).Join(KeyboardShortcutString, "  ");
        }

        private string KeyboardShortcutString(KeyboardShortcut shortcut)
        {
            if (shortcut.MainKey == KeyCode.None) return "<color=yellow>未设置</color>";
            if (shortcut.Modifiers.Count() == 0) return $"<color=yellow>{KeycodeString(shortcut.MainKey)}</color>";
            return $"<color=yellow>{shortcut.Modifiers.Join(KeycodeString, "+")}+{KeycodeString(shortcut.MainKey)}</color>";
        }
        private string KeycodeString(KeyCode key)
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
                KeyCode.Mouse0 => "左键",
                KeyCode.Mouse1 => "右键",
                KeyCode.Mouse2 => "中键",
                _ => key.ToString()
            };
        }

#if CHEATRELEASE
        private void CheatText(StringBuilder sb)
        {
            sb.AppendLine();
            if (Plugin.BooleanCheatDeck.Value && Plugin.BooleanCheatBlorfLastRoundCard.Value && charController is BlorfGamePlay)
            {
                foreach (var card in manager.BlorfGame.LastRound)
                {
                    var type = card switch
                    {
                        -1 => "Devil",
                        1 => "King",
                        2 => "Queen",
                        3 => "Ace",
                        4 => "Joker",
                        _ => ""
                    };
                    sb.AppendLine($"<color={(card == -1 || card == 4 || card == manager.BlorfGame.RoundCard ? "lime" : "red")}>{type}</color>");
                }
            }
            if (Plugin.BooleanCheatDice.Value && Plugin.BooleanCheatDiceTotalDice.Value && charController is DiceGamePlay)
            {
                for (var i = 0; i < 6; i++)
                {
                    sb.AppendLine($"{DiceCheatPatch.diceCounts[i],2}个{i + 1}");
                }
            }
            if (Plugin.BooleanCheatDeck.Value && Plugin.BooleanCheatBlorfLastRoundCard.Value && charController is ChaosGamePlay)
            {
                foreach (var card in manager.ChaosGame.LastRound)
                {
                    var type = card switch
                    {
                        1 => "King",
                        2 => "Queen",
                        3 => "Chaos",
                        4 => "Master",
                        _ => ""
                    };
                    sb.AppendLine($"<color={(card > 2 || card == manager.ChaosGame.RoundCard ? "lime" : "red")}>{type}</color>");
                }
            }
        }
#endif
    }
}

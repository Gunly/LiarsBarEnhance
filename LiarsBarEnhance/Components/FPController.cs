using BepInEx.Configuration;

using HarmonyLib;

using LiarsBarEnhance.Features;
using LiarsBarEnhance.Utils;

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

            Plugin.IntHintPosX.SettingChanged += (s, a) => guiShow = 1f;
            Plugin.IntHintPosY.SettingChanged += (s, a) => guiShow = 1f;
            hintTitle = HintTitle();
        }

        private void Update()
        {
            if (!charController.isOwned) return;
            if (!manager.GamePaused && !manager.Chatting)
            {
                MoveHead();
                MouseRotate();
                Move();
            }
            if (Plugin.KeyMoveResetPosition.IsDown())
            {
                charController.HeadPivot.transform.localPosition = initHeadPosition;
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
                charController.SetYaw(0f);
                charController.SetPitch(0f);
                CharMoveablePatch.CinemachineTargetRoll = 0f;
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
        private readonly string on = "<color=#00FF00>开</color>";
        private readonly string off = "<color=#FF0000>关</color>";
        private void OnGUI()
        {
            if (charController.isOwned && (Plugin.KeyCustomShowHint.IsPressed() || guiShow > 0f))
            {
                if (guiShow > 0) guiShow -= Time.deltaTime;
                var x = Plugin.IntHintPosX.Value;
                if (x < 0) x += Screen.width;
                var y = Plugin.IntHintPosY.Value;
                if (y < 0) y += Screen.height;
                GUI.Label(new Rect(x, y, 240, 480),
                    hintTitle +
                    $"{playerStats.PlayerName}{HintHealth()}\n" +
                    "\n" +
                    $"按住 {HintKey(Plugin.KeyCustomShowHint)} 显示提示\n" +
                    $"按住 {HintKey(Plugin.KeyViewCrazyShakeHead)} 疯狂转头\n" +
                    $"按住 {HintKey(Plugin.KeyCustomBigMouth)} 张嘴\n" +
                    $"按 {HintKey(Plugin.KeyMoveResetPosition)} 重置位置\n" +
                    $"按 {HintKey(Plugin.KeyViewReset)} 重置视角\n" +
                    $"身体移动: {HintKey(Plugin.KeyMoveForward, Plugin.KeyMoveBack, Plugin.KeyMoveLeft, Plugin.KeyMoveRight)}\n" +
                    $"头部移动: {HintKey(Plugin.KeyViewForward, Plugin.KeyViewBack, Plugin.KeyViewLeft, Plugin.KeyViewRight, Plugin.KeyViewUp, Plugin.KeyViewDown)}\n" +
                    $"头部偏转: {HintKey(Plugin.KeyViewAnticlockwise, Plugin.KeyViewClockwise)}\n" +
                    $"按住 {HintKey(Plugin.KeyRotateYaw)} 水平转动身体\n" +
                    $"按住 {HintKey(Plugin.KeyRotatePitch)} 垂直(前后)转动身体\n" +
                    $"按住 {HintKey(Plugin.KeyRotateRoll)} 垂直(左右)转动身体\n" +
                    $"按 {HintKey(Plugin.KeyMoveJump)} 跳跃\n" +
                    $"按 {HintKey(Plugin.KeyMoveSquat)} 蹲下\n" +
                    $"按 {HintKey(Plugin.KeyRotateAuto)} 自动旋转\n" +
                    $"传送: {HintKey(Plugin.KeyPosition)}\n" +
                    "\n" +
                    $"解除视角限制({HintKey(Plugin.KeyViewRemoveRotationLimit)}): {(Plugin.BooleanViewRemoveRotationLimit.Value ? on : off)}\n" +
                    $"移动方向跟随头部视角({HintKey(Plugin.KeyMoveFollowHeadShortcut)}): {(Plugin.BooleanMoveFollowHead.Value ? on : off)}\n" +
                    $"调整视场({HintKey(Plugin.KeyViewField)}, {FOVPatch.Fov:0.00}): {(Plugin.BooleanViewField.Value ? on : off)}\n" +
                    $"显示自身头顶信息: {(Plugin.BooleanCustomShowSelfInfo.Value ? on : off)}\n" +
                    "\n" +
                    $"Liar: {HintKey(Plugin.KeyAnimCallLiar)}  SpotOn: {HintKey(Plugin.KeyAnimSpotOn)}\n" +
                    $"扔牌: {HintKey(Plugin.KeyAnimThrow)}  展示: {HintKey(Plugin.KeyAnimShow)}\n" +
                    $"开枪: {HintKey(Plugin.KeyAnimRoulet)}  喝酒: {HintKey(Plugin.KeyAnimDrink)}\n" +
                    $"装弹: {HintKey(Plugin.KeyAnimReload)}  摇骰: {HintKey(Plugin.KeyAnimShake)}\n" +
                    "\n" +
                    $"Position:  X: {transform.localPosition.x:0.00}  Y: {transform.localPosition.y:0.00}  Z: {transform.localPosition.z:0.00}\n" +
                    $"Rotation:  X: {transform.localEulerAngles.x:0.00}  Y: {transform.localEulerAngles.y:0.00}  Z: {transform.localEulerAngles.z:0.00}\n" +
                    $"Pitch: {charController.GetPitch():0.00}  Yaw: {charController.GetYaw():0.00}  Roll: {CharMoveablePatch.CinemachineTargetRoll:0.00}\n" +
#if CHEATRELEASE
                    CheatText() +
#endif
                    "",
                    new GUIStyle
                    {
                        fontSize = 15,
                        normal = { textColor = Color.white }
                    }
                );
            }
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
            else
            {
                if (manager.DiceGame.DiceMode == DiceGamePlayManager.dicemode.Basic)
                    ruleSet = $"<color=#3487AB>{DiceGamePlayManager.dicemode.Basic}</color>";
                else
                    ruleSet = $"<color=#DFAF4A>{DiceGamePlayManager.dicemode.Traditional}</color>";
            }
            return $"{mode} - {ruleSet}\n";
        }

        private string HintHealth()
        {
            if (charController is BlorfGamePlay blorfGame)
            {
#if CHEATRELEASE
                return $"({blorfGame.Networkcurrentrevoler}|{(Plugin.BooleanCheatBlorf.Value && Plugin.BooleanCheatBlorfHealth.Value ? blorfGame.Networkrevolverbulllet + 1 : 6)})";
#else
                return $"({blorfGame.Networkcurrentrevoler}|6)";
#endif
            }
            else if (charController is DiceGamePlay diceGame)
            {
                return $"({playerStats.NetworkHealth}|2)";
            }
            else
            {
                return "";
            }
        }

        private string HintKey(params ConfigEntry<KeyboardShortcut>[] entries)
        {
            return entries.Select(e => e.Value).Join(KeyboardShortcutString);
        }

        private string KeyboardShortcutString(KeyboardShortcut shortcut)
        {
            if (shortcut.MainKey == KeyCode.None) return "未设置";
            if (shortcut.Modifiers.Count() == 0) return $"<color=#FFFF00>{KeycodeString(shortcut.MainKey)}</color>";
            return $"<color=#FFFF00>{shortcut.Modifiers.Join(KeycodeString, "+")}+{KeycodeString(shortcut.MainKey)}</color>";
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
        private string CheatText()
        {
            var sb = new StringBuilder();
            if (Plugin.BooleanCheatBlorf.Value && Plugin.BooleanCheatBlorfLastRoundCard.Value && charController is BlorfGamePlay)
            {
                foreach (var card in manager.BlorfGame.LastRound)
                {
                    var type = card switch
                    {
                        -1 => "Devil",
                        1 => "King",
                        2 => "Queen",
                        3 => "Ace",
                        _ => "Joker"
                    };
                    sb.AppendLine($"<color=#{(card == -1 || card == 4 || card == manager.BlorfGame.RoundCard ? "00FF" : "FF00")}00>{type}</color>");
                }
            }
            if (Plugin.BooleanCheatDice.Value && Plugin.BooleanCheatDiceTotalDice.Value && charController is DiceGamePlay)
            {
                for (var i = 0; i < 6; i++)
                {
                    sb.AppendLine($"{DiceCheatPatch.diceCounts[i],2}个{i + 1}");
                }
            }
            return sb.ToString();
        }
#endif
    }
}

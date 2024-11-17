using BepInEx.Configuration;

using LiarsBarEnhance.Features;
using LiarsBarEnhance.Utils;

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

        private void Start()
        {
            charController = GetComponent<CharController>();
            if (!charController.isOwned) return;
            playerStats = CharMoveablePatch.PlayerStats;
            manager = CharMoveablePatch.Manager;

            initHeadPosition = charController.HeadPivot.transform.localPosition;
            initBodyPosition = charController.transform.localPosition;
            initBodyRotation = charController.transform.localRotation;
            groundY = charController.transform.position.y;
            playerY = groundY;
        }

        private void Update()
        {
            if (!charController.isOwned) return;
            if (!manager.GamePaused)
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
                CharMoveablePatch.CinemachineTargetRoll = 0f;
                AnimationPatch.AnimFrame = -1;
                if (Plugin.BooleanResetView.Value)
                {
                    charController.SetYaw(0f);
                    charController.SetPitch(0f);
                    Plugin.FloatViewField.Value = 60f;
                }
            }
        }

        private void MoveHead()
        {
            if (!playerStats.Dead)
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
            var RotateYaw = Plugin.KeyRotateYaw.IsPressed();
            var RotatePitch = Plugin.KeyRotatePitch.IsPressed();
            var RotateRoll = Plugin.KeyRotateRoll.IsPressed();
            var sensivty = PlayerPrefs.GetFloat("MouseSensivity", 50f);
            var x = Input.GetAxis("Mouse X") * Time.deltaTime * sensivty;
            var y = Input.GetAxis("Mouse Y") * Time.deltaTime * sensivty;
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
            if (RotatePitch)
            {
                charController.transform.Rotate(y * Vector3.right, Space.Self);
                if (!playerStats.Dead) charController.AddPitch(-y);
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
            var run = Plugin.KeyMoveRun.IsPressed();
            var squat = Plugin.KeyMoveSquat.IsPressed();
            var jump = Plugin.KeyMoveJump.IsPressed();
            var moveSpeed = Plugin.FloatMoveSpeed.Value * (run ? 1.5f : 1f) * (squat ? 0.5f : 1f);

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
                charController.transform.Translate(moveSpeed * Time.deltaTime * WalkBodyRotate(Vector3.left, 0f), Space.Self);
            }
            if (Plugin.KeyMoveRight.IsPressed())
            {
                charController.transform.Translate(moveSpeed * Time.deltaTime * WalkBodyRotate(Vector3.right, 0f), Space.Self);
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

        private void OnGUI()
        {
            if (Plugin.KeyCustomShowHint.IsPressed())
            {
                var yes = "<color=#00FF00>是</color>";
                var no = "<color=#FF0000>否</color>";
                GUI.Label(new Rect(Screen.width - 240, 60, 240, 300),
                    $"按住 {HintKey(Plugin.KeyCustomShowHint)} 显示提示\n" +
                    $"按住 {HintKey(Plugin.KeyViewCrazyShakeHead)} 疯狂转头\n" +
                    $"按住 {HintKey(Plugin.KeyCustomBigMouth)} 张嘴\n" +
                    $"按 {HintKey(Plugin.KeyMoveResetPosition)} 重置位置\n" +
                    $"    重置位置时重置视角: {(Plugin.BooleanResetView.Value ? yes : no)}\n" +
                    $"按 {HintKey(Plugin.KeyViewRemoveRotationLimit)} 切换解除视角限制\n" +
                    $"    解除视角限制: {(Plugin.BooleanViewRemoveRotationLimit.Value ? yes : no)}\n" +
                    "默认↑↓←→身体移动\n" +
                    $"    移动方向跟随视角: {(Plugin.BooleanMoveFollowHead.Value ? yes : no)}\n" +
                    "默认小键盘数字头部移动\n" +
                    $"按 {HintKey(Plugin.KeyRotateAuto)} 自动旋转\n" +
                    $"按住 {HintKey(Plugin.KeyRotateYaw)} 水平转动身体\n" +
                    $"按住 {HintKey(Plugin.KeyRotatePitch)} 垂直(前后)转动身体\n" +
                    $"按住 {HintKey(Plugin.KeyRotateRoll)} 垂直(左右)转动身体\n" +
                    $"按 {HintKey(Plugin.KeyMoveJump)} 跳跃\n" +
                    $"按 {HintKey(Plugin.KeyMoveSquat)} 蹲下\n" +
                    $"按 {HintKey(Plugin.KeyMoveRun)} 奔跑\n" +
                    "默认 <color=#FFFF00>1-4</color> 传送到不同座位\n" +
                    "使用 <color=#FFFF00>鼠标滚轮</color> 调整视野",
                    new GUIStyle
                    {
                        fontSize = 15,
                        normal = { textColor = Color.white }
                    }
                );
            }
        }

        private string HintKey(ConfigEntry<KeyboardShortcut> entry)
        {
            return $"<color=#FFFF00>{entry.Value.MainKey}</color>";
        }
    }
}

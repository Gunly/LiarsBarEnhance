using BepInEx.Configuration;

using Cinemachine;

using HarmonyLib;

using LiarsBarEnhance.Features;
using LiarsBarEnhance.Utils;

using Mirror;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

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
        private Vector3 initScale;
        private bool rotating = false;
        private readonly List<KeyFrame> usingAnim = [];
        private int animFrame = -1;
        private Vector3 animStartPosition;
        private Quaternion animStartRotation;
        private CinemachineVirtualCamera cam;
        private float defaultFOV = 0f, fov = 0f;

        private void Start()
        {
            charController = GetComponent<CharController>();
            playerStats = FastMemberAccessor<CharController, PlayerStats>.Get(charController, "playerStats");
            initScale = charController.transform.localScale;
            if (Plugin.BooleanTestGiraffe.Value)
            {
                var ntrs = charController.gameObject.GetComponents<NetworkTransformReliable>();
                foreach (var ntr in ntrs)
                {
                    ntr.syncPosition = true;
                    ntr.syncScale = true;
                }
            }
            if (!charController.isOwned) return;

            initHeadPosition = charController.HeadPivot.transform.localPosition;
            initBodyPosition = charController.transform.localPosition;
            initBodyRotation = charController.transform.localRotation;
            groundY = charController.transform.position.y;
            playerY = groundY;

            manager = FastMemberAccessor<CharController, Manager>.Get(charController, "manager");
            try
            {
                cam = charController.HeadPivot.Find("Base HumanHead/Virtual Camera").GetComponent<CinemachineVirtualCamera>();
            }
            catch (Exception)
            {
                cam = charController.HeadPivot.Find("RHINO_Head/Virtual Camera").GetComponent<CinemachineVirtualCamera>();
            }
            fov = defaultFOV = cam.m_Lens.FieldOfView;
            cam.m_Lens.FieldOfView = Plugin.FloatViewField.Value;
        }

        private void Update()
        {
            if (!charController.isOwned) return;
            var newPos = false;
            if (!manager.GamePaused)
            {
                Move();
                MoveHead();
                MouseRotate();
                MouseViewFieldControl();
                newPos = PositionControl();
                KeyFrameAnimControl();
                if (ShortcutInput.IsDown(Plugin.KeyViewRemoveRotationLimit))
                {
                    Plugin.BooleanViewRemoveRotationLimit.Value = !Plugin.BooleanViewRemoveRotationLimit.Value;
                }
            }
            ScalePlayer();
            AutoRotate();
            MouseViewField();
            KeyFrameAnim();
            if (newPos || ShortcutInput.IsDown(Plugin.KeyMoveResetPosition))
            {
                charController.HeadPivot.transform.localPosition = initHeadPosition;
                CharMoveablePatch.CinemachineTargetRoll = 0f;
                if (!newPos)
                {
                    charController.transform.localPosition = initBodyPosition;
                    charController.transform.localRotation = initBodyRotation;
                    if (Plugin.BooleanResetView.Value)
                    {
                        SetYaw(0f);
                        SetPitch(0f);
                        Plugin.FloatViewField.Value = defaultFOV;
                    }
                }
                groundY = charController.transform.position.y;
                playerY = groundY;
                inGround = true;
                isFling = false;
                fullV = 0f;
                animFrame = -1;
            }
        }

        private void ScalePlayer()
        {
            if (Plugin.FloatCustomPlayerScale.Value == 0.5f)
            {
                charController.transform.localScale = initScale;
            }
            else
            {
                var scale = (Plugin.FloatCustomPlayerScale.Value - 0.5f) * 8f;
                if (scale > 0f) scale += 1f;
                else scale = 1 / (1 - scale);
                charController.transform.localScale = initScale * scale;
            }
        }

        private void MoveHead()
        {
            if (!playerStats.Dead)
            {
                if (ShortcutInput.IsPressed(Plugin.KeyViewForward))
                    charController.HeadPivot.transform.Translate(Plugin.FloatViewSpeed.Value * Time.deltaTime * Vector3.up, Space.Self);//Y+
                if (ShortcutInput.IsPressed(Plugin.KeyViewBack))
                    charController.HeadPivot.transform.Translate(Plugin.FloatViewSpeed.Value * Time.deltaTime * Vector3.down, Space.Self);//Y-
                if (ShortcutInput.IsPressed(Plugin.KeyViewLeft))
                    charController.HeadPivot.transform.Translate(Plugin.FloatViewSpeed.Value * Time.deltaTime * Vector3.forward, Space.Self);//Z+
                if (ShortcutInput.IsPressed(Plugin.KeyViewRight))
                    charController.HeadPivot.transform.Translate(Plugin.FloatViewSpeed.Value * Time.deltaTime * Vector3.back, Space.Self);//Z-
                if (ShortcutInput.IsPressed(Plugin.KeyViewUp))
                    charController.HeadPivot.transform.Translate(Plugin.FloatViewSpeed.Value * Time.deltaTime * Vector3.left, Space.Self);//X-
                if (ShortcutInput.IsPressed(Plugin.KeyViewDown))
                    charController.HeadPivot.transform.Translate(Plugin.FloatViewSpeed.Value * Time.deltaTime * Vector3.right, Space.Self);//X+
            }
            else
            {
                var speccamindex = FastMemberAccessor<CharController, int>.Get(charController, "speccamindex");
                var trans = manager.SpectatorCameraParrent.transform.GetChild(speccamindex);
                if (ShortcutInput.IsPressed(Plugin.KeyViewForward))
                    trans.Translate(Plugin.FloatViewSpeed.Value * Time.deltaTime * Vector3.forward, Space.Self);
                if (ShortcutInput.IsPressed(Plugin.KeyViewBack))
                    trans.Translate(Plugin.FloatViewSpeed.Value * Time.deltaTime * Vector3.back, Space.Self);
                if (ShortcutInput.IsPressed(Plugin.KeyViewLeft))
                    trans.Translate(Plugin.FloatViewSpeed.Value * Time.deltaTime * Vector3.left, Space.Self);
                if (ShortcutInput.IsPressed(Plugin.KeyViewRight))
                    trans.Translate(Plugin.FloatViewSpeed.Value * Time.deltaTime * Vector3.right, Space.Self);
                if (ShortcutInput.IsPressed(Plugin.KeyViewUp))
                    trans.Translate(Plugin.FloatViewSpeed.Value * Time.deltaTime * Vector3.up, Space.Self);
                if (ShortcutInput.IsPressed(Plugin.KeyViewDown))
                    trans.Translate(Plugin.FloatViewSpeed.Value * Time.deltaTime * Vector3.down, Space.Self);
            }
        }

        private void MouseRotate()
        {
            var RotateYaw = ShortcutInput.IsPressed(Plugin.KeyRotateYaw);
            var RotatePitch = ShortcutInput.IsPressed(Plugin.KeyRotatePitch);
            var RotateRoll = ShortcutInput.IsPressed(Plugin.KeyRotateRoll);
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
                if (playerStats.Dead)
                    SetYaw(0f);
                else
                    AddYaw(x);
            }
            if (RotatePitch)
            {
                charController.transform.Rotate(y * Vector3.right, Space.Self);
                if (playerStats.Dead)
                    SetYaw(0f);
                else
                    AddPitch(-y);
            }
            var speccamindex = FastMemberAccessor<CharController, int>.Get(charController, "speccamindex");
            var trans = manager.SpectatorCameraParrent.transform.GetChild(speccamindex);
            if (playerStats.Dead && Plugin.BooleanViewRemoveRotationLimit.Value)
            {
                var rz = (ShortcutInput.IsPressed(Plugin.KeyViewAnticlockwise) ? 1f : 0f) + (ShortcutInput.IsPressed(Plugin.KeyViewClockwise) ? -1f : 0f);
                trans.Rotate(new Vector3(-y, x, rz), Space.Self);
            }
            if (!Plugin.BooleanViewRemoveRotationLimit.Value) return;
            if (!playerStats.Dead) return;
            trans.Rotate(new Vector3(-y, x, 0f), Space.Self);
        }

        private void MouseViewFieldControl()
        {
            if (Plugin.BooleanViewMouseViewField.Value)
            {
                var mouseScroll = Input.GetAxis("Mouse ScrollWheel");
                if (mouseScroll != 0f)
                {
                    Plugin.FloatViewField.Value += -50f * mouseScroll;
                }
            }
        }
        private void MouseViewField()
        {
            var d = Plugin.FloatViewField.Value - fov;
            if (d == 0f) return;
            if (Mathf.Abs(d) < 0.02f)
            {
                fov = Plugin.FloatViewField.Value;
            }
            else if (d > 0f)
            {
                fov += Mathf.Max(0.1f, d / 5);
            }
            else
            {
                fov += Mathf.Min(-0.1f, d / 5);
            }
            if (!playerStats.Dead)
            {
                cam.m_Lens.FieldOfView = fov;
            }
            else
            {
                for (var i = 0; i < manager.SpectatorCameraParrent.transform.childCount; i++)
                {
                    manager.SpectatorCameraParrent.transform.GetChild(i).gameObject.GetComponent<CinemachineVirtualCamera>().m_Lens.FieldOfView = fov;
                }
            }
        }

        private void AutoRotate()
        {
            if (ShortcutInput.IsDown(Plugin.KeyRotateAuto)) rotating = !rotating;
            if (!rotating || manager.GamePaused) return;
            var rotateSpeed = Plugin.FloatAutoRotateSpeed.Value * 6;
            if ((Plugin.DirectionRotateState.Value & RotateDirection.Pitch) != RotateDirection.None)
                charController.transform.Rotate(rotateSpeed * Vector3.right, Space.Self);
            if ((Plugin.DirectionRotateState.Value & RotateDirection.Roll) != RotateDirection.None)
                charController.transform.Rotate(rotateSpeed * Vector3.forward, Space.Self);
            if ((Plugin.DirectionRotateState.Value & RotateDirection.Yaw) != RotateDirection.None)
                charController.transform.Rotate(rotateSpeed * Vector3.up, Space.Self);
            if ((Plugin.DirectionRotateState.Value & RotateDirection.HeadYaw) != RotateDirection.None)
            {
                AddYaw(rotateSpeed);
            }
            if ((Plugin.DirectionRotateState.Value & RotateDirection.HeadPitch) != RotateDirection.None)
            {
                AddPitch(rotateSpeed);
            }
        }

        private void Move()
        {
            var run = ShortcutInput.IsPressed(Plugin.KeyMoveRun);
            var squat = ShortcutInput.IsPressed(Plugin.KeyMoveSquat);
            var jump = ShortcutInput.IsPressed(Plugin.KeyMoveJump);
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
                if (ShortcutInput.IsDown(Plugin.KeyMoveSquat))
                {
                    groundY -= 0.3f;
                    if (inGround && groundY < playerY)
                    {
                        inGround = false;
                    }
                }
                if (ShortcutInput.IsUp(Plugin.KeyMoveSquat))
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
                    if (fullV > 1f && ShortcutInput.IsDown(Plugin.KeyMoveJump))
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
            if (ShortcutInput.IsPressed(Plugin.KeyMoveForward))
            {
                charController.transform.Translate(moveSpeed * Time.deltaTime * WalkBodyRotate(Vector3.forward, 0f), Space.Self);
            }
            if (ShortcutInput.IsPressed(Plugin.KeyMoveBack))
            {
                charController.transform.Translate(moveSpeed * Time.deltaTime * WalkBodyRotate(Vector3.back, 0f), Space.Self);
            }
            if (ShortcutInput.IsPressed(Plugin.KeyMoveLeft))
            {
                charController.transform.Translate(moveSpeed * Time.deltaTime * WalkBodyRotate(Vector3.left, 0f), Space.Self);
            }
            if (ShortcutInput.IsPressed(Plugin.KeyMoveRight))
            {
                charController.transform.Translate(moveSpeed * Time.deltaTime * WalkBodyRotate(Vector3.right, 0f), Space.Self);
            }
        }

        private Vector3 WalkBodyRotate(Vector3 d, float toYaw)
        {
            if (!Plugin.BooleanMoveFollowHead.Value) return d;
            var yaw = GetYaw();
            var rotateAngle = (toYaw - yaw) / 5f;
            var newYaw = yaw + rotateAngle;
            if (Mathf.Abs(rotateAngle) > 1f)
            {
                charController.transform.Rotate(rotateAngle * Vector3.up, Space.Self);
                SetYaw(newYaw);
                return (Quaternion.AngleAxis(-newYaw, Vector3.up) * d);
            }
            else if (rotateAngle != 0f)
            {
                rotateAngle = toYaw - yaw;
                newYaw = yaw + rotateAngle;
                charController.transform.Rotate(rotateAngle * Vector3.up, Space.Self);
                SetYaw(newYaw);
                return (Quaternion.AngleAxis(-newYaw, Vector3.up) * d);
            }
            else
            {
                return (Quaternion.AngleAxis(-toYaw, Vector3.up) * d);
            }
        }

        private bool PositionControl()
        {
            var newPos = false;
            for (var i = 0; i < Plugin.InitPositionNumValue; i++)
            {
                if (ShortcutInput.IsDown(Plugin.KeyPosition[i]))
                {
                    charController.transform.localPosition = Plugin.VectorPosition[i].Value;
                    charController.transform.localRotation = Quaternion.Euler(
                        Plugin.VectorRotation[i].Value.x, Plugin.VectorRotation[i].Value.y, Plugin.VectorRotation[i].Value.z);
                    newPos = true;
                }
            }
            return newPos;
        }

        private void KeyFrameAnimControl()
        {
            for (var i = 0; i < Plugin.InitAnimationNumValue; i++)
            {
                if (ShortcutInput.IsDown(Plugin.KeyAnims[i]))
                {
                    ReadAnim(i);
                    if (usingAnim.Count > 1)
                    {
                        animStartPosition = charController.transform.localPosition;
                        animStartRotation = charController.transform.localRotation;
                        animFrame = 0;
                    }
                    break;
                }
            }
        }

        private void ReadAnim(int index)
        {
            var name = Plugin.StringAnims[index].Value;
            var txt = System.Text.Encoding.Default.GetString(File.ReadAllBytes(AppDomain.CurrentDomain.BaseDirectory + "\\" + name));
            var lines = txt.Split('\n');
            usingAnim.Clear();
            foreach (var line in lines)
            {
                try
                {
                    var args = line.Split([' ', '\t'], StringSplitOptions.RemoveEmptyEntries);
                    if (args.Length < 7) continue;
                    var keyFrame = new KeyFrame
                    {
                        frame = int.Parse(args[0]),
                        position = new Vector3(float.Parse(args[1]), float.Parse(args[2]), float.Parse(args[3])),
                        rotation = new Vector3(float.Parse(args[4]), float.Parse(args[5]), float.Parse(args[6]))
                    };
                    usingAnim.Add(keyFrame);
                }
                catch (Exception) { continue; }
            }
            if (usingAnim.Count > 0 && usingAnim.First().frame > 0)
            {
                var keyFrame = new KeyFrame
                {
                    frame = 0,
                    position = usingAnim.First().position,
                    rotation = usingAnim.First().rotation
                };
                usingAnim.Insert(0, keyFrame);
            }
        }

        private void KeyFrameAnim()
        {
            if (animFrame < 0) return;
            if (animFrame >= usingAnim.Last().frame)
            {
                usingAnim.Clear();
                animFrame = -1;
                return;
            }
            for (var index = 0; index < usingAnim.Count - 1; index++)
            {
                var item = usingAnim[index + 1];
                var animKey = item.frame;
                if (animFrame < animKey)
                {
                    var thisItem = usingAnim[index];
                    var thisKey = thisItem.frame;
                    var progress = ((float)animFrame - thisKey) / (animKey - thisKey);
                    var thisPosition = thisItem.position;
                    var thisRotation = thisItem.rotation;
                    var animPosition = item.position;
                    var animRotation = item.rotation;
                    var framePosition = thisPosition + progress * (animPosition - thisPosition);
                    var frameRotation = thisRotation + progress * (animRotation - thisRotation);
                    charController.transform.localPosition = animStartPosition;
                    charController.transform.Translate(framePosition, Space.Self);
                    charController.transform.localRotation = animStartRotation;
                    charController.transform.Rotate(frameRotation, Space.Self);
                    break;
                }
            }
            ++animFrame;
        }

        public float GetYaw()
        {
            return (float)AccessTools.Field(typeof(CharController), "_cinemachineTargetYaw").GetValue(charController);
        }
        public void SetYaw(float value)
        {
            AccessTools.Field(typeof(CharController), "_cinemachineTargetYaw").SetValue(charController, value);
        }
        public void AddYaw(float value)
        {
            SetYaw(GetYaw() + value);
        }
        public float GetPitch()
        {
            return (float)AccessTools.Field(typeof(CharController), "_cinemachineTargetPitch").GetValue(charController);
        }
        public void SetPitch(float value)
        {
            AccessTools.Field(typeof(CharController), "_cinemachineTargetPitch").SetValue(charController, value);
        }
        public void AddPitch(float value)
        {
            SetPitch(GetPitch() + value);
        }

        internal class KeyFrame
        {
            public int frame;
            public Vector3 position;
            public Vector3 rotation;
        }

        private void OnGUI()
        {
            if (ShortcutInput.IsPressed(Plugin.KeyCustomShowHint))
            {
                var yes = "<color=#00FF00>是</color>";
                var no = "<color=#FF0000>否</color>";
                GUI.Label(new Rect(Screen.width - 240, 60, 240, 300),
                    $"按 {HintKey(Plugin.KeyCustomShowHint)} 显示提示\n" +
                    $"按 {HintKey(Plugin.KeyMoveResetPosition)} 重置位置\n" +
                    $"    重置位置时重置视角: {(Plugin.BooleanResetView.Value ? yes : no)}\n" +
                    $"按 {HintKey(Plugin.KeyViewRemoveRotationLimit)} 切换解除视角限制\n" +
                    $"    解除视角限制: {(Plugin.BooleanViewRemoveRotationLimit.Value ? yes : no)}\n" +
                    $"按住 {HintKey(Plugin.KeyViewCrazyShakeHead)} 疯狂转头\n" +
                    $"按住 {HintKey(Plugin.KeyCustomBigMouth)} 张嘴\n" +
                    "(默认)854671头部移动\n" +
                    "(默认)↑↓←→身体移动\n" +
                    $"按 {HintKey(Plugin.KeyMoveJump)} 跳跃\n" +
                    $"按 {HintKey(Plugin.KeyMoveSquat)} 蹲下\n" +
                    $"按 {HintKey(Plugin.KeyMoveRun)} 奔跑\n" +
                    $"按住 {HintKey(Plugin.KeyRotateYaw)} 水平转动身体\n" +
                    $"按住 {HintKey(Plugin.KeyRotatePitch)} 垂直(前后)转动身体\n" +
                    $"按住 {HintKey(Plugin.KeyRotateRoll)} 垂直(左右)转动身体",
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

using HarmonyLib;

using LiarsBarEnhance.Utils;

using UnityEngine;

namespace LiarsBarEnhance.Features;

[HarmonyPatch]
public class CharMoveablePatch
{
    private static float groundY;
    private static float playerY;
    private static bool inGround = true;
    private static bool isFling = false;
    private static float fullV = 0f;
    private static Vector3 initHeadPosition;
    private static Vector3 initBodyPosition;
    private static Quaternion initBodyRotation;

    [HarmonyPatch(typeof(CharController), "Start")]
    [HarmonyPostfix]
    public static void StartPostfix(CharController __instance)
    {
        if (!__instance.isOwned) return;
        RemoveHeadRotationlimitPatch.CinemachineTargetRoll = 0f;
        initHeadPosition = __instance.HeadPivot.transform.localPosition;
        initBodyPosition = __instance.transform.localPosition;
        initBodyRotation = __instance.transform.localRotation;
        groundY = __instance.transform.position.y;
        playerY = groundY;
    }

    [HarmonyPatch(typeof(CharController), "Update")]
    [HarmonyPostfix]
    public static void UpdatePostfix(CharController __instance)
    {
        if (!__instance.isOwned) return;
        if (__instance.manager().PluginControl())
        {
            MoveHead(__instance);
            MouseRotate(__instance);
            Move(__instance);
        }
        if (Plugin.KeyMoveResetPosition.IsDown())
        {
            __instance.transform.localPosition = initBodyPosition;
            __instance.transform.localRotation = initBodyRotation;
            groundY = __instance.transform.position.y;
            playerY = groundY;
            inGround = true;
            isFling = false;
            fullV = 0f;
            AnimationPatch.AnimFrame = -1;
        }
        if (Plugin.KeyViewReset.IsDown())
        {
            __instance.HeadPivot.transform.localPosition = initHeadPosition;
            __instance.SetYaw(0f);
            __instance.SetPitch(0f);
            RemoveHeadRotationlimitPatch.CinemachineTargetRoll = 0f;
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

    private static void MoveHead(CharController charController)
    {
        if (!charController.playerStats().Dead || !Plugin.BooleanViewRemoveRotationLimit.Value)
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
            var trans = charController.manager().SpectatorCameraParrent.transform.GetChild(speccamindex);
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

    private static void MouseRotate(CharController charController)
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
            if (!charController.playerStats().Dead) charController.AddYaw(x);
        }
        else if (charController.playerStats().Dead && !Plugin.BooleanViewRemoveRotationLimit.Value)
        {
            charController.AddYaw(-x);
        }
        if (RotatePitch)
        {
            charController.transform.Rotate(y * Vector3.right, Space.Self);
            if (!charController.playerStats().Dead) charController.AddPitch(-y);
        }
        else if (charController.playerStats().Dead && !Plugin.BooleanViewRemoveRotationLimit.Value)
        {
            charController.AddPitch(y);
        }
        var speccamindex = FastMemberAccessor<CharController, int>.Get(charController, "speccamindex");
        var trans = charController.manager().SpectatorCameraParrent.transform.GetChild(speccamindex);
        if (charController.playerStats().Dead && Plugin.BooleanViewRemoveRotationLimit.Value)
        {
            var rz = (Plugin.KeyViewAnticlockwise.IsPressed() ? 2f : 0f) + (Plugin.KeyViewClockwise.IsPressed() ? -2f : 0f);
            trans.Rotate(new Vector3(-y, x, rz), Space.Self);
        }
    }

    private static void Move(CharController charController)
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
            charController.transform.Translate(moveSpeed * Time.deltaTime * WalkBodyRotate(charController, Vector3.forward, 0f), Space.Self);
        }
        if (Plugin.KeyMoveBack.IsPressed())
        {
            charController.transform.Translate(moveSpeed * Time.deltaTime * WalkBodyRotate(charController, Vector3.back, 0f), Space.Self);
        }
        if (Plugin.KeyMoveLeft.IsPressed())
        {
            charController.transform.Translate(moveSpeed * Time.deltaTime * WalkBodyRotate(charController, Vector3.left, -Plugin.FloatMoveHorizontalBodyRotate.Value), Space.Self);
        }
        if (Plugin.KeyMoveRight.IsPressed())
        {
            charController.transform.Translate(moveSpeed * Time.deltaTime * WalkBodyRotate(charController, Vector3.right, Plugin.FloatMoveHorizontalBodyRotate.Value), Space.Self);
        }
    }

    private static Vector3 WalkBodyRotate(CharController charController, Vector3 d, float toYaw)
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
}
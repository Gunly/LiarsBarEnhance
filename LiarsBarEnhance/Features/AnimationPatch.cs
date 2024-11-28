using BepInEx;

using HarmonyLib;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using UnityEngine;

namespace LiarsBarEnhance.Features;

[HarmonyPatch]
public class AnimationPatch
{
    private static readonly List<KeyFrame> usingAnim = [];
    public static int AnimFrame = -1;
    private static Vector3 animStartPosition;
    private static Quaternion animStartRotation;
    private static readonly string[] charAnimBoolsBlorf = ["Look", "Throw", "Dead", "Roulet", "HaveCard", "Reload", "Winner"];
    private static readonly string[] charAnimBoolsDice = ["Look", "Show", "Drink", "Dead", "Winner"];
    private static readonly string[] charAnimBoolsChaos = ["Look", "Throw", "Dead", "Roulet", "HaveCard", "Reload", "Winner", "Fire", "Empty", "TakeAim"];

    [HarmonyPatch(typeof(CharController), nameof(CharController.Update))]
    [HarmonyPostfix]
    public static void UpdatePostfix(CharController __instance, Manager ___manager, PlayerStats ___playerStats)
    {
        if (!__instance.isOwned) return;
        if (!___manager.GamePaused && !___manager.Chatting)
        {
            if (!___playerStats.HaveTurn)
            {
                if (Input.GetKeyDown(KeyCode.Space)) __instance.animator.SetBool("Look", true);
                if (Input.GetKeyUp(KeyCode.Space)) __instance.animator.SetBool("Look", false);
            }
            if (Plugin.KeyAnimCallLiar.IsDown()) __instance.animator.SetTrigger("CallLiar");
            if (__instance is BlorfGamePlay)
            {
                if (Plugin.KeyAnimThrow.IsDown()) __instance.animator.SetBool("Throw", true);
                if (Plugin.KeyAnimThrow.IsUp()) __instance.animator.SetBool("Throw", false);
                if (Plugin.KeyAnimRoulet.IsDown()) __instance.animator.SetBool("Roulet", true);
                if (Plugin.KeyAnimRoulet.IsUp()) __instance.animator.SetBool("Roulet", false);
                if (Plugin.KeyAnimReload.IsDown()) __instance.animator.SetBool("Reload", true);
                if (Plugin.KeyAnimReload.IsUp()) __instance.animator.SetBool("Reload", false);
            }
            else if (__instance is DiceGamePlay)
            {
                if (Plugin.KeyAnimSpotOn.IsDown()) __instance.animator.SetTrigger("SpotOn");
                if (Plugin.KeyAnimShake.IsDown()) __instance.animator.SetTrigger("Shake");
                if (Plugin.KeyAnimShow.IsPressed()) __instance.animator.SetBool("Show", true);
                if (Plugin.KeyAnimShow.IsUp()) __instance.animator.SetBool("Show", false);
                if (Plugin.KeyAnimDrink.IsDown()) __instance.animator.SetBool("Drink", true);
                if (Plugin.KeyAnimDrink.IsUp()) __instance.animator.SetBool("Drink", false);
            }
            else if (__instance is ChaosGamePlay chaosGame)
            {
                if (Plugin.KeyAnimThrow.IsDown()) __instance.animator.SetBool("Throw", true);
                if (Plugin.KeyAnimThrow.IsUp()) __instance.animator.SetBool("Throw", false);
                if (Plugin.KeyAnimRoulet.IsDown()) __instance.animator.SetBool("Roulet", true);
                if (Plugin.KeyAnimRoulet.IsUp()) __instance.animator.SetBool("Roulet", false);
                if (Plugin.KeyAnimReload.IsDown()) __instance.animator.SetBool("Reload", true);
                if (Plugin.KeyAnimReload.IsUp()) __instance.animator.SetBool("Reload", false);
                if (Plugin.KeyAnimTakeAim.IsDown()) __instance.animator.SetBool("TakeAim", true);
                if (Plugin.KeyAnimTakeAim.IsUp()) __instance.animator.SetBool("TakeAim", false);
                if (!chaosGame.TakingAim && __instance.animator.GetBool("TakeAim"))
                {
                    var lookDirection = __instance.animator.GetInteger("LookDirection");
                    if (Input.GetKeyDown(KeyCode.A))
                    {
                        if (lookDirection >= 0) lookDirection--;
                    }
                    else if (Input.GetKeyDown(KeyCode.D))
                    {
                        if (lookDirection <= 0) lookDirection++;
                    }
                    __instance.animator.SetInteger("LookDirection", lookDirection);
                }
                if (Plugin.KeyAnimFire.IsDown()) __instance.animator.SetBool("Fire", true);
                if (Plugin.KeyAnimFire.IsUp()) __instance.animator.SetBool("Fire", false);
                if (Plugin.KeyAnimEmpty.IsDown()) __instance.animator.SetBool("Empty", true);
                if (Plugin.KeyAnimEmpty.IsUp()) __instance.animator.SetBool("Empty", false);
            }
            for (var i = 0; i < Plugin.InitAnimationNumValue; i++)
            {
                if (Plugin.KeyAnims[i].IsDown())
                {
                    if (__instance is BlorfGamePlay && charAnimBoolsBlorf.Contains(Plugin.StringAnims[i].Value))
                    {
                        __instance.animator.SetBool(Plugin.StringAnims[i].Value, !__instance.animator.GetBool(Plugin.StringAnims[i].Value));
                    }
                    else if (__instance is DiceGamePlay && charAnimBoolsDice.Contains(Plugin.StringAnims[i].Value))
                    {
                        __instance.animator.SetBool(Plugin.StringAnims[i].Value, !__instance.animator.GetBool(Plugin.StringAnims[i].Value));
                    }
                    else if (__instance is ChaosGamePlay && charAnimBoolsChaos.Contains(Plugin.StringAnims[i].Value))
                    {
                        __instance.animator.SetBool(Plugin.StringAnims[i].Value, !__instance.animator.GetBool(Plugin.StringAnims[i].Value));
                    }
                    else
                    {
                        ReadAnim(i);
                        if (usingAnim.Count > 1)
                        {
                            animStartPosition = __instance.transform.localPosition;
                            animStartRotation = __instance.transform.localRotation;
                            AnimFrame = 0;
                        }
                    }
                    break;
                }
            }
        }
        if (AnimFrame < 0) return;
        if (AnimFrame >= usingAnim.Last().frame)
        {
            usingAnim.Clear();
            AnimFrame = -1;
            return;
        }
        for (var index = 0; index < usingAnim.Count - 1; index++)
        {
            var item = usingAnim[index + 1];
            var animKey = item.frame;
            if (AnimFrame < animKey)
            {
                var thisItem = usingAnim[index];
                var thisKey = thisItem.frame;
                var progress = ((float)AnimFrame - thisKey) / (animKey - thisKey);
                var thisPosition = thisItem.position;
                var thisRotation = thisItem.rotation;
                var animPosition = item.position;
                var animRotation = item.rotation;
                var framePosition = thisPosition + progress * (animPosition - thisPosition);
                var frameRotation = thisRotation + progress * (animRotation - thisRotation);
                __instance.transform.localPosition = animStartPosition;
                __instance.transform.Translate(framePosition, Space.Self);
                __instance.transform.localRotation = animStartRotation;
                __instance.transform.Rotate(frameRotation, Space.Self);
                break;
            }
        }
        ++AnimFrame;
    }

    private static void ReadAnim(int index)
    {
        var name = Plugin.StringAnims[index].Value;
        var txt = System.Text.Encoding.Default.GetString(File.ReadAllBytes(Path.Combine(Paths.GameRootPath, name)));
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

    internal class KeyFrame
    {
        public int frame;
        public Vector3 position;
        public Vector3 rotation;
    }
}
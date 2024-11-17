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

    [HarmonyPatch(typeof(CharController), nameof(CharController.Update))]
    [HarmonyPostfix]
    public static void UpdatePostfix(CharController __instance)
    {
        if (!__instance.isOwned) return;
        for (var i = 0; i < Plugin.InitAnimationNumValue; i++)
        {
            if (Plugin.KeyAnims[i].IsDown())
            {
                ReadAnim(i);
                if (usingAnim.Count > 1)
                {
                    animStartPosition = __instance.transform.localPosition;
                    animStartRotation = __instance.transform.localRotation;
                    AnimFrame = 0;
                }
                break;
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
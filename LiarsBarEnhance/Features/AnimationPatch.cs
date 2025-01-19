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
    public static void UpdatePostfix(CharController __instance)
    {
        if (!__instance.isOwned) return;
        if (__instance.manager.PluginControl())
        {
            if (!__instance.playerStats.HaveTurn)
            {
                if (Input.GetKeyDown(KeyCode.Space)) __instance.animator.SetBool("Look", true);
                if (Input.GetKeyUp(KeyCode.Space)) __instance.animator.SetBool("Look", false);
            }
            if (Plugin.KeyAnimCallLiar.IsDown()) __instance.animator.SetTrigger("CallLiar");
            if (__instance.manager.mode == CustomNetworkManager.GameMode.LiarsDeck)
            {
                if (Plugin.KeyAnimThrow.IsDown()) __instance.animator.SetBool("Throw", true);
                if (Plugin.KeyAnimThrow.IsUp()) __instance.animator.SetBool("Throw", false);
                if (Plugin.KeyAnimRoulet.IsDown()) __instance.animator.SetBool("Roulet", true);
                if (Plugin.KeyAnimRoulet.IsUp())
                {
#if CHEATRELEASE
                    var blorfGame = __instance.playerStats.GetComponent<BlorfGamePlay>();
                    if (blorfGame)
                    {
                        if (Plugin.RouletAnimType.Value == RouletType.Roulet && blorfGame.Networkcurrentrevoler < blorfGame.Networkrevolverbulllet)
                        {
                            blorfGame.Networkcurrentrevoler++;
                        }
                        else if (Plugin.RouletAnimType.Value == RouletType.Suicide ||
                            (Plugin.RouletAnimType.Value == RouletType.Roulet && blorfGame.Networkcurrentrevoler == blorfGame.Networkrevolverbulllet))
                        {
                            __instance.animator.SetBool("Dead", true);
                            blorfGame.StartCoroutine("waitforheadopen");
                            blorfGame.CommandBeDead();
                            blorfGame.StartCoroutine("WaitforRevolverUI");
                        }
                    }
                    else
                    {
                        var blorfGameMatchMaking = __instance.playerStats.GetComponent<BlorfGamePlayMatchMaking>();
                        if (Plugin.RouletAnimType.Value == RouletType.Roulet && blorfGameMatchMaking.Networkcurrentrevoler < blorfGameMatchMaking.Networkrevolverbulllet)
                        {
                            blorfGameMatchMaking.Networkcurrentrevoler++;
                        }
                        else if (Plugin.RouletAnimType.Value == RouletType.Suicide ||
                            (Plugin.RouletAnimType.Value == RouletType.Roulet && blorfGameMatchMaking.Networkcurrentrevoler == blorfGameMatchMaking.Networkrevolverbulllet))
                        {
                            __instance.animator.SetBool("Dead", true);
                            blorfGameMatchMaking.StartCoroutine("waitforheadopen");
                            blorfGameMatchMaking.CommandBeDead();
                            blorfGameMatchMaking.StartCoroutine("WaitforRevolverUI");
                        }
                    }
#endif
                    __instance.animator.SetBool("Roulet", false);
                }
                if (Plugin.KeyAnimReload.IsDown()) __instance.animator.SetBool("Reload", true);
                if (Plugin.KeyAnimReload.IsUp()) __instance.animator.SetBool("Reload", false);
            }
            else if (__instance.manager.mode == CustomNetworkManager.GameMode.LiarsDice)
            {
                if (Plugin.KeyAnimSpotOn.IsDown()) __instance.animator.SetTrigger("SpotOn");
                if (Plugin.KeyAnimShake.IsDown()) __instance.animator.SetTrigger("Shake");
                if (Plugin.KeyAnimShow.IsPressed()) __instance.animator.SetBool("Show", true);
                if (Plugin.KeyAnimShow.IsUp()) __instance.animator.SetBool("Show", false);
                if (Plugin.KeyAnimDrink.IsDown()) __instance.animator.SetBool("Drink", true);
                if (Plugin.KeyAnimDrink.IsUp())
                {
#if CHEATRELEASE
                    if (Plugin.RouletAnimType.Value == RouletType.Roulet && __instance.playerStats.NetworkHealth == 2)
                    {
                        __instance.playerStats.NetworkHealth = 1;
                    }
                    else if (Plugin.RouletAnimType.Value == RouletType.Suicide ||
                        (Plugin.RouletAnimType.Value == RouletType.Roulet && __instance.playerStats.NetworkHealth == 1))
                    {
                        __instance.playerStats.NetworkHealth = 0;
                        __instance.playerStats.NetworkDead = true;
                    }
#endif
                    __instance.animator.SetBool("Drink", false);
                }
            }
            else if (__instance.manager.mode == CustomNetworkManager.GameMode.LiarsChaos)
            {
                var chaosGame = __instance.playerStats.GetComponent<ChaosGamePlay>();
                if (Plugin.KeyAnimThrow.IsDown()) __instance.animator.SetBool("Throw", true);
                if (Plugin.KeyAnimThrow.IsUp()) __instance.animator.SetBool("Throw", false);
                if (Plugin.KeyAnimRoulet.IsDown()) __instance.animator.SetBool("Roulet", true);
                if (Plugin.KeyAnimRoulet.IsUp())
                {
#if CHEATRELEASE
                    if (Plugin.RouletAnimType.Value == RouletType.Roulet && chaosGame.Networkcurrentrevoler < chaosGame.Networkrevolverbulllet)
                    {
                        chaosGame.Networkcurrentrevoler++;
                    }
                    else if (Plugin.RouletAnimType.Value == RouletType.Suicide ||
                        (Plugin.RouletAnimType.Value == RouletType.Roulet && chaosGame.Networkcurrentrevoler == chaosGame.Networkrevolverbulllet))
                    {
                        __instance.animator.SetBool("Dead", true);
                        chaosGame.StartCoroutine("waitforheadopen");
                        chaosGame.CommandBeDead();
                        chaosGame.StartCoroutine("WaitforRevolverUI");
                    }
#endif
                    __instance.animator.SetBool("Roulet", false);
                }
                if (Plugin.KeyAnimReload.IsDown()) __instance.animator.SetBool("Reload", true);
                if (Plugin.KeyAnimReload.IsUp()) __instance.animator.SetBool("Reload", false);
                if (Plugin.KeyAnimTakeAim.IsDown())
                {
                    __instance.animator.SetInteger("LookDirection", 0);
                    __instance.animator.SetBool("TakeAim", true);
                }
                if (!chaosGame.TakingAim && __instance.animator.GetBool("TakeAim"))
                {
                    if (Input.GetKeyDown(KeyCode.A))
                    {
                        var lookDirection = __instance.animator.GetInteger("LookDirection");
                        if (lookDirection >= 0)
                        {
                            lookDirection--;
                            __instance.animator.SetInteger("LookDirection", lookDirection);
                            chaosGame.NetworkAim = lookDirection;
                        }
                    }
                    else if (Input.GetKeyDown(KeyCode.D))
                    {
                        var lookDirection = __instance.animator.GetInteger("LookDirection");
                        if (lookDirection <= 0)
                        {
                            lookDirection++;
                            __instance.animator.SetInteger("LookDirection", lookDirection);
                            chaosGame.NetworkAim = lookDirection;
                        }
                    }
#if CHEATRELEASE
                    if (Plugin.RouletAnimType.Value == RouletType.Roulet && chaosGame.Networkcurrentrevoler < chaosGame.Networkrevolverbulllet)
                    {
                        chaosGame.Networkcurrentrevoler++;
                        if (Plugin.KeyAnimFire.IsDown() || Plugin.KeyAnimEmpty.IsDown())
                        {
                            __instance.animator.SetBool("Empty", true);
                        }
                    }
                    else if (Plugin.RouletAnimType.Value == RouletType.Suicide ||
                        (Plugin.RouletAnimType.Value == RouletType.Roulet && chaosGame.Networkcurrentrevoler == chaosGame.Networkrevolverbulllet))
                    {
                        if (Plugin.KeyAnimFire.IsDown() || Plugin.KeyAnimEmpty.IsDown())
                        {
                            __instance.animator.SetBool("Fire", true);
                            chaosGame.Networkcurrentrevoler = 0;
                            chaosGame.Networkrevolverbulllet = UnityEngine.Random.Range(0, 6);
                            chaosGame.HitTargetCmd();
                        }
                    }
                    else
                    {
                        if (Plugin.KeyAnimFire.IsDown())
                        {
                            __instance.animator.SetBool("Fire", true);
                        }
                        if (Plugin.KeyAnimEmpty.IsDown())
                        {
                            __instance.animator.SetBool("Empty", true);
                        }
                    }
                    if (Plugin.KeyAnimFire.IsUp() || Plugin.KeyAnimEmpty.IsUp())
                    {
                        __instance.animator.SetBool("Fire", false);
                        __instance.animator.SetBool("Empty", false);
                        __instance.animator.SetBool("TakeAim", false);
                    }
#else
                    if (Plugin.KeyAnimFire.IsDown())
                    {
                        __instance.animator.SetBool("Fire", true);
                    }
                    if (Plugin.KeyAnimFire.IsUp())
                    {
                        __instance.animator.SetBool("Fire", false);
                        __instance.animator.SetBool("TakeAim", false);
                    }
                    if (Plugin.KeyAnimEmpty.IsDown())
                    {
                        __instance.animator.SetBool("Empty", true);
                    }
                    if (Plugin.KeyAnimEmpty.IsUp())
                    {
                        __instance.animator.SetBool("Empty", false);
                        __instance.animator.SetBool("TakeAim", false);
                    }
#endif
                }
            }
            for (var i = 0; i < Plugin.IntAnimationNum.Value; i++)
            {
                if (Plugin.KeyAnims[i].IsDown())
                {
                    if (__instance.manager.mode == CustomNetworkManager.GameMode.LiarsDeck && charAnimBoolsBlorf.Contains(Plugin.StringAnims[i].Value))
                    {
                        __instance.animator.SetBool(Plugin.StringAnims[i].Value, !__instance.animator.GetBool(Plugin.StringAnims[i].Value));
                    }
                    else if (__instance.manager.mode == CustomNetworkManager.GameMode.LiarsDice && charAnimBoolsDice.Contains(Plugin.StringAnims[i].Value))
                    {
                        __instance.animator.SetBool(Plugin.StringAnims[i].Value, !__instance.animator.GetBool(Plugin.StringAnims[i].Value));
                    }
                    else if (__instance.manager.mode == CustomNetworkManager.GameMode.LiarsChaos && charAnimBoolsChaos.Contains(Plugin.StringAnims[i].Value))
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
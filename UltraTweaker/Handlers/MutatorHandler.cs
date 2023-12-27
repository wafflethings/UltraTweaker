﻿using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UltraTweaker.Subsettings;
using UltraTweaker.Tweaks;
using UnityEngine;

namespace UltraTweaker.Handlers
{
    internal static class MutatorHandler
    {
        public static Harmony harmony = new($"{UltraTweaker.GUID}.mutator_handler");

        public static void Patch()
        {
            harmony.PatchAll(typeof(MutatorPatches));
        }

        public class MutatorPatches
        {
            [HarmonyPatch(typeof(LeaderboardController), nameof(LeaderboardController.SubmitCyberGrindScore))]
            [HarmonyPatch(typeof(LeaderboardController), nameof(LeaderboardController.SubmitLevelScore))]
            [HarmonyPrefix]
            public static bool DisableCG()
            {
                foreach (Tweak tw in UltraTweaker.AllTweaks.Values)
                {
                    TweakMetadata tm = Attribute.GetCustomAttribute(tw.GetType(), typeof(TweakMetadata)) as TweakMetadata;
                    if (tw.IsEnabled && !tm.AllowCG)
                    {
                        Debug.Log("A no-CG tweak has been detected, disable CG ‼️");
                        return false;
                    }
                }

                return true;
            }

            [HarmonyPatch(typeof(FinalRank), nameof(FinalRank.SetInfo)), HarmonyPostfix]
            private static void ShowStuff()
            {
                string data = "";
                int limitPerLine = 0;
                foreach (Tweak tw in UltraTweaker.AllTweaks.Values)
                {
                    if (tw.IsEnabled)
                    {
                        TweakMetadata tm = Attribute.GetCustomAttribute(tw.GetType(), typeof(TweakMetadata)) as TweakMetadata;
                        if (tm.IsMutator)
                        {
                            string props = "";
                            limitPerLine += 1;
                            foreach (Subsetting sub in tw.Subsettings.Values)
                            {

                                props += sub.Serialize();
                                if (sub != tw.Subsettings.Values.Last())
                                {
                                    props += ", ";
                                }
                            }

                            if (tw.Subsettings.Values.Count > 0)
                            {
                                data += $"[{tm.Name}: {props}] ";
                            }
                            else
                            {
                                data += $"[{tm.Name}] ";
                            }

                            if (limitPerLine == 4 && tw == UltraTweaker.AllTweaks.Values.Last())
                            {
                                data += "\n";
                                limitPerLine = 0;
                            }
                        }
                    }
                }

                if (data != "")
                {
                    HudMessageReceiver.Instance.SendHudMessage(data);
                    HudMessageReceiver.Instance.text.fontSizeMax = HudMessageReceiver.Instance.text.fontSize;
                    HudMessageReceiver.Instance.text.fontSizeMin = 8;
                }
            }

            [HarmonyPatch(typeof(StatsManager), nameof(StatsManager.StartTimer)), HarmonyPostfix]
            public static void DisableMutators()
            {
                if (!CheatsController.Instance.cheatsEnabled)
                {
                    foreach (Tweak tw in UltraTweaker.AllTweaks.Values)
                    {
                        TweakMetadata meta = Attribute.GetCustomAttribute(tw.GetType(), typeof(TweakMetadata)) as TweakMetadata;
                        if (meta.IsMutator)
                        {
                            tw.SetControls(false);
                        }
                    }
                }
            }

            [HarmonyPatch(typeof(CheatsController), nameof(CheatsController.ActivateCheats)), HarmonyPostfix]
            public static void EnableOnCheats()
            {
                foreach (Tweak tw in UltraTweaker.AllTweaks.Values)
                {
                    TweakMetadata meta = Attribute.GetCustomAttribute(tw.GetType(), typeof(TweakMetadata)) as TweakMetadata;
                    if (meta.IsMutator)
                    {
                        tw.SetControls(true);
                    }
                }
            }
        }
    }
}

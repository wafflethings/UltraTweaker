﻿using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using UltraTweaker.Subsettings.Impl;
using UnityEngine;
using UltraTweaker.UIElements.Impl;

namespace UltraTweaker.Tweaks.Impl
{
    [TweakMetadata("Fragility", $"{UltraTweaker.GUID}.mutator_fragility", "Change your max health.", $"{UltraTweaker.GUID}.mutators", 3, "Fragility", true, true)]
    public class Fragility : Tweak
    {
        private Harmony _harmony = new($"{UltraTweaker.GUID}.mutator_fragility");

        public Fragility()
        {
            Subsettings = new()
            {
                { "max_health", new IntSubsetting(this, new Metadata("Max Health", "max_health", "Maximum player health."),
                    new SliderIntSubsettingElement("{0}"), 25, 100, 1) }
            };
        }

        public override void OnTweakEnabled()
        {
            base.OnTweakEnabled();
            _harmony.PatchAll(typeof(FragilityPatches));
        }

        public override void OnTweakDisabled()
        {
            base.OnTweakDisabled();
            _harmony.UnpatchSelf();
        }

        public static class FragilityPatches
        {
            [HarmonyPatch(typeof(NewMovement), nameof(NewMovement.Update)), HarmonyPostfix]
            private static void LimitHp(NewMovement __instance)
            {
                int thing = GetInstance<Fragility>().Subsettings["max_health"].GetValue<int>();

                if (__instance.antiHp < 100 - thing)
                {
                    __instance.antiHp = 100 - thing;
                }

                if (__instance.hp > thing)
                {
                    __instance.hp = thing;
                }
            }
        }
    }
}

using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using UltraTweaker.Tweaks;
using UnityEngine;

namespace Extension.Tweaks
{
    [TweakMetadata("Test Tweak", $"{Extension.GUID}.tweak", "This is a tweak.", $"{Extension.GUID}.ext_page", 0)]
    public class TestTweak : Tweak
    {
        private Harmony harmony = new($"{Extension.GUID}.tweak");
        protected float CoolSubsettingValue;

        public TestTweak()
        {
            Subsettings = new()
            {
                { "cool_subsetting", new FloatSubsetting(this, new Metadata("Subsetting", "cool_subsetting", "This is a subsetting."),
                    new SliderFloatSubsettingElement("{0} :)", 1), 0.5f, 1, 0) }
            };
        }

        public override void OnTweakEnabled()
        {
            base.OnTweakEnabled();
            CoolSubsettingValue = Subsettings["cool_subsetting"].GetValue<float>();
            harmony.PatchAll(typeof(TestTweakPatches));
        }

        public override void OnTweakDisabled()
        {
            base.OnTweakDisabled();
            harmony.UnpatchSelf();
        }

        public override void OnSubsettingUpdate()
        {
            CoolSubsettingValue = Subsettings["cool_subsetting"].GetValue<float>();
        }

        public class TestTweakPatches
        {
            [HarmonyPatch(typeof(NewMovement), nameof(NewMovement.Start)), HarmonyPostfix]
            private static void ShowOnSpawn()
            {
                if (IsGameplayScene())
                {
                    float val = GetInstance<TestTweak>().CoolSubsettingValue;
                    HudMessageReceiver.Instance.SendHudMessage($"Test tweak: {val}.");
                }
            }
        }
    }
}

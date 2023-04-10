using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using UltraTweaker.Handlers;
using UltraTweaker.Tweaks;
using UnityEngine;

namespace Extension.Tweaks
{
    // All the metadata for the tweak. Needed for the mod to find the tweak.
    [TweakMetadata("Test Tweak", $"{Extension.GUID}.tweak", "This is a tweak.", $"{Extension.GUID}.ext_page", 0, "TestIcon")]
    public class TestTweak : Tweak // All tweaks must inherit `Tweak`.
    {
        private Harmony harmony = new($"{Extension.GUID}.tweak");
        protected float CoolSubsettingValue;

        // All subsettings must be set in the constructor.
        public TestTweak()
        {
            // Caches the TestIcon from the Assets bundle, so that it doesn't have to be loaded more than once.
            AssetHandler.CacheAsset<Sprite>("TestIcon", Extension.Assets);

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

        // This will update the value when it is changed.
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
                    // You use GetInstance<T> to get the instance of a tweak.
                    float val = GetInstance<TestTweak>().CoolSubsettingValue;
                    HudMessageReceiver.Instance.SendHudMessage($"Test tweak: {val}.");
                }
            }
        }
    }
}

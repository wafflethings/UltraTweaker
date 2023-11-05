using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Text;
using UltraTweaker.Subsettings.Impl;
using UltraTweaker.UIElements.Impl;

namespace UltraTweaker.Tweaks.Impl
{
    [TweakMetadata("Ice", $"{UltraTweaker.GUID}.mutator_ice", "Become slippery.", $"{UltraTweaker.GUID}.mutators", 6, "Ice", false, true)]
    public class Ice : Tweak
    {
        private Harmony _harmony = new($"{UltraTweaker.GUID}.mutator_ice");

        public Ice()
        {
            Subsettings = new()
            {
                { "slippyness", new FloatSubsetting(this, new Metadata("Friction", "slippyness", "How grippy you are."),
                    new SliderFloatSubsettingElement("{0}", 2), 0.1f, 0.5f, 0) }
            };
        }

        public override void OnTweakEnabled()
        {
            base.OnTweakEnabled();
            _harmony.PatchAll(typeof(IcePatches));
        }

        public override void OnTweakDisabled()
        {
            base.OnTweakDisabled();
            _harmony.UnpatchSelf();
            NewMovement.Instance.modForcedFrictionMultip = 1;
        }

        public override void OnSubsettingUpdate()
        {
            NewMovement.Instance.modForcedFrictionMultip = GetInstance<Ice>().Subsettings["slippyness"].GetValue<float>();
        }

        public static class IcePatches
        {
            [HarmonyPatch(typeof(NewMovement), nameof(NewMovement.Start))]
            [HarmonyPostfix]
            public static void IcePlayer(NewMovement __instance)
            {
                __instance.modForcedFrictionMultip = GetInstance<Ice>().Subsettings["slippyness"].GetValue<float>();
            }
        }
    }
}

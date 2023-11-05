using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Text;
using UltraTweaker.Subsettings.Impl;
using UltraTweaker.UIElements.Impl;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace UltraTweaker.Tweaks.Impl
{
    [TweakMetadata("Unobtrusive Blood", $"{UltraTweaker.GUID}.unobtrusive_blood", "Make the screen blood more transparent, or gone.", $"{UltraTweaker.GUID}.hud", 3)]
    public class UnobtrusiveBlood : Tweak
    {
        private Harmony _harmony = new($"{UltraTweaker.GUID}.unobtrusive_blood");

        public UnobtrusiveBlood()
        {
            Subsettings = new()
            {
                { "transparency", new IntSubsetting(this, new("Blood Opacity", "transparency", "How transparent the blood is."),
                    new SliderIntSubsettingElement("{0}%"), 44, 100, 0) },
            };
        }

        public override void OnTweakEnabled()
        {
            base.OnTweakEnabled();
            _harmony.PatchAll(typeof(UnobtrusiveBloodPatches));

            OnSubsettingUpdate();
        }

        public override void OnTweakDisabled()
        {
            base.OnTweakDisabled();
            _harmony.UnpatchSelf();
        }

        public static class UnobtrusiveBloodPatches
        {
            [HarmonyPatch(typeof(ScreenBlood), nameof(ScreenBlood.Start)), HarmonyPostfix]
            private static void ChangeOpacity(ScreenBlood __instance)
            {
                __instance.clr.a = GetInstance<UnobtrusiveBlood>().Subsettings["transparency"].GetValue<int>() / 100f;
            }
        }
    }
}

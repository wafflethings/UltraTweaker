using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace UltraTweaker.Tweaks.Impl
{
    [TweakMetadata("Hitstop", $"{UltraTweaker.GUID}.hitstop", "Change hitstop duration, change the parry flash.", $"{UltraTweaker.GUID}.misc", 0)]
    public class Hitstop : Tweak
    {
        private Harmony harmony = new($"{UltraTweaker.GUID}.hitstop");

        public Hitstop()
        {
            Subsettings = new()
            {
                { "hitstop_length", new IntSubsetting(this, new("Hitstop Length", "hitstop_length", "How long hitstop lasts."), 
                    new SliderIntSubsettingElement("{0}%"), 100, 200, 0) },

                { "truestop_length", new IntSubsetting(this, new("Truestop Length", "truestop_length", "How long truestop lasts."), 
                    new SliderIntSubsettingElement("{0}%"), 100, 200, 0) },

                { "slowdown_length", new IntSubsetting(this, new("Slowdown Length", "slowdown_length", "How long slowdown lasts."), 
                    new SliderIntSubsettingElement("{0}%"), 100, 200, 0) },

                { "parry_flash", new BoolSubsetting(this, new("Parry Flash", "parry_flash", "Does the parry flash exist?"), 
                    new BoolSubsettingElement(), true) }
            };
        }

        public override void OnTweakEnabled()
        {
            base.OnTweakEnabled();
            harmony.PatchAll(typeof(HitstopPatches));

            OnSubsettingUpdate();
        }

        public override void OnTweakDisabled()
        {
            base.OnTweakDisabled();
            harmony.UnpatchSelf();

            if (TimeController.Instance != null)
            {
                if (TimeController.Instance.parryFlash != null)
                {
                    TimeController.Instance.parryFlash.GetComponent<Image>().enabled = true;
                }

                if (TimeController.Instance.parryLight != null)
                {
                    TimeController.Instance.parryLight.GetComponent<Light>().enabled = true;
                }
            }
        }

        public override void OnSubsettingUpdate()
        {
            if (TimeController.Instance != null)
            {
                if (TimeController.Instance.parryFlash != null)
                {
                    TimeController.Instance.parryFlash.GetComponent<Image>().enabled = Subsettings["parry_flash"].GetValue<bool>();
                }

                if (TimeController.Instance.parryLight != null)
                {
                    TimeController.Instance.parryLight.GetComponent<Light>().enabled = Subsettings["parry_flash"].GetValue<bool>();
                }
            }
        }

        public override void OnSceneLoad(Scene scene, LoadSceneMode mode)
        {
            if (TimeController.Instance != null)
            {
                if (TimeController.Instance.parryFlash != null)
                {
                    TimeController.Instance.parryFlash.GetComponent<Image>().enabled = Subsettings["parry_flash"].GetValue<bool>();
                }

                if (TimeController.Instance.parryLight != null)
                {
                    TimeController.Instance.parryLight.GetComponent<Light>().enabled = Subsettings["parry_flash"].GetValue<bool>();
                }
            }
        }

        public static class HitstopPatches
        {
            [HarmonyPatch(typeof(TimeController), nameof(TimeController.HitStop)), HarmonyPrefix]
            private static void PatchHitstop(ref float length)
            {
                length *= (GetInstance<Hitstop>().Subsettings["hitstop_length"].GetValue<int>() / 100f);
            }

            [HarmonyPatch(typeof(TimeController), nameof(TimeController.TrueStop)), HarmonyPrefix]
            private static void PatchTruestop(ref float length)
            {
                length *= (GetInstance<Hitstop>().Subsettings["truestop_length"].GetValue<int>() / 100f);
            }

            [HarmonyPatch(typeof(TimeController), nameof(TimeController.SlowDown)), HarmonyPrefix]
            private static void PatchSlowdown(ref float amount)
            {
                amount *= (GetInstance<Hitstop>().Subsettings["slowdown_length"].GetValue<int>() / 100f);
            }
        }
    }
}

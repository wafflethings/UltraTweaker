using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using UnityEngine;

namespace UltraTweaker.Tweaks.Impl
{
    [TweakMetadata("Ultrahot", $"{UltraTweaker.GUID}.mutator_ultrahot", "Time moves when you move.", $"{UltraTweaker.GUID}.mutators", 12, "Ultrahot", false, true)]
    public class Ultrahot : Tweak
    {
        public override void OnTweakEnabled()
        {
            base.OnTweakEnabled();
        }

        public override void OnTweakDisabled()
        {
            base.OnTweakDisabled();
        }

        public void Update()
        {
            if (IsGameplayScene())
            {
                float VelocityFor1 = 20f; // The amount of velocity needed to make timescale 1
                float Min = 0.01f;
                float Max = 1.25f;
                float LerpSpeed = 15f;

                float thing = NewMovement.Instance.rb.velocity.magnitude / VelocityFor1;

                if (thing > Max)
                    thing = Max;

                if (thing < Min)
                    thing = Min;

                Time.timeScale = Mathf.Lerp(Time.timeScale, thing, Time.deltaTime * LerpSpeed);
            }
        }
    }
}

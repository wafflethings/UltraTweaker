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
                float velocityForOne = 20f; // The amount of velocity needed to make timescale 1
                float min = 0.01f;
                float max = 1.25f;
                float lerpSpeed = 15f;

                float targetSpeed = NewMovement.Instance.rb.velocity.magnitude / velocityForOne;
                targetSpeed = Mathf.Clamp(targetSpeed, min, max);

                Time.timeScale = Mathf.Lerp(Time.timeScale, targetSpeed, Time.deltaTime * lerpSpeed);
            }
        }
    }
}

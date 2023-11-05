using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using UltraTweaker.Subsettings.Impl;
using UltraTweaker.UIElements.Impl;
using UnityEngine;

namespace UltraTweaker.Tweaks.Impl
{
    [TweakMetadata("Fresh", $"{UltraTweaker.GUID}.mutator_fresh", "Hurts you if you're not stylish.", $"{UltraTweaker.GUID}.mutators", 4, "Fresh", true, true)]
    public class Fresh : Tweak
    {
        private float _toRemove = 0;

        public Fresh()
        {
            Subsettings = new()
            {
                { "fresh", new IntSubsetting(this, new Metadata("FRESH", "fresh", "HP drained per second on Fresh."),
                    new SliderIntSubsettingElement("{0}"), 0, 100, 0) },

                { "used", new IntSubsetting(this, new Metadata("USED", "used", "HP drained per second on Used."),
                    new SliderIntSubsettingElement("{0}"), 0, 100, 4) },

                { "stale", new IntSubsetting(this, new Metadata("STALE", "stale", "HP drained per second on Stale."),
                    new SliderIntSubsettingElement("{0}"), 0, 100, 8) },

                { "dull", new IntSubsetting(this, new Metadata("DULL", "dull", "HP drained per second on Dull."),
                    new SliderIntSubsettingElement("{0}"), 0, 100, 12) }
            };
        }

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
            Dictionary<StyleFreshnessState, float> dict = new Dictionary<StyleFreshnessState, float>()
            {
                { StyleFreshnessState.Fresh, Subsettings["fresh"].GetValue<int>() },
                { StyleFreshnessState.Used, Subsettings["used"].GetValue<int>() },
                { StyleFreshnessState.Stale, Subsettings["stale"].GetValue<int>() },
                { StyleFreshnessState.Dull, Subsettings["dull"].GetValue<int>() }
            };

            if (NewMovement.Instance != null && StatsManager.Instance.timer && GunControl.Instance.activated)
            {
                _toRemove += dict[StyleHUD.Instance.GetFreshnessState(GunControl.Instance.currentWeapon)] * Time.deltaTime;

                if ((int)_toRemove >= 1)
                {
                    NewMovement.Instance.hp -= (int)_toRemove;
                    _toRemove -= (int)_toRemove;
                }

                if (NewMovement.Instance.hp <= 0 && !NewMovement.Instance.dead)
                {
                    NewMovement.Instance.GetHurt(int.MaxValue, false, 1, true, true);
                }
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Text;
using UltraTweaker.Subsettings.Impl;
using UnityEngine;
using UltraTweaker.UIElements.Impl;

namespace UltraTweaker.Tweaks.Impl
{
    [TweakMetadata("Fuel Leak", $"{UltraTweaker.GUID}.mutator_fuel_leak", "Lose health over time.", $"{UltraTweaker.GUID}.mutators", 5, "Leak", true, true)]
    public class FuelLeak : Tweak
    {
        private float _toRemove = 0;

        public FuelLeak()
        {
            Subsettings = new()
            {
                { "drain", new IntSubsetting(this, new Metadata("Damage Per Second", "drain", "HP drained per second."),
                    new SliderIntSubsettingElement("{0}"), 0, 25, 1) },
            };
        }

        public void Update()
        {
            if (NewMovement.Instance != null && StatsManager.Instance.timer && GunControl.Instance.activated)
            {
                _toRemove += Time.deltaTime * Subsettings["drain"].GetValue<int>();

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

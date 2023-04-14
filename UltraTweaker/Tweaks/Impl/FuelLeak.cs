using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace UltraTweaker.Tweaks.Impl
{
    [TweakMetadata("Fuel Leak", $"{UltraTweaker.GUID}.mutator_fuel_leak", "Lose health over time.", $"{UltraTweaker.GUID}.mutators", 5, "Leak", true, true)]
    public class FuelLeak : Tweak
    {
        private float ToRemove = 0;

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
                ToRemove += Time.deltaTime * Subsettings["drain"].GetValue<int>();

                if ((int)ToRemove >= 1)
                {
                    NewMovement.Instance.hp -= (int)ToRemove;
                    ToRemove -= (int)ToRemove;
                }

                if (NewMovement.Instance.hp <= 0 && !NewMovement.Instance.dead)
                {
                    NewMovement.Instance.GetHurt(int.MaxValue, false, 1, true, true);
                }
            }
        }
    }
}

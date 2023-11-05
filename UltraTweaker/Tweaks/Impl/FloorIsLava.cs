using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Text;
using UltraTweaker.Subsettings.Impl;
using UnityEngine;
using UltraTweaker.UIElements.Impl;

namespace UltraTweaker.Tweaks.Impl
{
    [TweakMetadata("Floor Is Lava", $"{UltraTweaker.GUID}.mutator_floor_is_lava", "Take damage when on the floor.", $"{UltraTweaker.GUID}.mutators", 2, "Lava", true, true)]
    public class FloorIsLava : Tweak
    {
        private float _toRemove = 0;
        private float _onFloorFor;

        public FloorIsLava()
        {
            Subsettings = new()
            {
                 { "damage_after", new FloatSubsetting(this, new Metadata("Damage After", "damage_after", "Time before damage starts."),
                    new SliderFloatSubsettingElement("{0}s"), 0.1f, 5, 0) },

                { "damage_per_second", new IntSubsetting(this, new Metadata("Damage Per Second", "damage_per_second", "Damage per second."),
                    new SliderIntSubsettingElement("{0}"), 25, 100, 0) }
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

        private void Update()
        {
            if (NewMovement.Instance != null)
            {
                if (NewMovement.Instance.gc.touchingGround)
                {
                    _onFloorFor += Time.deltaTime;
                }
                else
                {
                    _onFloorFor = 0;
                }

                if (StatsManager.Instance.timer && _onFloorFor > Subsettings["damage_after"].GetValue<float>())
                {
                    _toRemove += Time.deltaTime * Subsettings["damage_per_second"].GetValue<int>();

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
}

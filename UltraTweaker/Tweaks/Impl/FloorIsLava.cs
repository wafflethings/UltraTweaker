using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace UltraTweaker.Tweaks.Impl
{
    [TweakMetadata("Floor Is Lava", $"{UltraTweaker.GUID}.mutator_floor_is_lava", "Take damage when on the floor.", $"{UltraTweaker.GUID}.mutators", 1, "Lava", true, true)]
    public class FloorIsLava : Tweak
    {
        private float ToRemove = 0;
        private float OnFloorFor;

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
                    OnFloorFor += Time.deltaTime;
                }
                else
                {
                    OnFloorFor = 0;
                }

                if (StatsManager.Instance.timer && OnFloorFor > Subsettings["damage_after"].GetValue<float>())
                {
                    ToRemove += Time.deltaTime * Subsettings["damage_per_second"].GetValue<int>();

                    if ((int)ToRemove >= 1)
                    {
                        NewMovement.Instance.hp -= (int)ToRemove;
                        ToRemove -= (int)ToRemove;
                    }

                    if (NewMovement.Instance.hp <= 0)
                    {
                        NewMovement.Instance.GetHurt(int.MaxValue, false, 1, true, true);
                    }
                }
            }
        }
    }
}

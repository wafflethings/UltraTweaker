using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace UltraTweaker.Tweaks.Impl
{
    [TweakMetadata("Tankify", $"{UltraTweaker.GUID}.mutator_tankify", "Change enemy health.", $"{UltraTweaker.GUID}.mutators", 11, "Tankify", false, true)]
    public class Tankify : Tweak
    {
        private Harmony harmony = new($"{UltraTweaker.GUID}.mutator_tankify");

        public Tankify()
        {
            Subsettings = new()
            {
                { "multiplier", new FloatSubsetting(this, new Metadata("Health Multplier", "multiplier", "Enemy health multiplier."),
                    new SliderFloatSubsettingElement("{0}"), 2, 10, 0) }
            };
        }

        public override void OnTweakEnabled()
        {
            base.OnTweakEnabled();
            harmony.PatchAll(typeof(TankifyPatches));
        }

        public override void OnTweakDisabled()
        {
            base.OnTweakDisabled();
            harmony.UnpatchSelf();
        }

        public static class TankifyPatches
        {
            [HarmonyPatch(typeof(EnemyIdentifier), nameof(EnemyIdentifier.Start)), HarmonyPostfix]
            public static void IncreaseHealth(EnemyIdentifier __instance)
            {
                float mult = GetInstance<Tankify>().Subsettings["multiplier"].GetValue<float>();

                if (!__instance.healthBuff)
                {
                    __instance.gameObject.AddComponent<DisableDoubleRender>();
                    __instance.HealthBuff(mult);
                }
                else
                {
                    __instance.healthBuffModifier *= mult;
                }
            }
        }
    }
}

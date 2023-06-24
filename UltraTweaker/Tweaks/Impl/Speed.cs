using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using UnityEngine;
using UnityEngine.AI;

namespace UltraTweaker.Tweaks.Impl
{
    [TweakMetadata("Speed", $"{UltraTweaker.GUID}.mutator_speed", "Speed up yourself, and enemies", $"{UltraTweaker.GUID}.mutators", 9, "Speed", false, true)]
    public class Speed : Tweak
    {
        private Harmony harmony = new($"{UltraTweaker.GUID}.mutator_speed");
        private static float StartSpeed = 0;

        public Speed()
        {
            Subsettings = new()
            {
                { "player_speed_mult", new FloatSubsetting(this, new Metadata("Player Speed", "player_speed_mult", "Speed multiplier for the player."),
                    new SliderFloatSubsettingElement("{0}x"), 2, 10, 0) },

                { "enemy_speed_mult", new FloatSubsetting(this, new Metadata("Enemy Speed", "enemy_speed_mult", "Speed multiplier for the enemies."),
                    new SliderFloatSubsettingElement("{0}x"), 2, 10, 0) }
            };
        }

        public override void OnSubsettingUpdate()
        {
            NewMovement.Instance.walkSpeed = StartSpeed * Subsettings["player_speed_mult"].GetValue<float>();
        }

        public override void OnTweakEnabled()
        {
            base.OnTweakEnabled();
            if (NewMovement.Instance != null)
            {
                StartSpeed = NewMovement.Instance.walkSpeed;
                NewMovement.Instance.walkSpeed = StartSpeed * GetInstance<Speed>().Subsettings["player_speed_mult"].GetValue<float>();
            }
            harmony.PatchAll(typeof(SpeedPatches));
        }

        public override void OnTweakDisabled()
        {
            base.OnTweakDisabled();
            if (NewMovement.Instance != null)
            {
                NewMovement.Instance.walkSpeed = StartSpeed;
            }
            harmony.UnpatchSelf();
        }

        public static class SpeedPatches
        {
            [HarmonyPatch(typeof(NewMovement), nameof(NewMovement.Start)), HarmonyPostfix]
            public static void SpeedPlayer(NewMovement __instance)
            {
                StartSpeed = __instance.walkSpeed;
                __instance.walkSpeed = StartSpeed * GetInstance<Speed>().Subsettings["player_speed_mult"].GetValue<float>();
            }

            [HarmonyPatch(typeof(EnemyIdentifier), nameof(EnemyIdentifier.Start)), HarmonyPostfix]
            public static void SpeedEnemy(EnemyIdentifier __instance)
            {
                float mult = GetInstance<Speed>().Subsettings["enemy_speed_mult"].GetValue<float>();

                if (!__instance.speedBuff)
                {
                    __instance.gameObject.AddComponent<DisableDoubleRender>();
                    __instance.SpeedBuff(mult);
                }
                else
                {
                    __instance.speedBuffModifier *= mult;
                }
            }
        }
    }
}

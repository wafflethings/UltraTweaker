using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace UltraTweaker.Tweaks.Impl
{
    [TweakMetadata("Explode On Death", $"{UltraTweaker.GUID}.explode_self", "Explode and destroy everything when you die.", $"{UltraTweaker.GUID}.fun", 1)]
    public class ExplodeOnDeath : Tweak
    {
        private Harmony harmony = new($"{UltraTweaker.GUID}.explode_self");

        public override void OnTweakEnabled()
        {
            base.OnTweakEnabled();
            harmony.PatchAll(typeof(ExplosionPatches));
        }

        public override void OnTweakDisabled()
        {
            base.OnTweakDisabled();
            harmony.UnpatchSelf();
        }

        public static class ExplosionPatches
        {
            [HarmonyPatch(typeof(NewMovement), nameof(NewMovement.GetHurt)), HarmonyPostfix]
            private static void ExplodeOnDeath()
            {
                if (NewMovement.Instance.dead)
                {
                    GameObject originalExplosion = GunSetter.Instance.shotgunPump[0].GetComponent<Shotgun>().explosion;
                    GameObject deathExplosion = Instantiate(originalExplosion, NewMovement.Instance.transform.position, NewMovement.Instance.transform.rotation);

                    foreach (Explosion explosion in deathExplosion.GetComponentsInChildren<Explosion>())
                    {
                        explosion.enemyDamageMultiplier = 15f;
                        explosion.maxSize *= 15;
                        explosion.damage = 0;
                    }
                }
            }
        }
    }
}

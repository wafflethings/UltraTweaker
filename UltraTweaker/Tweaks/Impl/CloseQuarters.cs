using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace UltraTweaker.Tweaks.Impl
{
    [TweakMetadata("Close Quarters", $"{UltraTweaker.GUID}.mutator_close_quarters", "Blesses enemies when far.", $"{UltraTweaker.GUID}.mutators", 0, "Cross", true, true)]
    public class CloseQuarters : Tweak
    {
        private Harmony harmony = new($"{UltraTweaker.GUID}.mutator_close_quarters");

        public CloseQuarters()
        {
            Subsettings = new()
            {
                { "enemy_distance", new IntSubsetting(this, new Metadata("Distance", "enemy_distance", "Distance to bless at."),
                    new SliderIntSubsettingElement("{0}m"), 15, 30, 5) }
            };
        }

        public override void OnTweakEnabled()
        {
            base.OnTweakEnabled();
            harmony.PatchAll(typeof(DistancePatches));
        }

        public override void OnTweakDisabled()
        {
            base.OnTweakDisabled();
            harmony.UnpatchSelf();
        }

        public static class DistancePatches
        {
            [HarmonyPatch(typeof(EnemyIdentifier), nameof(EnemyIdentifier.Awake))]
            [HarmonyPostfix]
            public static void AddComp(EnemyIdentifier __instance)
            {
                __instance.gameObject.AddComponent<BlessWhenFar>();
            }
        }

        public class BlessWhenFar : MonoBehaviour
        {
            private EnemyIdentifier eid;

            public void Start()
            {
                eid = GetComponent<EnemyIdentifier>();
            }

            public void Update()
            {
                if (Vector3.Distance(transform.position, NewMovement.Instance.transform.position)
                    > GetInstance<CloseQuarters>().Subsettings["enemy_distance"].GetValue<int>())
                {
                    if (!eid.blessed)
                    {
                        eid.Bless();
                    }
                }
                else
                {
                    if (eid.blessed)
                    {
                        eid.Unbless();
                    }
                }
            }
        }
    }
}

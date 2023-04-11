using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using UnityEngine;

namespace UltraTweaker.Tweaks.Impl
{
    [TweakMetadata("Submerged", $"{UltraTweaker.GUID}.mutator_submerged", "Everything is underwater.", $"{UltraTweaker.GUID}.mutators", 9, "Submerged", false, true)]
    public class Submerged : Tweak
    {
        private Harmony harmony = new($"{UltraTweaker.GUID}.mutator_submerged");
        private static GameObject water; // bo'ul o' wo'uh

        public override void OnTweakEnabled()
        {
            base.OnTweakEnabled();
            harmony.PatchAll(typeof(SubmergedPatches));

            MakeWater();
        }

        public override void OnTweakDisabled()
        {
            base.OnTweakDisabled();
            harmony.UnpatchSelf();

            if (water != null)
            {
                Destroy(water);
            }
        }

        public static void MakeWater()
        {
            if (IsGameplayScene())
            {
                water = GameObject.CreatePrimitive(PrimitiveType.Cube);
                water.name = "UT WATER!!!";
                water.AddComponent<Rigidbody>();
                water.GetComponent<Rigidbody>().isKinematic = true;
                water.GetComponent<Collider>().isTrigger = true;
                water.AddComponent<Water>();
                water.GetComponent<Water>().bubblesParticle = new GameObject();
                water.GetComponent<Water>().clr = new Color(0, 0.5f, 1);
                water.GetComponent<MeshRenderer>().enabled = false;
                water.transform.localScale = Vector3.one * 10000000000; // I think this should be big enough
            }
        }

        public static class SubmergedPatches
        {
            [HarmonyPatch(typeof(NewMovement), nameof(NewMovement.Start)), HarmonyPostfix]
            public static void SpawnWater()
            {
                MakeWater();
            }

            [HarmonyPatch(typeof(Water), nameof(Water.Start)), HarmonyPrefix]
            public static void DisableOtherWaters(Water __instance)
            {
                if (__instance.gameObject.name != "UT WATER!!!")
                {
                    __instance.enabled = false;
                }
            }

            [HarmonyPatch(typeof(BloodsplatterManager), nameof(BloodsplatterManager.GetGore)), HarmonyPrefix]
            public static void MakeUnderwater(ref bool isUnderwater)
            {
                isUnderwater = true;
            }
        }
    }
}

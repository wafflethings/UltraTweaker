using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using UnityEngine;

namespace UltraTweaker.Tweaks.Impl
{
    [TweakMetadata("Submerged", $"{UltraTweaker.GUID}.mutator_submerged", "Everything is underwater.", $"{UltraTweaker.GUID}.mutators", 10, "Submerged", false, true)]
    public class Submerged : Tweak
    {
        private Harmony _harmony = new($"{UltraTweaker.GUID}.mutator_submerged");
        private static GameObject _water; // bo'ul o' wo'uh

        public override void OnTweakEnabled()
        {
            base.OnTweakEnabled();
            _harmony.PatchAll(typeof(SubmergedPatches));

            MakeWater();
        }

        public override void OnTweakDisabled()
        {
            base.OnTweakDisabled();
            _harmony.UnpatchSelf();

            if (_water != null)
            {
                Destroy(_water);
            }
        }

        public static void MakeWater()
        {
            if (IsGameplayScene())
            {
                _water = GameObject.CreatePrimitive(PrimitiveType.Cube);
                _water.name = "UT WATER!!!";
                _water.AddComponent<Rigidbody>();
                _water.GetComponent<Rigidbody>().isKinematic = true;
                _water.GetComponent<Collider>().isTrigger = true;
                _water.AddComponent<Water>();
                _water.GetComponent<Water>().bubblesParticle = new GameObject();
                _water.GetComponent<Water>().clr = new Color(0, 0.5f, 1);
                _water.GetComponent<MeshRenderer>().enabled = false;
                _water.transform.localScale = Vector3.one * 10000000000; // I think this should be big enough
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

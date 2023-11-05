using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UltraTweaker.Handlers;

namespace UltraTweaker.Tweaks.Impl
{
    [TweakMetadata("Skull Florpification", $"{UltraTweaker.GUID}.florpify_:3", "Replace skulls with Florp.", $"{UltraTweaker.GUID}.fun", 0)]
    public class Florpify : Tweak
    {
        private Harmony _harmony = new($"{UltraTweaker.GUID}.florpify_:3");
        private static Dictionary<ItemType, GameObject> _florps;

        public override void OnTweakEnabled()
        {
            base.OnTweakEnabled();

            if (_florps == null)
            {
                _florps = new()
                {
                    { ItemType.SkullBlue, AssetHandler.Bundle.LoadAsset<GameObject>("Blue Florp") },
                    { ItemType.SkullRed, AssetHandler.Bundle.LoadAsset<GameObject>("Red Florp") }
                };
            }

            _harmony.PatchAll(typeof(FlorpyPatches));
        }

        public override void OnTweakDisabled()
        {
            base.OnTweakDisabled();
            _harmony.UnpatchSelf();

            // i cba undoing this one lmao
        }

        public static class FlorpyPatches
        {
            [HarmonyPatch(typeof(Skull), nameof(Skull.Start)), HarmonyPostfix]
            private static void CreateFlorpy(Skull __instance)
            {
                Renderer renderer = __instance.gameObject.GetComponent<Renderer>();
                if (renderer != null)
                {
                    renderer.enabled = false;
                    GameObject newFlorp = Instantiate(_florps[__instance.GetComponent<ItemIdentifier>().itemType], renderer.transform);
                    newFlorp.GetComponentInChildren<Renderer>().material.shader = renderer.material.shader;
                }
            }
        }
    }
}

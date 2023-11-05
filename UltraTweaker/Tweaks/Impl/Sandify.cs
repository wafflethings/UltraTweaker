using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using UnityEngine;

namespace UltraTweaker.Tweaks.Impl
{
    [TweakMetadata("Sandify", $"{UltraTweaker.GUID}.mutator_sandify", "Cover enemies in sand.", $"{UltraTweaker.GUID}.mutators", 8, "Sandify", true, true)]
    public class Sandify : Tweak
    {
        private Harmony _harmony = new($"{UltraTweaker.GUID}.mutator_sandify");

        public override void OnTweakEnabled()
        {
            base.OnTweakEnabled();
            _harmony.PatchAll(typeof(SandifyPatches));
        }

        public override void OnTweakDisabled()
        {
            base.OnTweakDisabled();
            _harmony.UnpatchSelf();
        }

        public static class SandifyPatches
        {
            [HarmonyPatch(typeof(EnemyIdentifier), nameof(EnemyIdentifier.Awake)), HarmonyPostfix]
            public static void MakeSandy(EnemyIdentifier __instance)
            {
                __instance.sandified = true;
            }
        }
    }
}

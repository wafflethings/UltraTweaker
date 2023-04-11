using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UltraTweaker;
using UltraTweaker.Tweaks;
using UnityEngine;
using UnityEngine.UI;

namespace UltraTweaker.Tweaks.Impl
{
    [TweakMetadata("Force Gun Panel", $"{UltraTweaker.GUID}.force_gun", "Makes the gun panel always appear. So that you can use the other Rail charge indicator.", $"{UltraTweaker.GUID}.hud", 3)]
    public class ForceGunPanel : Tweak
    {
        private Harmony harmony = new($"{UltraTweaker.GUID}.force_gun");

        public override void OnTweakEnabled()
        {
            base.OnTweakEnabled();
            harmony.PatchAll(typeof(ForceGunPatches));

            if (IsGameplayScene())
            {
                GunControl.Instance.gunPanel[0].SetActive(true);
            }
        }

        public override void OnTweakDisabled()
        {
            base.OnTweakDisabled();
            harmony.UnpatchSelf();
            GunControl.Instance.gunPanel[0].SetActive(false);
        }

        public static class ForceGunPatches
        {
            [HarmonyPatch(typeof(GunControl), nameof(GunControl.Start)), HarmonyPostfix]
            private static void PatchGunPanel(GunControl __instance)
            {
                if (IsGameplayScene())
                {
                    __instance.gunPanel[0].SetActive(true);
                }
            }

            [HarmonyPatch(typeof(GunSetter), nameof(GunSetter.ResetWeapons)), HarmonyPostfix]
            private static void PatchGunPanelSetter(GunSetter __instance)
            {
                if (IsGameplayScene())
                {
                    GunControl.Instance.gunPanel[0].SetActive(true);
                }
            }
        }
    }
}

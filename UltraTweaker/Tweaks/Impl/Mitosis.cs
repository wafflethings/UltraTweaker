using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace UltraTweaker.Tweaks.Impl
{
    [TweakMetadata("Mitosis", $"{UltraTweaker.GUID}.mutator_mitosis", "Duplicates enemies.", $"{UltraTweaker.GUID}.mutators", 7, "Mitosis", false, true)]
    public class Mitosis : Tweak
    {
        private Harmony harmony = new($"{UltraTweaker.GUID}.mutator_mitosis");
        public static List<ActivateNextWave> alreadyMultiplied = new();

        public Mitosis()
        {
            Subsettings = new()
            {
                { "enemy_amount", new IntSubsetting(this, new Metadata("Amount", "enemy_amount", "The amount of enemies to clone"),
                    new SliderIntSubsettingElement(), 2, 10, 2) }
            };
        }

        public override void OnTweakEnabled()
        {
            base.OnTweakEnabled();
            alreadyMultiplied.Clear();
            harmony.PatchAll(typeof(MitosisPatches));
        }

        public override void OnTweakDisabled()
        {
            base.OnTweakDisabled();
            harmony.UnpatchSelf();
        }

        public override void OnSceneLoad(Scene scene, LoadSceneMode mode)
        {
            alreadyMultiplied.Clear();
        }

        public static class MitosisPatches
        {
            [HarmonyPatch(typeof(EnemyIdentifier), nameof(EnemyIdentifier.Start)), HarmonyPrefix]
            public static void HmmTodayIWillUndergoMitosis(EnemyIdentifier __instance)
            {
                if (!__instance.gameObject.name.Contains("(MITOSISED)"))
                {
                    for (int i = 0; i < GetInstance<Mitosis>().Subsettings["enemy_amount"].GetValue<int>() - 1; i++)
                    {
                        GameObject obj = Instantiate(__instance.gameObject, __instance.transform.parent);

                        obj.name = __instance.gameObject.name + "(MITOSISED)";
                        obj.transform.position = __instance.transform.position;
                        obj.GetComponent<EnemyIdentifier>().blessed = false;
                    }
                }
            }

            [HarmonyPatch(typeof(ActivateNextWave), nameof(ActivateNextWave.Awake)), HarmonyPostfix]
            public static void IncreaseAnw(ActivateNextWave __instance)
            {               
                GoreZone goreZone = __instance.gameObject.GetComponentInParent<GoreZone>();
                if (goreZone == null) //No gorezone? ,':^(
                {
                    Debug.Log($"No GoreZone found in hierarchy of {__instance.gameObject.name}");
                    return;    
                }
                    
                Debug.Log($"{__instance.enemyCount} | {((goreZone.transform.parent == null) ? "orphan" : goreZone.transform.parent.name)} / {goreZone.transform.name} / {__instance.gameObject.name}");
                
                if (!goreZone.gameObject.name.Contains("(Clone)"))
                    return;
                    
                __instance.enemyCount *= GetInstance<Mitosis>().Subsettings["enemy_amount"].GetValue<int>();

                foreach (DeathMarker deathMarker in __instance.gameObject.GetComponentsInChildren<DeathMarker>(true))
                {
                    __instance.enemyCount -= 1;
                }

                Debug.Log($"{__instance.enemyCount} | Whar?");        
            }
        }
    }
}

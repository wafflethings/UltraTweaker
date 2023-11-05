using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Text;
using UltraTweaker.Subsettings.Impl;
using UnityEngine;
using UnityEngine.SceneManagement;
using UltraTweaker.UIElements.Impl;

namespace UltraTweaker.Tweaks.Impl
{
    [TweakMetadata("Viewmodel Transform", $"{UltraTweaker.GUID}.viewmodel_transform", "Resize, change the FOV of, and otherwise tweak the viewmodel.", $"{UltraTweaker.GUID}.misc", 1)]
    public class ViewmodelTransform : Tweak
    {
        private Harmony _harmony = new($"{UltraTweaker.GUID}.viewmodel_transform");
        private static Dictionary<GameObject, Vector3> _originalScale = new();

        public ViewmodelTransform()
        {
            Subsettings = new()
            {
                { "viewmodel_fov", new IntSubsetting(this, new("FOV", "viewmodel_fov", "What is the FOV of the viewmodel?"),
                    new SliderIntSubsettingElement("{0}"), 90, 150, 50) },

                { "viewmodel_size_multiplier", new IntSubsetting(this, new("Size", "viewmodel_size_multiplier", "How big is the viewmodel?"),
                    new SliderIntSubsettingElement("{0}%"), 100, 125, 0) },

                { "viewmodel_bob", new BoolSubsetting(this, new("Bobbing", "viewmodel_bob", "Does the viewmodel bob when you walk?"),
                    new BoolSubsettingElement(), true)  },

                { "viewmodel_tilt", new BoolSubsetting(this, new("Aim-assist Tilt", "viewmodel_tilt", "Does the viewmodel tilt with aim-assist?"),
                    new BoolSubsettingElement(), true)  }

            };
        }

        public override void OnTweakEnabled()
        {
            base.OnTweakEnabled();
            _harmony.PatchAll(typeof(ViewmodelPatches));

            if (GunControl.Instance != null && FistControl.Instance != null)
            {
                UpdateBobAndTilt();
            } 
        }

        public override void OnTweakDisabled()
        {
            base.OnTweakDisabled();
            _harmony.UnpatchSelf();

            if (GunControl.Instance != null && FistControl.Instance != null)
            {
                NewMovement.Instance.gameObject.ChildByName("Main Camera").ChildByName("HUD Camera").GetComponent<Camera>().fieldOfView = 90;

                FistControl.Instance.transform.localScale = Vector3.one;

                GunControl.Instance.GetComponent<WalkingBob>().enabled = true;
                GunControl.Instance.GetComponent<RotateToFaceFrustumTarget>().enabled = true;
            }

            foreach (GameObject go in _originalScale.Keys)
            {
                go.transform.localScale = _originalScale[go];
            }

            _originalScale.Clear();
        }

        public override void OnSceneLoad(Scene scene, LoadSceneMode mode)
        {
            _originalScale.Clear();
        }

        public void LateUpdate()
        {
            if (NewMovement.Instance != null)
            {
                NewMovement.Instance.gameObject.ChildByName("Main Camera").ChildByName("HUD Camera").GetComponent<Camera>().fieldOfView
                    = Subsettings["viewmodel_fov"].GetValue<int>();
            }
        }

        public override void OnSubsettingUpdate()
        {
            UpdateBobAndTilt();

            if (GunControl.Instance != null && FistControl.Instance != null)
            {
                if (GunControl.Instance.currentWeapon != null && GunControl.Instance.currentWeapon.GetComponent<WeaponPos>() != null)
                {
                    GunControl.Instance.currentWeapon.GetComponent<WeaponPos>().CheckPosition();
                }
                UpdateBobAndTilt();

                FistControl.Instance.transform.localScale = Vector3.one;
            }
        }

        public static void UpdateBobAndTilt()
        {
            if (GunControl.Instance != null)
            {
                GunControl.Instance.GetComponent<WalkingBob>().enabled = GetInstance<ViewmodelTransform>().Subsettings["viewmodel_bob"].GetValue<bool>();
                GunControl.Instance.GetComponent<RotateToFaceFrustumTarget>().enabled = GetInstance<ViewmodelTransform>().Subsettings["viewmodel_tilt"].GetValue<bool>();
            }
        }

        public static class ViewmodelPatches
        {
            [HarmonyPatch(typeof(FistControl), nameof(FistControl.Update)), HarmonyPostfix]
            private static void FistSize(FistControl __instance)
            {
                if (__instance.transform.localScale == Vector3.one)
                {
                    float size = GetInstance<ViewmodelTransform>().Subsettings["viewmodel_size_multiplier"].GetValue<int>();

                    // this breaks parries help idk why 

                    //__instance.transform.localScale *=  size / 100f;
                    //if (size != 0)
                    //{
                    //    __instance.gameObject.ChildByName("Projectile Parry Zone").transform.localScale /= size / 100f;
                    //}
                }
            }

            [HarmonyPatch(typeof(WeaponPos), nameof(WeaponPos.CheckPosition))]
            [HarmonyPostfix]
            static void PatchWeaponScale_Check(WeaponPos __instance)
            {
                if (__instance.gameObject.name.Contains("Revolver"))
                {
                    if (!_originalScale.ContainsKey(__instance.gameObject))
                    {
                        _originalScale.Add(__instance.gameObject, __instance.gameObject.transform.localScale);
                    }
                    __instance.gameObject.transform.localScale = _originalScale[__instance.gameObject] * GetInstance<ViewmodelTransform>().Subsettings["viewmodel_size_multiplier"].GetValue<int>() / 100;
                } else
                {
                    foreach (GameObject child in __instance.gameObject.ChildrenList())
                    {
                        if (!_originalScale.ContainsKey(child))
                        {
                            _originalScale.Add(child, child.transform.localScale);
                        }

                        child.transform.localScale = _originalScale[child] * GetInstance<ViewmodelTransform>().Subsettings["viewmodel_size_multiplier"].GetValue<int>() / 100;
                    }
                }
            }

            [HarmonyPatch(typeof(WeaponPos), nameof(WeaponPos.Start))]
            [HarmonyPostfix]
            static void PatchWeaponScale_Start(WeaponPos __instance)
            {
                if (!__instance.gameObject.name.Contains("Revolver"))
                {
                    
                }
            }


            [HarmonyPatch(typeof(GunControl), nameof(GunControl.Start)), HarmonyPostfix]
            private static void BobAndTilt()
            {
                UpdateBobAndTilt();
            }
        }
    }
}

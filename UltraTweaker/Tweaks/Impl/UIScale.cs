using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UltraTweaker.Subsettings.Impl;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UltraTweaker.UIElements.Impl;

namespace UltraTweaker.Tweaks.Impl
{
    [TweakMetadata("UI Scale", $"{UltraTweaker.GUID}.ui_scale", "Change the size of your HUD and UI.", $"{UltraTweaker.GUID}.hud", 0)]
    public class UIScale : Tweak
    {
        private Harmony _harmony = new($"{UltraTweaker.GUID}.ui_scale");

        public static bool DoesntKnowOriginal = true;
        public static Vector3 OriginalInfoScale;
        public static Vector3 OriginalStyleScale;
        public static Vector3 OriginalResultsScale;

        private static GameObject _info;
        private static GameObject _style;
        private static GameObject _results;

        public UIScale()
        {
            Subsettings = new()
            {
                { "info_hud_scale", new IntSubsetting(this, new("Info HUD Scale", "info_hud_scale", "The size of the info, e.g. health and stamina."), 
                    new SliderIntSubsettingElement("{0}%"), 100, 110, 0) },

                { "style_hud_scale", new IntSubsetting(this, new("Style HUD Scale", "style_hud_scale", "The size of the style panel."), 
                    new SliderIntSubsettingElement("{0}%"), 100, 110, 0) },

                { "finalrank_hud_scale", new IntSubsetting(this, new("End HUD Scale", "finalrank_hud_scale", "The size of the panel that shows your final rank."), 
                    new SliderIntSubsettingElement("{0}%"), 100, 110, 0) },

                { "bossbar_hud_scale", new IntSubsetting(this, new("Boss Bar Scale", "bossbar_hud_scale", "The size of the boss bar."), 
                    new SliderIntSubsettingElement("{0}%"), 100, 100, 0) },

                { "canvas_scale", new IntSubsetting(this, new("Canvas Scale", "canvas_scale", "The size of the info, e.g. health and stamina."), 
                    new SliderIntSubsettingElement("{0}%"), 100, 100, 25) }
            };
        }

        public override void OnTweakEnabled()
        {
            base.OnTweakEnabled();
            DoesntKnowOriginal = true;
            _harmony.PatchAll(typeof(UIScalePatches));
            UpdateHUD();
            UpdateCanvas();
        }

        public override void OnTweakDisabled()
        {
            base.OnTweakDisabled();
            UpdateHUD(true);
            UpdateCanvas(true);
            _harmony.UnpatchSelf();
        }
        public override void OnSubsettingUpdate()
        {
            UpdateHUD();
            UpdateCanvas();
        }

        public static void UpdateHUD(bool toDefault = false)
        {
            if ((_info == null || _style == null || _results == null) && FindObjectsOfType<HudController>() != null)
            {
                foreach (HudController hc in FindObjectsOfType<HudController>())
                {
                    if (hc.gameObject.name == "HUD")
                    {
                        _info = hc.gameObject.ChildByName("GunCanvas");
                        _style = hc.gameObject.ChildByName("StyleCanvas");
                        _results = hc.gameObject.ChildByName("FinishCanvas");

                        if (DoesntKnowOriginal)
                        {
                            OriginalInfoScale = _info.transform.localScale;
                            OriginalStyleScale = _style.transform.localScale;
                            OriginalResultsScale = _results.transform.localScale;
                            DoesntKnowOriginal = false;
                        }
                    }
                }
            }

            if (!(_info == null || _style == null || _results == null))
            {
                float InfoScale = GetInstance<UIScale>().Subsettings["info_hud_scale"].GetValue<int>();
                float StyleScale = GetInstance<UIScale>().Subsettings["style_hud_scale"].GetValue<int>();
                float ResultsScale = GetInstance<UIScale>().Subsettings["finalrank_hud_scale"].GetValue<int>();

                if (!toDefault)
                {
                    _info.transform.localScale = OriginalInfoScale * (InfoScale / 100);
                    _style.transform.localScale = OriginalStyleScale * (StyleScale / 100);
                    _results.transform.localScale = OriginalResultsScale * (ResultsScale / 100);
                }
                else
                {
                    _info.transform.localScale = OriginalInfoScale;
                    _style.transform.localScale = OriginalStyleScale;
                    _results.transform.localScale = OriginalResultsScale;
                }
            }
        }

        public static void UpdateCanvas(bool toDefault = false)
        {
            float CanvasScale = GetInstance<UIScale>().Subsettings["canvas_scale"].GetValue<int>();

            if (CanvasScale != 100)
            {
                GameObject canvas = CanvasController.Instance.gameObject;
                canvas.GetComponent<CanvasScaler>().uiScaleMode = CanvasScaler.ScaleMode.ConstantPixelSize;
                canvas.GetComponent<CanvasScaler>().scaleFactor = 1920 / Screen.width * 1.5f;

                if (!toDefault)
                {
                    canvas.GetComponent<CanvasScaler>().scaleFactor *= CanvasScale / 100;
                }
            }
        }

        public override void OnSceneLoad(Scene scene, LoadSceneMode mode)
        {
            DoesntKnowOriginal = true;
        }

        public static class UIScalePatches
        {
            [HarmonyPatch(typeof(CanvasController), nameof(CanvasController.Awake)), HarmonyPostfix]
            private static void PatchCanvasScale(CanvasController __instance)
            {
                UpdateCanvas();
            }

            [HarmonyPatch(typeof(HudController), nameof(HudController.Start)), HarmonyPostfix]
            private static void PatchHUDScale(HudController __instance)
            {
                if (__instance.gameObject.name == "HUD")
                {
                    _info = __instance.gameObject.ChildByName("GunCanvas");
                    _style = __instance.gameObject.ChildByName("StyleCanvas");
                    _results = __instance.gameObject.ChildByName("FinishCanvas");

                    if (DoesntKnowOriginal)
                    {
                        OriginalInfoScale = _info.transform.localScale;
                        OriginalStyleScale = _style.transform.localScale;
                        OriginalResultsScale = _results.transform.localScale;
                        DoesntKnowOriginal = false;
                    }

                    UpdateHUD();
                }
            }

            [HarmonyPatch(typeof(BossHealthBar), nameof(BossHealthBar.Awake))]
            [HarmonyPostfix]
            static void PatchBossbarScale(BossHealthBar __instance)
            {
                float BarScale = GetInstance<UIScale>().Subsettings["bossbar_hud_scale"].GetValue<int>();
                __instance.bossBar.transform.localScale *= BarScale / 100;
            }
        }
    }
}

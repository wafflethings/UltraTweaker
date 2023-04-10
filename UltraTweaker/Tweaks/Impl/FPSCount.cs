using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UltraTweaker;
using UltraTweaker.Tweaks;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UltraTweaker.Handlers;

namespace UltraTweaker.Tweaks.Impl
{
    [TweakMetadata("FPS Counter", $"{UltraTweaker.GUID}.fps_meter", "Shows a counter with your frames per second.", $"{UltraTweaker.GUID}.hud", 2)]
    public class FPSCount : Tweak
    {
        private GameObject originalCounter;
        private GameObject counter;
        private Text text;

        public override void OnTweakEnabled()
        {
            base.OnTweakEnabled();
            
            if (originalCounter == null)
            {
                originalCounter = AssetHandler.Bundle.LoadAsset<GameObject>("FPS");
            }

            CreateCounter();
        }

        public override void OnTweakDisabled()
        {
            base.OnTweakDisabled();
            Destroy(counter);
        }

        public void CreateCounter()
        {
            if (CanvasController.Instance != null)
            {
                counter = Instantiate(originalCounter, CanvasController.Instance.transform);
                text = counter.ChildByName("Count").GetComponent<Text>();
            }
        }

        public override void OnSceneLoad(Scene scene, LoadSceneMode mode)
        {
            CreateCounter();
        }

        public void Update()
        {
            if (text != null)
            {
                float FPS = 1.00f / Time.unscaledDeltaTime;
                text.text = $"FPS: {(int)FPS}";
            }
        }
    }
}

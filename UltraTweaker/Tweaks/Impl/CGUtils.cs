using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UltraTweaker.Handlers;

namespace UltraTweaker.Tweaks.Impl
{
    [TweakMetadata("CG Stats", $"{UltraTweaker.GUID}.cg_stats", "Shows the tab panel, but for the Cyber Grind.", $"{UltraTweaker.GUID}.cybergrind", 1)]
    public class CGUtils : Tweak
    {
        private GameObject originalPanel;
        private GameObject panel;
        private Text waves;
        private Text enemies;
        private Text time;
        private Text kills;
        private Text style;

        private LevelStatsEnabler lse;

        private float minutes;
        private float seconds;

        private StatsManager sm;

        public override void OnTweakEnabled()
        {
            base.OnTweakEnabled();
            if (SceneHelper.CurrentScene == "Endless")
            {
                Create();
            }
        }

        public override void OnTweakDisabled()
        {
            base.OnTweakDisabled();

            if (panel != null)
            {
                Destroy(panel);
            }
        }

        public void Create()
        {
            sm = StatsManager.Instance;

            lse = CanvasController.Instance.gameObject.GetComponentInChildren<LevelStatsEnabler>(true);

            if (originalPanel == null)
            {
                originalPanel = AssetHandler.Bundle.LoadAsset<GameObject>("Cybergrind Stats.prefab");
            }
            panel = Instantiate(originalPanel, lse.transform);

            waves = panel.ChildByName("Waves Title").ChildByName("Waves").GetComponent<Text>();
            enemies = panel.ChildByName("Enemies Title").ChildByName("Enemies").GetComponent<Text>();
            time = panel.ChildByName("Time Title").ChildByName("Time").GetComponent<Text>();
            kills = panel.ChildByName("Kills Title").ChildByName("Kills").GetComponent<Text>();
            style = panel.ChildByName("Style Title").ChildByName("Style").GetComponent<Text>();

            waves.text = "0";
            enemies.text = "0";
            time.text = "00:00.000";
            kills.text = "0";
            style.text = "0";

            panel.SetActive(false);
        }

        public void Update()
        {
            if (SceneHelper.CurrentScene == "Endless" && lse != null)
            {
                if (!lse.gameObject.activeSelf)
                {
                    lse.gameObject.SetActive(true);
                }

                if (lse.levelStats != panel)
                {
                    lse.levelStats = panel;
                }

                EndlessGrid eg = EndlessGrid.Instance;

                waves.text = eg.currentWave.ToString();
                enemies.text = (eg.tempEnemyAmount - eg.anw.deadEnemies).ToString();
                kills.text = sm.kills.ToString();
                style.text = sm.stylePoints.ToString();

                seconds = sm.seconds;
                minutes = 0f;

                while (seconds >= 60f)
                {
                    seconds -= 60f;
                    minutes += 1f;
                }

                time.text = minutes + ":" + seconds.ToString("00.000");
            }
        }

        public override void OnSceneLoad(Scene scene, LoadSceneMode mode)
        {
            if (SceneHelper.CurrentScene == "Endless")
            {
                Create();
            }
        }
    }
}

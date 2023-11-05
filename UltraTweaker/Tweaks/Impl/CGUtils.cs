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
        private GameObject _originalPanel;
        private GameObject _panel;
        private Text _waves;
        private Text _enemies;
        private Text _time;
        private Text _kills;
        private Text _style;

        private StatsManager _sm;
        private LevelStatsEnabler _lse;

        private float _minutes;
        private float _seconds;

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

            if (_panel != null)
            {
                Destroy(_panel);
            }
        }

        public void Create()
        {
            _lse = CanvasController.Instance.gameObject.GetComponentInChildren<LevelStatsEnabler>(true);
            _sm = StatsManager.Instance;

            if (_originalPanel == null)
            {
                _originalPanel = AssetHandler.Bundle.LoadAsset<GameObject>("Cybergrind Stats.prefab");
            }

            _panel = Instantiate(_originalPanel, _lse.transform);

            _waves = _panel.ChildByName("Waves Title").ChildByName("Waves").GetComponent<Text>();
            _enemies = _panel.ChildByName("Enemies Title").ChildByName("Enemies").GetComponent<Text>();
            _time = _panel.ChildByName("Time Title").ChildByName("Time").GetComponent<Text>();
            _kills = _panel.ChildByName("Kills Title").ChildByName("Kills").GetComponent<Text>();
            _style = _panel.ChildByName("Style Title").ChildByName("Style").GetComponent<Text>();

            _waves.text = "0";
            _enemies.text = "0";
            _time.text = "00:00.000";
            _kills.text = "0";
            _style.text = "0";

            _panel.SetActive(false);
        }

        public void Update()
        {
            if (SceneHelper.CurrentScene == "Endless" && _lse != null)
            {
                if (!_lse.gameObject.activeSelf)
                {
                    _lse.gameObject.SetActive(true);
                }

                if (_lse.levelStats != _panel)
                {
                    _lse.levelStats = _panel;
                }

                EndlessGrid eg = EndlessGrid.Instance;

                _waves.text = eg.currentWave.ToString();
                _enemies.text = (eg.tempEnemyAmount - eg.anw.deadEnemies).ToString();
                _kills.text = _sm.kills.ToString();
                _style.text = _sm.stylePoints.ToString();

                _seconds = _sm.seconds;
                _minutes = 0f;

                while (_seconds >= 60f)
                {
                    _seconds -= 60f;
                    _minutes += 1f;
                }

                _time.text = _minutes + ":" + _seconds.ToString("00.000");
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

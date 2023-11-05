using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UltraTweaker.Handlers;
using UltraTweaker.Subsettings.Impl;
using UltraTweaker.UIElements.Impl;

namespace UltraTweaker.Tweaks.Impl
{
    [TweakMetadata("Stat Panels", $"{UltraTweaker.GUID}.stat_panels", "Various info panels.", $"{UltraTweaker.GUID}.hud", 1)]
    public class StatPanels : Tweak
    {
        private Harmony harmony = new($"{UltraTweaker.GUID}.stat_panels");

        private GameObject _originalPanels;
        private GameObject _currentPanels;

        private GameObject _dps;
        private Text _dpsText;

        private GameObject _speed;
        private Text _speedText;

        private GameObject _info;
        private Text _hpText;
        private Text _staminaText;

        private GameObject _weapons;
        private Image _gunImage;
        private Image _punchImage;
        private Slider _railSlider;

        // All hits that have occured in the last second
        public static List<Hit> HitsSecond = new List<Hit>();

        public class Hit
        {
            public DateTime time;
            public float dmg;

            public Hit (float dmg)
            {
                this.dmg = dmg;
                time = DateTime.Now;
            }
        }

        public StatPanels()
        {
            Subsettings = new()
            {
                { "info", new BoolSubsetting(this, new("Info Panel", "info", "Shows your health and stamina."), 
                    new BoolSubsettingElement(), false)  },

                { "weapons", new BoolSubsetting(this, new("Weapon Panel", "weapons", "Shows your weapon, fist, and rail charge."), 
                    new BoolSubsettingElement(), false)  },

                { "dps", new BoolSubsetting(this, new("DPS Panel", "dps", "Shows your damage per second."), 
                    new BoolSubsettingElement(), false)  },

                { "speed", new BoolSubsetting(this, new("Speed Panel", "speed", "Shows your speed."), 
                    new BoolSubsettingElement(), false)  },

                { "speed_mode", new IntSubsetting(this, new("Speed: Mode", "speed_mode", "Should it show total speed, or speed in each direction?"),
                    new DropdownIntSubsettingElement(new List<string>() { "(x) m/s", "(x, y, z) m/s"} ), 0, 1, 0)  },

                { "size", new IntSubsetting(this, new("Size", "size", "How big the panels are."), 
                    new SliderIntSubsettingElement("{0}%"), 100, 200, 0) }
            };
        }

        public override void OnTweakEnabled()
        {
            base.OnTweakEnabled();
            harmony.PatchAll(typeof(PanelPatches));
            Create();
        }

        public override void OnTweakDisabled()
        {
            base.OnTweakDisabled();
            Destroy(_currentPanels);
            harmony.UnpatchSelf();
        }

        public override void OnSubsettingUpdate()
        {
            if (CanvasController.Instance != null && IsGameplayScene() && _currentPanels != null)
            {
                _dps.SetActive(Subsettings["dps"].GetValue<bool>());
                _speed.SetActive(Subsettings["speed"].GetValue<bool>());
                _info.SetActive(Subsettings["info"].GetValue<bool>());
                _weapons.SetActive(Subsettings["weapons"].GetValue<bool>());

                foreach (GameObject row in _currentPanels.ChildrenList())
                {
                    int activeAmount = 0;
                    foreach (GameObject panel in row.ChildrenList())
                    {
                        if (panel.activeSelf)
                        {
                            activeAmount += 1;
                        }
                    }

                    if (activeAmount == 0)
                    {
                        row.SetActive(false);
                    } else
                    {
                        row.SetActive(true);
                    }
                }

                _currentPanels.transform.SetAsFirstSibling();
                _currentPanels.transform.localScale = Vector3.one * Subsettings["size"].GetValue<int>() / 100f;
            }
        }

        public override void OnSceneLoad(Scene scene, LoadSceneMode mode)
        {
            Create();
        }

        private void Create()
        {
            if (CanvasController.Instance != null && IsGameplayScene())
            {
                if (_originalPanels == null)
                {
                    _originalPanels = AssetHandler.Bundle.LoadAsset<GameObject>("All Panels.prefab");
                }

                _currentPanels = Instantiate(_originalPanels, CanvasController.Instance.transform);

                _dps = _currentPanels.ChildByName("Top Row").ChildByName("DPS");
                _speed = _currentPanels.ChildByName("Top Row").ChildByName("Speed");
                _info = _currentPanels.ChildByName("Bottom Row").ChildByName("Info");
                _weapons = _currentPanels.ChildByName("Bottom Row").ChildByName("Weapons");

                _dpsText = _dps.ChildByName("DPS").GetComponent<Text>();
                _speedText = _speed.ChildByName("SPEED").GetComponent<Text>();
                _hpText = _info.ChildByName("HP").GetComponent<Text>();
                _staminaText = _info.ChildByName("Stamina").GetComponent<Text>();
                _gunImage = _weapons.ChildByName("Gun").GetComponent<Image>();
                _punchImage = _weapons.ChildByName("Fist").GetComponent<Image>();
                _railSlider = _weapons.ChildByName("Slider").GetComponent<Slider>();

                OnSubsettingUpdate();
            }
        }

        private float CalculateDps()
        {
            float Damage = 0;
            // I would use foreach but you can't edit the list in foreaches
            for (int i = 0; i < HitsSecond.Count; i++)
            {
                Hit hit = HitsSecond[i];
                if ((DateTime.Now - hit.time).TotalSeconds > 1)
                {
                    HitsSecond.RemoveAt(i);
                    i--;
                }
                else
                {
                    float ActualDmg = hit.dmg;

                    if (ActualDmg < 1000000 && ActualDmg > 0)
                        Damage += ActualDmg;
                }
            }

            return Damage;
        }

        public void Update()
        {
            if (_currentPanels != null)
            {
                if (_info.activeSelf && NewMovement.Instance != null)
                {
                    string prefix = "";
                    string suffix = "";
                    if (NewMovement.Instance.antiHp != 0)
                    {
                        prefix += "<color=#7C7A7B>";
                        suffix += "</color>";
                    }

                    _hpText.text = $"{NewMovement.Instance.hp}{prefix} / {100 - Math.Round(NewMovement.Instance.antiHp, 0)}{suffix}";
                    _staminaText.text = $"{(NewMovement.Instance.boostCharge / 100).ToString("0.00")} / 3.00";
                }

                if (_weapons.activeSelf && NewMovement.Instance != null && WeaponHUD.Instance != null && WeaponCharges.Instance != null && FistControl.Instance != null)
                {
                    _gunImage.sprite = WeaponHUD.Instance.img.sprite;
                    _gunImage.color = WeaponHUD.Instance.img.color;
                    _punchImage.sprite = FistControl.Instance.fistIcon.sprite;
                    _punchImage.color = FistControl.Instance.fistIcon.color;
                    _railSlider.value = WeaponCharges.Instance.raicharge;
                }

                if (_dps.activeSelf)
                {
                    _dpsText.text = Math.Round(CalculateDps(), 2).ToString();
                }

                if (_speed.activeSelf)
                {
                    if (Subsettings["speed_mode"].GetValue<int>() == 0)
                    {
                        _speedText.fontSize = 72;
                        _speedText.text = Math.Round(NewMovement.Instance.rb.velocity.magnitude, 2).ToString();
                    } else
                    {
                        _speedText.fontSize = 42;
                        Vector3 velo = NewMovement.Instance.rb.velocity;
                        string text = new Vector3(velo.x, velo.y, velo.z).ToString();
                        text = text.Replace("(", "").Replace(")", "").Replace(", ", "\n");

                        _speedText.text = text;
                    }
                }
            }
        }

        public class PanelPatches
        {
            [HarmonyPatch(typeof(EnemyIdentifier), nameof(EnemyIdentifier.DeliverDamage)), HarmonyPrefix]
            private static void SetHealthBefore(EnemyIdentifier __instance, out float __state)
            {
                __state = __instance.health;
            }

            [HarmonyPatch(typeof(EnemyIdentifier), nameof(EnemyIdentifier.DeliverDamage)), HarmonyPostfix]
            private static void DoHealthAfter(EnemyIdentifier __instance, float __state)
            {
                float realHealth = __instance.health;
                if (realHealth < 0)
                {
                    realHealth = 0;
                }

                float damage = __state - realHealth;
                if (damage != 0f)
                {
                    HitsSecond.Add(new Hit(damage));
                }
            }
        }
    }
}

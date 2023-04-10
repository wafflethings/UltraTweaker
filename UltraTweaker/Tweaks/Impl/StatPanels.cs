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
    [TweakMetadata("Stat Panels", $"{UltraTweaker.GUID}.stat_panels", "Various info panels.", $"{UltraTweaker.GUID}.hud", 1)]
    public class StatPanels : Tweak
    {
        private Harmony harmony = new($"{UltraTweaker.GUID}.stat_panels");

        private GameObject originalPanels;
        private GameObject currentPanels;

        private GameObject DPS;
        private Text DPSText;

        private GameObject Speed;
        private Text SpeedText;

        private GameObject Info;
        private Text HPText;
        private Text StaminaText;

        private GameObject Weapons;
        private Image GunImage;
        private Image PunchImage;
        private Slider RailSlider;

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
                    new DropdownSubsettingElement(new List<string>() { "(x) m/s", "(x, y, z) m/s"} ), 0, 1, 0)  },

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
            Destroy(currentPanels);
            harmony.UnpatchSelf();
        }

        public override void OnSubsettingUpdate()
        {
            if (CanvasController.Instance != null && IsGameplayScene() && currentPanels != null)
            {
                DPS.SetActive(Subsettings["dps"].GetValue<bool>());
                Speed.SetActive(Subsettings["speed"].GetValue<bool>());
                Info.SetActive(Subsettings["info"].GetValue<bool>());
                Weapons.SetActive(Subsettings["weapons"].GetValue<bool>());

                foreach (GameObject row in currentPanels.ChildrenList())
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
                    }
                }

                currentPanels.transform.SetAsFirstSibling();
                currentPanels.transform.localScale = Vector3.one * Subsettings["size"].GetValue<int>() / 100f;
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
                if (originalPanels == null)
                {
                    originalPanels = AssetHandler.Bundle.LoadAsset<GameObject>("All Panels.prefab");
                }

                currentPanels = Instantiate(originalPanels, CanvasController.Instance.transform);

                DPS = currentPanels.ChildByName("Top Row").ChildByName("DPS");
                Speed = currentPanels.ChildByName("Top Row").ChildByName("Speed");
                Info = currentPanels.ChildByName("Bottom Row").ChildByName("Info");
                Weapons = currentPanels.ChildByName("Bottom Row").ChildByName("Weapons");

                DPSText = DPS.ChildByName("DPS").GetComponent<Text>();
                SpeedText = Speed.ChildByName("SPEED").GetComponent<Text>();
                HPText = Info.ChildByName("HP").GetComponent<Text>();
                StaminaText = Info.ChildByName("Stamina").GetComponent<Text>();
                GunImage = Weapons.ChildByName("Gun").GetComponent<Image>();
                PunchImage = Weapons.ChildByName("Fist").GetComponent<Image>();
                RailSlider = Weapons.ChildByName("Slider").GetComponent<Slider>();

                OnSubsettingUpdate();
            }
        }

        private float CalculateDPS()
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
            if (currentPanels != null)
            {
                if (Info.activeSelf && NewMovement.Instance != null)
                {
                    string prefix = "";
                    string suffix = "";
                    if (NewMovement.Instance.antiHp != 0)
                    {
                        prefix += "<color=#7C7A7B>";
                        suffix += "</color>";
                    }

                    HPText.text = $"{NewMovement.Instance.hp}{prefix} / {100 - Math.Round(NewMovement.Instance.antiHp, 0)}{suffix}";
                    StaminaText.text = $"{(NewMovement.Instance.boostCharge / 100).ToString("0.00")} / 3.00";
                }

                if (Weapons.activeSelf && NewMovement.Instance != null && WeaponHUD.Instance != null && WeaponCharges.Instance != null && FistControl.Instance != null)
                {
                    GunImage.sprite = WeaponHUD.Instance.img.sprite;
                    GunImage.color = WeaponHUD.Instance.img.color;
                    PunchImage.sprite = FistControl.Instance.fistIcon.sprite;
                    PunchImage.color = FistControl.Instance.fistIcon.color;
                    RailSlider.value = WeaponCharges.Instance.raicharge;
                }

                if (DPS.activeSelf)
                {
                    DPSText.text = Math.Round(CalculateDPS(), 2).ToString();
                }

                if (Speed.activeSelf)
                {
                    if (Subsettings["speed_mode"].GetValue<int>() == 0)
                    {
                        SpeedText.fontSize = 72;
                        SpeedText.text = Math.Round(NewMovement.Instance.rb.velocity.magnitude, 2).ToString();
                    } else
                    {
                        SpeedText.fontSize = 42;
                        Vector3 velo = NewMovement.Instance.rb.velocity;
                        string text = new Vector3(velo.x, velo.y, velo.z).ToString();
                        text = text.Replace("(", "").Replace(")", "").Replace(", ", "\n");

                        SpeedText.text = text;
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

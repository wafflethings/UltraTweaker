using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Text;
using UltraTweaker.Subsettings;
using UltraTweaker.Tweaks;
using UnityEngine;
using UnityEngine.UI;

namespace UltraTweaker.Handlers
{
    public static class SettingUIHandler
    {
        public static Harmony Harmony = new($"{UltraTweaker.GUID}.setting_ui_handler");

        public static GameObject OriginalSettingMenu;
        public static GameObject OriginalSettingPage;
        public static GameObject OriginalPageButton;
        public static GameObject CurrentSettingMenu;
        public static GameObject OriginalResetButton;
        public static GameObject CurrentResetButton;
        public static GameObject NewButton;

        public static Dictionary<string, Page> Pages = new()
        {
            { $"{UltraTweaker.GUID}.misc", new Page("TWEAKS: MISC") },
            { $"{UltraTweaker.GUID}.hud", new Page("TWEAKS: HUD") },
            { $"{UltraTweaker.GUID}.cybergrind", new Page("TWEAKS: CYBERGRIND") },
            { $"{UltraTweaker.GUID}.fun", new Page("TWEAKS: FUN") },
            { $"{UltraTweaker.GUID}.mutators", new Page("TWEAKS: MUTATORS") },
        };

        public static void Patch()
        {
            Harmony.PatchAll(typeof(SettingUIPatches));
        }

        public static void CreateUI(GameObject optionsMenu)
        {
            if (OriginalSettingMenu == null)
            {
                OriginalSettingMenu = AssetHandler.Bundle.LoadAsset<GameObject>("Settings Menu.prefab");
            }

            if (OriginalSettingPage == null)
            {
                OriginalSettingPage = AssetHandler.Bundle.LoadAsset<GameObject>("Settings Page.prefab");
            }

            if (OriginalPageButton == null)
            {
                OriginalPageButton = AssetHandler.Bundle.LoadAsset<GameObject>("Page Button.prefab");
            }

            if (OriginalResetButton == null)
            {
                OriginalResetButton = AssetHandler.Bundle.LoadAsset<GameObject>("Reset Button.prefab");
            }

            CurrentSettingMenu = GameObject.Instantiate(OriginalSettingMenu, optionsMenu.transform);

            int PagesSoFar = 0;

            foreach (Page page in Pages.Values)
            {
                page.PageObject = GameObject.Instantiate(OriginalSettingPage, CurrentSettingMenu.transform);
                page.PageObject.ChildByName("Text").GetComponent<Text>().text = $"--{page.PageName}--";
                page.PageObject.name = page.PageName;
                page.PageObject.AddComponent<HudOpenEffect>();

                if (PagesSoFar != 0)
                {
                    //first page should start open!
                    page.PageObject.SetActive(false);
                }

                GameObject pageButton = GameObject.Instantiate(OriginalPageButton, CurrentSettingMenu.ChildByName("Page Button Holder").transform);
                pageButton.transform.SetSiblingIndex(PagesSoFar);
                pageButton.GetComponent<Button>().onClick.AddListener(() =>
                {
                    foreach (Page page2 in Pages.Values)
                    {
                        if (page2 != page)
                        {
                            page2.PageObject.SetActive(false);
                        } else
                        {
                            page2.PageObject.SetActive(true);
                        }
                    }
                });
                pageButton.ChildByName("Text").GetComponent<Text>().text = (PagesSoFar + 1).ToString();

                if(PagesSoFar == 0)
                {
                    CurrentResetButton = GameObject.Instantiate(OriginalResetButton, page.PageObject.GetComponentInChildren<LayoutGroup>(true).transform);
                    CurrentResetButton.GetComponent<Button>().onClick.AddListener(() =>
                    {
                        ResetAllSettings();
                        GameObject.Destroy(CurrentSettingMenu);
                        GameObject.Destroy(NewButton);
                        CreateUI(optionsMenu);
                        CurrentSettingMenu.SetActive(true);
                    });
                }

                PagesSoFar += 1;
            }

            foreach (Tweak tw in UltraTweaker.AllTweaks.Values)
            {
                TweakMetadata meta = Attribute.GetCustomAttribute(tw.GetType(), typeof(TweakMetadata)) as TweakMetadata;
                tw.Element.Create(Pages[meta.PageId].PageObject.GetComponentInChildren<VerticalLayoutGroup>().transform).transform.SetSiblingIndex(meta.InsertAt);
                
                foreach (Subsetting sub in tw.Subsettings.Values)
                {
                    sub.Element.Create(tw.Element.CurrentSetting.GetComponentInChildren<LayoutGroup>(true).transform);
                }
            }

            NewButton = GameObject.Instantiate(optionsMenu.ChildByName("Gameplay"), optionsMenu.transform);

            NewButton.transform.localPosition += new Vector3(0, (95 * 1080) / Screen.height, 0);
            Vector3 newPos = NewButton.transform.position;
            newPos.y = (int)newPos.y;
            NewButton.transform.position = newPos;

            foreach (GameObject child in optionsMenu.ChildrenList())
            {
                if (child.TryGetComponent(out Button bu))
                {
                    bu.onClick.AddListener(() =>
                    {
                        CurrentSettingMenu.SetActive(false);
                    });
                }
            }

            NewButton.ChildByName("Text").GetComponent<Text>().text = "ULTRATWEAKER";
            NewButton.GetComponent<Button>().onClick = new();

            List<GameObject> toDisable = new()
            {
                optionsMenu.ChildByName("Gameplay Options"),
                optionsMenu.ChildByName("Controls Options"),
                optionsMenu.ChildByName("Video Options"),
                optionsMenu.ChildByName("Audio Options"),
                optionsMenu.ChildByName("HUD Options"),
                optionsMenu.ChildByName("Assist Options"),
                optionsMenu.ChildByName("ColorBlindness Options")
            };

            NewButton.GetComponent<Button>().onClick.AddListener(() =>
            {
                foreach (GameObject go in toDisable)
                {
                    go.SetActive(false);
                }

                CurrentSettingMenu.SetActive(true);
            });

            CurrentResetButton.transform.SetAsFirstSibling();
            CurrentSettingMenu.AddComponent<HudOpenEffect>();
            CurrentSettingMenu.SetActive(false);
        }

        public static void ResetAllSettings()
        {
            foreach (Tweak tw in UltraTweaker.AllTweaks.Values)
            {
                tw.IsEnabled = false;

                foreach (Subsetting sub in tw.Subsettings.Values)
                {
                    sub.ResetValue();
                }
            }
        }

        public class SettingUIPatches
        {
            [HarmonyPatch(typeof(OptionsMenuToManager), nameof(OptionsMenuToManager.Start)), HarmonyPostfix]
            public static void AddMenu(OptionsMenuToManager __instance)
            {
                CreateUI(__instance.gameObject.ChildByName("OptionsMenu"));
            }
        }

        public class Page
        {
            public string PageName;
            public GameObject PageObject;

            public Page(string PageName)
            {
                this.PageName = PageName;
            }
        }
    }
}

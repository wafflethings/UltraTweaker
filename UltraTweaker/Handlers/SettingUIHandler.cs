using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Text;
using UltraTweaker.Tweaks;
using UnityEngine;
using UnityEngine.UI;

namespace UltraTweaker.Handlers
{
    public static class SettingUIHandler
    {
        public static Harmony harmony = new($"{UltraTweaker.GUID}.setting_ui_handler");

        public static GameObject originalSettingMenu;
        public static GameObject originalSettingPage;
        public static GameObject originalPageButton;
        public static GameObject currentSettingMenu;

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
            harmony.PatchAll(typeof(SettingUIPatches));
        }

        public static void CreateUI(GameObject optionsMenu)
        {
            if (originalSettingMenu == null)
            {
                originalSettingMenu = AssetHandler.Bundle.LoadAsset<GameObject>("Settings Menu.prefab");
            }

            if (originalSettingPage == null)
            {
                originalSettingPage = AssetHandler.Bundle.LoadAsset<GameObject>("Settings Page.prefab");
            }

            if (originalPageButton == null)
            {
                originalPageButton = AssetHandler.Bundle.LoadAsset<GameObject>("Page Button.prefab");
            }

            currentSettingMenu = GameObject.Instantiate(originalSettingMenu, optionsMenu.transform);

            int PagesSoFar = 0;

            foreach (Page page in Pages.Values)
            {
                page.PageObject = GameObject.Instantiate(originalSettingPage, currentSettingMenu.transform);
                page.PageObject.ChildByName("Text").GetComponent<Text>().text = $"--{page.PageName}--";
                page.PageObject.name = page.PageName;
                page.PageObject.AddComponent<HudOpenEffect>();

                if (PagesSoFar != 0)
                {
                    //first page should start open!
                    page.PageObject.SetActive(false);
                }

                GameObject pageButton = GameObject.Instantiate(originalPageButton, currentSettingMenu.ChildByName("Page Button Holder").transform);
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

                PagesSoFar += 1;
            }

            foreach (Tweak tw in UltraTweaker.AllTweaks.Values)
            {
                TweakMetadata meta = Attribute.GetCustomAttribute(tw.GetType(), typeof(TweakMetadata)) as TweakMetadata;
                tw.Element.Create(Pages[meta.PageId].PageObject.GetComponentInChildren<VerticalLayoutGroup>().transform).transform.SetSiblingIndex(meta.insertAt);
                
                foreach (Subsetting sub in tw.Subsettings.Values)
                {
                    sub.element.Create(tw.Element.currentSetting.GetComponentInChildren<LayoutGroup>(true).transform);
                }
            }

            GameObject newBtn = GameObject.Instantiate(optionsMenu.ChildByName("Gameplay"), optionsMenu.transform);

            newBtn.transform.localPosition += new Vector3(0, (95 * 1080) / Screen.height, 0);
            Vector3 newPos = newBtn.transform.position;
            newPos.y = (int)newPos.y;
            newBtn.transform.position = newPos;

            foreach (GameObject child in optionsMenu.ChildrenList())
            {
                if (child.TryGetComponent(out Button bu))
                {
                    bu.onClick.AddListener(() =>
                    {
                        currentSettingMenu.SetActive(false);
                    });
                }
            }

            newBtn.ChildByName("Text").GetComponent<Text>().text = "ULTRATWEAKER";
            newBtn.GetComponent<Button>().onClick = new();

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

            newBtn.GetComponent<Button>().onClick.AddListener(() =>
            {
                foreach (GameObject go in toDisable)
                {
                    go.SetActive(false);
                }

                currentSettingMenu.SetActive(true);
            });

            currentSettingMenu.AddComponent<HudOpenEffect>();
            currentSettingMenu.SetActive(false);
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

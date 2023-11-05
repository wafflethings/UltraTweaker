using System;
using System.Collections.Generic;
using System.Text;
using UltraTweaker.Handlers;
using UltraTweaker.Subsettings.Impl;
using UltraTweaker.Tweaks;
using UnityEngine;
using UnityEngine.UI;

namespace UltraTweaker.UIElements.Impl
{
    public class DropdownIntSubsettingElement : SubsettingUIElement
    {
        public static GameObject BaseSetting;
        public GameObject CurrentSetting;
        public List<string> Options;
        public Dropdown Dropdown;

        public DropdownIntSubsettingElement(List<string> options)
        {
            Options = options;
        }

        public override GameObject Create(Transform t)
        {
            if (BaseSetting == null)
            {
                BaseSetting = AssetHandler.Bundle.LoadAsset<GameObject>("Dropdown Subsetting.prefab");
            }

            CurrentSetting = GameObject.Instantiate(BaseSetting, t);
            CurrentSetting.ChildByName("Text").GetComponent<Text>().text = Subsetting.Metadata.Name.ToUpper();
            CurrentSetting.ChildByName("Box").ChildByName("Description").GetComponent<Text>().text = Subsetting.Metadata.Description;
            CurrentSetting.ChildByName("Text").AddComponent<ShowBoxOnHover>();
            Dropdown = CurrentSetting.ChildByName("Dropdown").GetComponent<Dropdown>();
            Dropdown.ClearOptions();
            Dropdown.AddOptions(Options);

            Dropdown.value = ((IntSubsetting)Subsetting).Value;

            Dropdown.onValueChanged.AddListener((num) =>
            {
                ((IntSubsetting)Subsetting).Value = num;

                if (Subsetting.Parent.IsEnabled)
                {
                    Subsetting.Parent.OnSubsettingUpdate();
                }
            });

            return CurrentSetting;
        }

        public override void SetControlsActive(bool active)
        {
            Dropdown.interactable = active;
        }
    }
}

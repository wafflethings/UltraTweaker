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
    public class BoolSubsettingElement : SubsettingUIElement
    {
        public static GameObject BaseSetting;
        public GameObject CurrentSetting;
        public Toggle Toggle;

        public override GameObject Create(Transform t)
        {
            if (BaseSetting == null)
            {
                BaseSetting = AssetHandler.Bundle.LoadAsset<GameObject>("Toggle Subsetting.prefab");
            }

            CurrentSetting = GameObject.Instantiate(BaseSetting, t);
            CurrentSetting.ChildByName("Text").GetComponent<Text>().text = Subsetting.Metadata.Name.ToUpper();
            Toggle = CurrentSetting.ChildByName("Toggle").GetComponent<Toggle>();
            CurrentSetting.ChildByName("Box").ChildByName("Description").GetComponent<Text>().text = Subsetting.Metadata.Description;
            CurrentSetting.ChildByName("Text").AddComponent<ShowBoxOnHover>();

            Toggle.isOn = ((BoolSubsetting)Subsetting).Value;

            Toggle.onValueChanged.AddListener((state) =>
            {
                ((BoolSubsetting)Subsetting).Value = state;

                if (Subsetting.Parent.IsEnabled)
                {
                    Subsetting.Parent.OnSubsettingUpdate();
                }
            });

            return CurrentSetting;
        }

        public override void SetControlsActive(bool active)
        {
            Toggle.interactable = active;
        }
    }
}

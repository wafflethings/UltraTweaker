using System;
using System.Collections.Generic;
using System.Text;
using UltraTweaker.Handlers;
using UltraTweaker.Tweaks;
using UnityEngine;
using UnityEngine.UI;

namespace UltraTweaker.UIElements
{
    public class TweakUIElement
    {
        public static GameObject BaseSetting;
        public GameObject CurrentSetting;
        public Tweak Tweak;
        public Toggle Toggle;

        public virtual string Name => "Tweak Setting.prefab";

        public TweakUIElement(Tweak tweak)
        {
            Tweak = tweak;
        }

        public GameObject Create(Transform t)
        {
            if (BaseSetting == null)
            {
                BaseSetting = AssetHandler.Bundle.LoadAsset<GameObject>(Name);
            }

            Metadata meta = Attribute.GetCustomAttribute(Tweak.GetType(), typeof(Metadata)) as Metadata;
            CurrentSetting = GameObject.Instantiate(BaseSetting, t);
            Toggle = CurrentSetting.ChildByName("Toggle").GetComponent<Toggle>();

            Toggle.onValueChanged.AddListener((state) =>
            {
                Tweak.IsEnabled = state;
            });

            Toggle.isOn = Tweak.IsEnabled;

            // hardcoded subsetting size is 30
            // i would make it dynamic but contentsizefitter doesnt set the size until after this code is run, bc ofc it doesnt
            float preferredHeight = Tweak.Subsettings.Count * 30;
            preferredHeight += (Tweak.Subsettings.Count - 1) * CurrentSetting.ChildByName("Subsetting Holder").GetComponent<VerticalLayoutGroup>().spacing;

            CurrentSetting.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical,
                CurrentSetting.ChildByName("Background").GetComponent<RectTransform>().rect.height +
                preferredHeight);

            CurrentSetting.ChildByName("Text").GetComponent<Text>().text = meta.Name.ToUpper();
            CurrentSetting.ChildByName("Box").ChildByName("Description").GetComponent<Text>().text = meta.Description;
            CurrentSetting.ChildByName("Text").AddComponent<ShowBoxOnHover>();

            if (((TweakMetadata)meta).Icon != null)
            {
                CurrentSetting.ChildByName("Icon").GetComponent<Image>().sprite = ((TweakMetadata)meta).Icon;
            }
            else
            {
                CurrentSetting.ChildByName("Icon").SetActive(false);
            }

            if (!((TweakMetadata)meta).AllowCG)
            {
                CurrentSetting.ChildByName("Disabled CG").SetActive(true);
                CurrentSetting.ChildByName("Disabled CG").AddComponent<ShowBoxOnHover>().Box = CurrentSetting.ChildByName("Disabled CG").ChildByName("Box");
            }

            return CurrentSetting;
        }

        public void SetControlsActive(bool active)
        {
            Toggle.interactable = active;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UltraTweaker.Handlers;
using UltraTweaker.Subsettings.Impl;
using UltraTweaker.Tweaks;
using UnityEngine;
using UnityEngine.UI;

namespace UltraTweaker.UIElements.Impl
{
    public class SliderIntSubsettingElement : SubsettingUIElement
    {
        public static GameObject BaseSetting;
        public GameObject CurrentSetting;
        public string DisplayAs = "{0}";
        public InputField InputField;
        public Slider Slider;

        public SliderIntSubsettingElement(string displayAs = "{0}")
        {
            DisplayAs = displayAs;
        }

        public override GameObject Create(Transform t)
        {
            if (BaseSetting == null)
            {
                BaseSetting = AssetHandler.Bundle.LoadAsset<GameObject>("Slider Subsetting.prefab");
            }

            CurrentSetting = GameObject.Instantiate(BaseSetting, t);
            CurrentSetting.ChildByName("Text").GetComponent<Text>().text = Subsetting.Metadata.Name.ToUpper();
            CurrentSetting.ChildByName("Box").ChildByName("Description").GetComponent<Text>().text = Subsetting.Metadata.Description;
            CurrentSetting.ChildByName("Text").AddComponent<ShowBoxOnHover>();
            InputField = CurrentSetting.ChildByName("InputField").GetComponent<InputField>();
            Slider = CurrentSetting.ChildByName("Slider").GetComponent<Slider>();
            Slider.maxValue = ((IntSubsetting)Subsetting).MaxValue;
            Slider.minValue = ((IntSubsetting)Subsetting).MinValue;

            InputField.onEndEdit.AddListener((str) =>
            {
                static string RemoveNonNumeric(string s)
                {
                    string addToStart = "";

                    if (s.StartsWith("-"))
                    {
                        addToStart += "-";
                    }

                    return addToStart + string.Concat(s?.Where(c => char.IsNumber(c)) ?? "");
                }

                str = RemoveNonNumeric(str);
                ((IntSubsetting)Subsetting).Value = int.Parse(str);

                Slider.value = int.Parse(str);
                InputField.text = string.Format(DisplayAs, int.Parse(str));

                if (Subsetting.Parent.IsEnabled)
                {
                    Subsetting.Parent.OnSubsettingUpdate();
                }
            });

            Slider.value = ((IntSubsetting)Subsetting).Value;
            InputField.text = string.Format(DisplayAs, ((IntSubsetting)Subsetting).Value);

            Slider.onValueChanged.AddListener((num) =>
            {
                // the point of this is so that you can go above the max.
                if (((IntSubsetting)Subsetting).Value <= Slider.maxValue || Slider.isPointerDown)
                {
                    ((IntSubsetting)Subsetting).Value = (int)num;
                    InputField.text = string.Format(DisplayAs, (int)num);

                    if (Subsetting.Parent.IsEnabled)
                    {
                        Subsetting.Parent.OnSubsettingUpdate();
                    }
                }
            });

            return CurrentSetting;
        }

        public override void SetControlsActive(bool active)
        {
            Slider.interactable = active;
            InputField.interactable = active;
        }
    }
}

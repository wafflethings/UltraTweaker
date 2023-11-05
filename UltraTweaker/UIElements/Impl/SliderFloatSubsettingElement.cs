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
    public class SliderFloatSubsettingElement : SubsettingUIElement
    {
        public static GameObject BaseSetting;
        public GameObject CurrentSetting;
        public string DisplayAs = "{0}";
        public InputField InputField;
        public Slider Slider;
        public int RoundDigits;

        public SliderFloatSubsettingElement(string displayAs = "{0}", int roundDigits = 1)
        {
            DisplayAs = displayAs;
            RoundDigits = roundDigits;
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
            Slider.maxValue = ((FloatSubsetting)Subsetting).MaxValue;
            Slider.minValue = ((FloatSubsetting)Subsetting).MinValue;
            Slider.wholeNumbers = false;

            InputField.onEndEdit.AddListener((str) =>
            {
                static string RemoveNonNumeric(string s)
                {
                    string addToStart = "";

                    if (s.StartsWith("-"))
                    {
                        addToStart += "-";
                    }

                    return addToStart + string.Concat(s?.Where(c => char.IsNumber(c) || c == '.') ?? "");
                }

                str = RemoveNonNumeric(str);
                ((FloatSubsetting)Subsetting).Value = float.Parse(str);


                Slider.value = (float)Math.Round(float.Parse(str), RoundDigits);
                InputField.text = string.Format(DisplayAs, (float)Math.Round(float.Parse(str), RoundDigits));
            });

            Slider.value = ((FloatSubsetting)Subsetting).Value;
            InputField.text = string.Format(DisplayAs, ((FloatSubsetting)Subsetting).Value);

            Slider.onValueChanged.AddListener((num) =>
            {
                // the point of this is so that you can go above the max.
                if (((FloatSubsetting)Subsetting).Value <= Slider.maxValue || Slider.isPointerDown)
                {
                    ((FloatSubsetting)Subsetting).Value = (float)Math.Round(num, RoundDigits);
                    InputField.text = string.Format(DisplayAs, (float)Math.Round(num, RoundDigits));

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

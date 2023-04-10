using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UltraTweaker.Handlers;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UltraTweaker.Tweaks
{
    public class TweakUIElement
    {
        public static GameObject baseSetting;
        public GameObject currentSetting;
        public Tweak tweak;
        public Toggle toggle;

        public virtual string Name => "Tweak Setting.prefab";

        public TweakUIElement(Tweak tweak)
        {
            this.tweak = tweak;
        }

        public virtual GameObject Create(Transform t)
        {
            if (baseSetting == null)
            {
                baseSetting = AssetHandler.Bundle.LoadAsset<GameObject>(Name);
            }

            Metadata meta = Attribute.GetCustomAttribute(tweak.GetType(), typeof(Metadata)) as Metadata;
            currentSetting = GameObject.Instantiate(baseSetting, t);
            toggle = currentSetting.ChildByName("Toggle").GetComponent<Toggle>();

            toggle.onValueChanged.AddListener((state) =>
            {
                tweak.IsEnabled = state;
            });

            toggle.isOn = tweak.IsEnabled;

            // hardcoded subsetting size is 30
            // i would make it dynamic but contentsizefitter doesnt set the size until after this code is run, bc ofc it doesnt
            float preferredHeight = tweak.Subsettings.Count * 30;
            preferredHeight += (tweak.Subsettings.Count - 1) * currentSetting.ChildByName("Subsetting Holder").GetComponent<VerticalLayoutGroup>().spacing;

            currentSetting.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 
                currentSetting.ChildByName("Background").GetComponent<RectTransform>().rect.height +
                preferredHeight);

            currentSetting.ChildByName("Text").GetComponent<Text>().text = meta.Name.ToUpper();
            currentSetting.ChildByName("Box").ChildByName("Description").GetComponent<Text>().text = meta.Description;
            currentSetting.ChildByName("Text").AddComponent<ShowBoxOnHover>();

            if (((TweakMetadata)meta).Icon != null)
            {
                currentSetting.ChildByName("Icon").GetComponent<Image>().sprite = ((TweakMetadata)meta).Icon;
            } 
            else
            {
                currentSetting.ChildByName("Icon").SetActive(false);
            }

            if (!((TweakMetadata)meta).AllowCG)
            {
                currentSetting.ChildByName("Disabled CG").SetActive(true);
                currentSetting.ChildByName("Disabled CG").AddComponent<ShowBoxOnHover>().Box = currentSetting.ChildByName("Disabled CG").ChildByName("Box");
            }

            return currentSetting;
        }
    }

    /* public class MutatorUIElement : TweakUIElement
    {
        public override string Name => "Mutator Setting.prefab";
        public Sprite icon;

        public MutatorUIElement(Tweak tweak, string iconName) : base(tweak)
        {
            if (icon == null)
            {
                icon = AssetHandler.Bundle.LoadAsset<Sprite>(iconName);
            }
        }

        public override GameObject Create(Transform t)
        {
            GameObject og = base.Create(t);
            og.ChildByName("Image").GetComponent<Image>().sprite = icon;
            return og;
        }
    } */

    public abstract class SubsettingUIElement
    {
        public Subsetting subsetting;

        public abstract GameObject Create(Transform t);
    }

    public class SliderFloatSubsettingElement : SubsettingUIElement
    {
        public static GameObject baseSetting;
        public GameObject currentSetting;
        public string displayAs = "{0}";
        public InputField inputField;
        public Slider slider;
        public int roundDigits;

        public SliderFloatSubsettingElement(string displayAs = "{0}", int roundDigits = 1)
        {
            this.displayAs = displayAs;
            this.roundDigits = roundDigits;
        }

        public override GameObject Create(Transform t)
        {
            if (baseSetting == null)
            {
                baseSetting = AssetHandler.Bundle.LoadAsset<GameObject>("Slider Subsetting.prefab");
            }

            currentSetting = GameObject.Instantiate(baseSetting, t);
            currentSetting.ChildByName("Text").GetComponent<Text>().text = subsetting.metadata.Name.ToUpper();
            currentSetting.ChildByName("Box").ChildByName("Description").GetComponent<Text>().text = subsetting.metadata.Description;
            currentSetting.ChildByName("Text").AddComponent<ShowBoxOnHover>();
            inputField = currentSetting.ChildByName("InputField").GetComponent<InputField>();
            slider = currentSetting.ChildByName("Slider").GetComponent<Slider>();
            slider.maxValue = ((FloatSubsetting)subsetting).maxValue;
            slider.minValue = ((FloatSubsetting)subsetting).minValue;
            slider.wholeNumbers = false;

            inputField.onEndEdit.AddListener((str) =>
            {
                static string RemoveNonNumeric(string s)
                {
                    string addToStart = "";

                    if (s.StartsWith("-") )
                    {
                        addToStart += "-";
                    }

                    return addToStart + string.Concat(s?.Where(c => char.IsNumber(c) || c == '.') ?? "");
                }

                str = RemoveNonNumeric(str);
                ((FloatSubsetting)subsetting).value = float.Parse(str);


                slider.value = (float)Math.Round(float.Parse(str), roundDigits);
                inputField.text = string.Format(displayAs, (float)Math.Round(float.Parse(str), roundDigits));
            });

            slider.value = ((FloatSubsetting)subsetting).value;
            inputField.text = string.Format(displayAs, ((FloatSubsetting)subsetting).value);

            slider.onValueChanged.AddListener((num) =>
            {
                // the point of this is so that you can go above the max.
                if (((FloatSubsetting)subsetting).value <= slider.maxValue || slider.isPointerDown)
                {
                    ((FloatSubsetting)subsetting).value = (float)Math.Round(num, roundDigits);
                    inputField.text = string.Format(displayAs, (float)Math.Round(num, roundDigits));

                    if (subsetting.parent.IsEnabled)
                    {
                        subsetting.parent.OnSubsettingUpdate();
                    }
                }
            });

            return currentSetting;
        }
    }

    public class SliderIntSubsettingElement : SubsettingUIElement
    {
        public static GameObject baseSetting;
        public GameObject currentSetting;
        public string displayAs = "{0}";
        public InputField inputField;
        public Slider slider;

        public SliderIntSubsettingElement(string displayAs = "{0}")
        {
            this.displayAs = displayAs;
        }

        public override GameObject Create(Transform t)
        {
            if (baseSetting == null)
            {
                baseSetting = AssetHandler.Bundle.LoadAsset<GameObject>("Slider Subsetting.prefab");
            }

            currentSetting = GameObject.Instantiate(baseSetting, t);
            currentSetting.ChildByName("Text").GetComponent<Text>().text = subsetting.metadata.Name.ToUpper();
            currentSetting.ChildByName("Box").ChildByName("Description").GetComponent<Text>().text = subsetting.metadata.Description;
            currentSetting.ChildByName("Text").AddComponent<ShowBoxOnHover>();
            inputField = currentSetting.ChildByName("InputField").GetComponent<InputField>();
            slider = currentSetting.ChildByName("Slider").GetComponent<Slider>();
            slider.maxValue = ((IntSubsetting)subsetting).maxValue;
            slider.minValue = ((IntSubsetting)subsetting).minValue;

            inputField.onEndEdit.AddListener((str) =>
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
                ((IntSubsetting)subsetting).value = int.Parse(str);

                slider.value = int.Parse(str);
                inputField.text = string.Format(displayAs, int.Parse(str));

                if (subsetting.parent.IsEnabled)
                {
                    subsetting.parent.OnSubsettingUpdate();
                }
            });

            slider.value = ((IntSubsetting)subsetting).value;
            inputField.text = string.Format(displayAs, ((IntSubsetting)subsetting).value);

            slider.onValueChanged.AddListener((num) =>
            {
                // the point of this is so that you can go above the max.
                if (((IntSubsetting)subsetting).value <= slider.maxValue || slider.isPointerDown)
                {
                    ((IntSubsetting)subsetting).value = (int)num;
                    inputField.text = string.Format(displayAs, (int)num);

                    if (subsetting.parent.IsEnabled)
                    {
                        subsetting.parent.OnSubsettingUpdate();
                    }
                }
            });

            return currentSetting;
        }
    }

    public class BoolSubsettingElement : SubsettingUIElement
    {
        public static GameObject baseSetting;
        public GameObject currentSetting;
        public Toggle toggle;

        public override GameObject Create(Transform t)
        {
            if (baseSetting == null)
            {
                baseSetting = AssetHandler.Bundle.LoadAsset<GameObject>("Toggle Subsetting.prefab");
            }

            currentSetting = GameObject.Instantiate(baseSetting, t);
            currentSetting.ChildByName("Text").GetComponent<Text>().text = subsetting.metadata.Name.ToUpper();
            toggle = currentSetting.ChildByName("Toggle").GetComponent<Toggle>();
            currentSetting.ChildByName("Box").ChildByName("Description").GetComponent<Text>().text = subsetting.metadata.Description;
            currentSetting.ChildByName("Text").AddComponent<ShowBoxOnHover>();

            toggle.isOn = ((BoolSubsetting)subsetting).value;

            toggle.onValueChanged.AddListener((state) =>
            {
                ((BoolSubsetting)subsetting).value = state;

                if (subsetting.parent.IsEnabled)
                {
                    subsetting.parent.OnSubsettingUpdate();
                }
            });

            return currentSetting;
        }
    }

    public class DropdownSubsettingElement : SubsettingUIElement
    {
        public static GameObject baseSetting;
        public GameObject currentSetting;
        public List<string> Options;
        public Dropdown dropdown;

        public DropdownSubsettingElement(List<string> Options)
        {
            this.Options = Options;
        }

        public override GameObject Create(Transform t)
        {
            if (baseSetting == null)
            {
                baseSetting = AssetHandler.Bundle.LoadAsset<GameObject>("Dropdown Subsetting.prefab");
            }

            currentSetting = GameObject.Instantiate(baseSetting, t);
            currentSetting.ChildByName("Text").GetComponent<Text>().text = subsetting.metadata.Name.ToUpper();
            currentSetting.ChildByName("Box").ChildByName("Description").GetComponent<Text>().text = subsetting.metadata.Description;
            currentSetting.ChildByName("Text").AddComponent<ShowBoxOnHover>();
            dropdown = currentSetting.ChildByName("Dropdown").GetComponent<Dropdown>();
            dropdown.ClearOptions();
            dropdown.AddOptions(Options);

            dropdown.value = ((IntSubsetting)subsetting).value;

            dropdown.onValueChanged.AddListener((num) =>
            {
                ((IntSubsetting)subsetting).value = num;

                if (subsetting.parent.IsEnabled)
                {
                    subsetting.parent.OnSubsettingUpdate();
                }
            });

            return currentSetting;
        }
    }

    public class ShowBoxOnHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        public GameObject Box;

        public void Start()
        {
            if (Box == null)
                Box = transform.parent.gameObject.ChildByName("Box");
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            Box.SetActive(true);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            Box.SetActive(false);
        }
    }
}

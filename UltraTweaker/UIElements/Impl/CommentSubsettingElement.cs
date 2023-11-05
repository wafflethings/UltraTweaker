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
    public class CommentSubsettingElement : SubsettingUIElement
    {
        public static GameObject BaseSetting;
        public GameObject CurrentSetting;

        public override GameObject Create(Transform t)
        {
            if (BaseSetting == null)
            {
                BaseSetting = AssetHandler.Bundle.LoadAsset<GameObject>("Comment Subsetting.prefab");
            }

            CurrentSetting = GameObject.Instantiate(BaseSetting, t);
            CurrentSetting.ChildByName("Text").GetComponent<Text>().text = Subsetting.Metadata.Name.ToUpper();
            CurrentSetting.ChildByName("Comment").GetComponent<Text>().text = Subsetting.Metadata.Description;

            GameObject actionBtn = CurrentSetting.ChildByName("Button");
            actionBtn.SetActive(false);

            CommentSubsetting cs = (Subsetting as CommentSubsetting);
            if (cs != null)
            {
                if (cs.Action != null)
                {
                    actionBtn.SetActive(true);
                    actionBtn.GetComponent<Button>().onClick.AddListener(() => cs.Action.Invoke());
                    actionBtn.ChildByName("Text").GetComponent<Text>().text = cs.ButtonText;
                }
            }

            return CurrentSetting;
        }

        public override void SetControlsActive(bool active)
        {
            //
        }
    }
}

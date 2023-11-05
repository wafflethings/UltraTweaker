using System;
using System.Collections.Generic;
using System.Text;
using UltraTweaker.Subsettings;
using UnityEngine;

namespace UltraTweaker.UIElements
{
    public abstract class SubsettingUIElement
    {
        public Subsetting Subsetting;
        public abstract GameObject Create(Transform t);
        public abstract void SetControlsActive(bool active);
    }
}

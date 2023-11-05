using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using UltraTweaker.Tweaks;
using UnityEngine;
using UltraTweaker.UIElements;

namespace UltraTweaker.Subsettings
{
    public abstract class Subsetting
    {
        public Tweak Parent;
        public Metadata Metadata;
        public SubsettingUIElement Element;

        public Subsetting(Tweak parent, Metadata metadata, SubsettingUIElement element)
        {
            Parent = parent;
            Metadata = metadata;
            Element = element;
            Element.Subsetting = this;
        }

        public T GetValue<T>()
        {
            return ((TypedSubsetting<T>)this).Value;
        }

        public virtual string Serialize()
        {
            return "";
        }

        public abstract void Deserialize(string str);

        public abstract void ResetValue();
    }
}

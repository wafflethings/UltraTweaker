using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using UnityEngine;

namespace UltraTweaker.Tweaks
{
    public abstract class Subsetting
    {
        public Tweak parent;
        public Metadata metadata;
        public SubsettingUIElement element;

        public Subsetting(Tweak parent, Metadata metadata, SubsettingUIElement element)
        {
            this.parent = parent;
            this.metadata = metadata;
            this.element = element;

            element.subsetting = this;
        }

        public T GetValue<T>()
        {
            return ((TypedSubsetting<T>)this).value;
        }

        public virtual string Serialize()
        {
            return "";
        }

        public abstract void Deserialize(string str);

        public abstract void ResetValue();
    }

    public class TypedSubsetting<T> : Subsetting
    {
        public T value;
        public T defaultValue;

        public override void Deserialize(string str) { }

        public TypedSubsetting(Tweak parent, Metadata metadata, SubsettingUIElement element) : base(parent, metadata, element)
        {

        }

        public override void ResetValue()
        {
            value = defaultValue;
        }
    }

    public class CommentSubsetting : TypedSubsetting<string>
    {
        public Action act;
        public string btnText;
        public CommentSubsetting(Tweak parent, Metadata metadata, SubsettingUIElement element, Action act = null, string btnText = "") : base(parent, metadata, element)
        {
            this.act = act;
            this.btnText = btnText;
        }

        public override string Serialize()
        {
            return "";
        }
    }

    public class BoolSubsetting : TypedSubsetting<bool>
    {
        public BoolSubsetting(Tweak parent, Metadata metadata, SubsettingUIElement element, bool defaultValue) : base(parent, metadata, element)
        {
            this.defaultValue = defaultValue;
            value = defaultValue;
        }

        public override void Deserialize(string str)
        {
            value = Convert.ToBoolean(str);
        }

        public override string Serialize()
        {
            return value.ToString();
        }
    }

    public class IntSubsetting : TypedSubsetting<int>
    {
        public int maxValue;
        public int minValue;

        public IntSubsetting(Tweak parent, Metadata metadata, SubsettingUIElement element, int defaultValue, int maxValue, int minValue) : base(parent, metadata, element)
        {
            this.defaultValue = defaultValue;
            this.maxValue = maxValue;
            this.minValue = minValue;

            value = defaultValue;
        }

        public override void Deserialize(string str)
        {
            value = int.Parse(str, CultureInfo.GetCultureInfo("en-GB"));
        }

        public override string Serialize()
        {
            return value.ToString();
        }
    }

    public class FloatSubsetting : TypedSubsetting<float>
    {
        public float maxValue;
        public float minValue;

        public FloatSubsetting(Tweak parent, Metadata metadata, SubsettingUIElement element, float defaultValue, float maxValue, float minValue) : base(parent, metadata, element)
        {
            this.defaultValue = defaultValue;
            this.maxValue = maxValue;
            this.minValue = minValue;

            value = defaultValue;
        }

        public override void Deserialize(string str)
        {
            value = float.Parse(str, CultureInfo.GetCultureInfo("en-GB"));
        }

        public override string Serialize()
        {
            return value.ToString();
        }
    }
}

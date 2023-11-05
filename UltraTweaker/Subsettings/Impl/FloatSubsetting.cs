using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using UltraTweaker.Tweaks;
using UltraTweaker.UIElements;

namespace UltraTweaker.Subsettings.Impl
{
    public class FloatSubsetting : TypedSubsetting<float>
    {
        public float MaxValue;
        public float MinValue;

        public FloatSubsetting(Tweak parent, Metadata metadata, SubsettingUIElement element, float defaultValue, float maxValue, float minValue) : base(parent, metadata, element)
        {
            DefaultValue = defaultValue;
            MaxValue = maxValue;
            MinValue = minValue;
            Value = defaultValue;
        }

        public override void Deserialize(string str)
        {
            Value = float.Parse(str, CultureInfo.GetCultureInfo("en-GB"));
        }

        public override string Serialize()
        {
            return Value.ToString();
        }
    }
}

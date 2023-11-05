using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using UltraTweaker.Tweaks;
using UltraTweaker.UIElements;

namespace UltraTweaker.Subsettings.Impl
{
    public class IntSubsetting : TypedSubsetting<int>
    {
        public int MaxValue;
        public int MinValue;

        public IntSubsetting(Tweak parent, Metadata metadata, SubsettingUIElement element, int defaultValue, int maxValue, int minValue) : base(parent, metadata, element)
        {
            DefaultValue = defaultValue;
            MaxValue = maxValue;
            MinValue = minValue;
            Value = defaultValue;
        }

        public override void Deserialize(string str)
        {
            Value = int.Parse(str, CultureInfo.GetCultureInfo("en-GB"));
        }

        public override string Serialize()
        {
            return Value.ToString();
        }
    }
}

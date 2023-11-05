using System;
using System.Collections.Generic;
using System.Text;
using UltraTweaker.Tweaks;
using UltraTweaker.UIElements;

namespace UltraTweaker.Subsettings.Impl
{
    public class StringSubsetting : TypedSubsetting<string>
    {
        public StringSubsetting(Tweak parent, Metadata metadata, SubsettingUIElement element, string defaultValue) : base(parent, metadata, element)
        {
            DefaultValue = defaultValue;
            Value = defaultValue;
        }

        public override void Deserialize(string str)
        {
            Value = str;
        }

        public override string Serialize()
        {
            return Value;
        }
    }
}

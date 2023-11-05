using System;
using System.Collections.Generic;
using System.Text;
using UltraTweaker.Tweaks;
using UltraTweaker.UIElements;

namespace UltraTweaker.Subsettings.Impl
{
    public class BoolSubsetting : TypedSubsetting<bool>
    {
        public BoolSubsetting(Tweak parent, Metadata metadata, SubsettingUIElement element, bool defaultValue) : base(parent, metadata, element)
        {
            DefaultValue = defaultValue;
            Value = defaultValue;
        }

        public override void Deserialize(string str)
        {
            Value = Convert.ToBoolean(str);
        }

        public override string Serialize()
        {
            return Value.ToString();
        }
    }
}

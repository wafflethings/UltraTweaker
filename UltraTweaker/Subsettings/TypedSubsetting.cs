using System;
using System.Collections.Generic;
using System.Text;
using UltraTweaker.Tweaks;
using UltraTweaker.UIElements;

namespace UltraTweaker.Subsettings
{
    public class TypedSubsetting<T> : Subsetting
    {
        public T Value;
        public T DefaultValue;

        public override void Deserialize(string str) { }

        public TypedSubsetting(Tweak parent, Metadata metadata, SubsettingUIElement element) : base(parent, metadata, element)
        {

        }

        public override void ResetValue()
        {
            Value = DefaultValue;
        }
    }
}

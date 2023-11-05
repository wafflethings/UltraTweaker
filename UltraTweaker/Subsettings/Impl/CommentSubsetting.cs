using System;
using System.Collections.Generic;
using System.Text;
using UltraTweaker.Tweaks;
using UltraTweaker.UIElements;

namespace UltraTweaker.Subsettings.Impl
{
    public class CommentSubsetting : TypedSubsetting<string>
    {
        public Action Action;
        public string ButtonText;

        public CommentSubsetting(Tweak parent, Metadata metadata, SubsettingUIElement element, Action action = null, string buttonText = "") : base(parent, metadata, element)
        {
            Action = action;
            ButtonText = buttonText;
        }

        public override string Serialize()
        {
            return "";
        }
    }
}

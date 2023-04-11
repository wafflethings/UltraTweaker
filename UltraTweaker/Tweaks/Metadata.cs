using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UltraTweaker.Handlers;
using UnityEngine;

namespace UltraTweaker.Tweaks
{
    public class Metadata : Attribute
    {
        public string Name { get; private set; }
        public string ID { get; private set; }
        public string Description { get; private set; }

        /// <summary>
        /// The generic metadata. Applies to both tweaks and subsettings.
        /// </summary>
        /// <param name="Name">The name of the option.</param>
        /// <param name="ID">The ID, used for saving.</param>
        /// <param name="Description">Text that appears when you hover on the option name.</param>
        public Metadata(string Name, string ID, string Description = "")
        {
            this.Name = Name;
            this.ID = ID;
            this.Description = Description;
        }
    }

    /// <summary>
    /// Tweak-specific metadata. Not for subsettings.
    /// </summary>
    public class TweakMetadata : Metadata
    {
        public string PageId { get; private set; }
        public int InsertAt { get; private set; }
        public Sprite Icon { get; private set; }
        public bool AllowCG { get; private set; }
        public bool IsMutator { get; private set; }

        /// <summary>
        /// Tweak-specific metadata. Not for subsettings.
        /// </summary>
        /// <param name="Name">The name of the option.</param>
        /// <param name="ID">The ID, used for saving.</param>
        /// <param name="Description">Text that appears when you hover on the option name.</param>
        /// <param name="PageId">Which page should the tweak be on?</param>
        /// <param name="InsertAt">What position on the page it is inserted at.</param>
        /// <param name="IconName">The name of the icon. Make sure to cache it with <see cref="AssetHandler.CacheAsset{T}(string, AssetBundle)"/></param>
        /// <param name="AllowCG">Should this tweak disable The Cyber Grind?</param>
        /// <param name="IsMutator">Is the tweak a mutator? It will show in the end screen,</param>
        public TweakMetadata(string Name, string ID, string Description = "", string PageId = $"{UltraTweaker.GUID}.misc", int InsertAt = 0, string IconName = null, bool AllowCG = true, bool IsMutator = false) : base(Name, ID, Description)
        {
            if (IconName != null)
            {
                Icon = AssetHandler.GetCachedAsset<Sprite>(IconName);
            }

            this.PageId = PageId;
            this.InsertAt = InsertAt;
            this.AllowCG = AllowCG;
            this.IsMutator = IsMutator;
        }
    }
}

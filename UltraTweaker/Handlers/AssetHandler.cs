using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

namespace UltraTweaker.Handlers
{
    public static class AssetHandler
    {
        public static readonly string AssetsPath = Path.Combine(PathUtils.ModPath(), "Assets");
        public static readonly string BundlePath = Path.Combine(AssetsPath, "ultratweaker_assets.bundle");
        public static AssetBundle Bundle { get; private set; }
        private static Dictionary<string, UnityEngine.Object> CachedAssets = new();

        internal static void LoadBundle()
        {
            Bundle = AssetBundle.LoadFromFile(BundlePath);
        }

        /// <summary>
        /// Loads a cached asset from memory.
        /// </summary>
        /// <param name="Name">The name of the asset.</param>
        public static T GetCachedAsset<T>(string Name) where T : UnityEngine.Object
        {
            if (!CachedAssets.ContainsKey(Name))
            {
                CacheAsset<T>(Name, Bundle);
            }
            return CachedAssets[Name] as T;
        }

        /// <summary>
        /// Caches an asset from a bundle.
        /// </summary>
        /// <param name="Name">The name of the asset.</param>
        /// <param name="bundle">The name of the bundle. UT's main bundle by default.</param>
        public static void CacheAsset<T>(string Name, AssetBundle bundle = null) where T : UnityEngine.Object
        {
            if (bundle == null)
            {
                bundle = Bundle;
            }

            CachedAssets.Add(Name, bundle.LoadAsset<T>(Name));
        }
    }
}

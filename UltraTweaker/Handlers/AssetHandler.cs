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

        internal static void LoadBundle()
        {
            Bundle = AssetBundle.LoadFromFile(BundlePath);
        }
    }
}

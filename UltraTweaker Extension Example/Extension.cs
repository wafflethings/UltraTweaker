using BepInEx;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UltraTweaker.Tweaks;
using UltraTweaker.Handlers;
using UltraTweaker;
using System.IO;

namespace Extension
{
    [BepInPlugin(GUID, Name, Version)]
    public class Extension : BaseUnityPlugin
    {
        public const string GUID = "waffle.ultrakill.ut_extension";
        public const string Name = "Extension";
        public const string Version = "1.0.0";

        public static readonly string BundlePath = Path.Combine(PathUtils.ModPath(Assembly.GetExecutingAssembly()), "Assets", "template_assets.bundle");
        public static AssetBundle Assets;

        public void Start()
        {
            Debug.Log($"{Name} has started.");

            // This creates a new page.
            SettingUIHandler.Pages.Add($"{GUID}.ext_page", new SettingUIHandler.Page("EXTENSION: TWEAKS"));

            Assets = AssetBundle.LoadFromFile(BundlePath);

            // This adds this assembly to the ones that get checked for tweaks.
            UltraTweaker.UltraTweaker.AddAssembly(Assembly.GetExecutingAssembly());
        }
    }
}

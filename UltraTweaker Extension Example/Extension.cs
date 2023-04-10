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

namespace Extension
{
    [BepInPlugin(GUID, Name, Version)]
    public class Extension : BaseUnityPlugin
    {
        public const string GUID = "waffle.ultrakill.ut_extension";
        public const string Name = "Extension";
        public const string Version = "1.0.0";

        public void Start()
        {
            Debug.Log($"{Name} has started.");

            SettingUIHandler.Pages.Add($"{GUID}.ext_page", new SettingUIHandler.Page("EXTENSION: TWEAKS"));
            UltraTweaker.UltraTweaker.AddAssembly(Assembly.GetExecutingAssembly());
        }
    }
}

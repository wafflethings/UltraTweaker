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

namespace UltraTweaker
{
    [BepInPlugin(GUID, Name, Version)]
    public class UltraTweaker : BaseUnityPlugin
    {
        public const string GUID = "waffle.ultrakill.ultratweaker";
        public const string Name = "UltraTweaker";
        public const string Version = "1.1.0";
       
        public static Dictionary<Type, Tweak> AllTweaks = new();
        internal static List<Assembly> AssembliesToCheck = new()
        {
            Assembly.GetExecutingAssembly()
        };

        public static void AddAssembly(Assembly asm)
        {
            AssembliesToCheck.Add(asm);
            Tweak.RefreshTweakHolder();
            SaveHandler.LoadData();
        }

        public void Start()
        {
            Debug.Log($"{Name} v{Version} has started.");

            AssetHandler.LoadBundle();
            Tweak.CreateTweakHolder();
            SaveHandler.LoadData();
            SettingUIHandler.Patch();
            MutatorHandler.Patch();
        }

        public void OnDestroy()
        {
            SaveHandler.SaveData();
        }
    }
}

using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace UltraTweaker.Tweaks
{
    /// <summary>
    /// The tweak. Extends from MonoBehaviour, only one is ever made.
    /// </summary>
    public class Tweak : MonoBehaviour
    {
        private static GameObject TweakHolder;
        public static void CreateTweakHolder()
        {
            TweakHolder = new("UT Tweak Holder - don't destroy!");
            DontDestroyOnLoad(TweakHolder);

            foreach (Assembly asm in UltraTweaker.AssembliesToCheck)
            {
                Debug.Log($"Searching {asm.GetName().Name} for tweaks.");

                foreach (Type t in asm.DefinedTypes.Where(type => type.IsDefined(typeof(Metadata), false)))
                {
                    Debug.Log($"Found tweak; {t.Name}.");
                    Tweak tw = (Tweak)TweakHolder.AddComponent(t);
                    tw.enabled = false;
                    UltraTweaker.AllTweaks.Add(t, tw);
                }
            }
        }

        public static void RefreshTweakHolder()
        {
            foreach (Assembly asm in UltraTweaker.AssembliesToCheck)
            {
                Debug.Log($"Searching {asm.GetName().Name} for tweaks.");

                foreach (Type t in asm.DefinedTypes.Where(type => type.IsDefined(typeof(Metadata), false)))
                {
                    if (!UltraTweaker.AllTweaks.ContainsKey(t))
                    {
                        Debug.Log($"Found tweak; {t.Name}.");
                        Tweak tw = (Tweak)TweakHolder.AddComponent(t);
                        tw.enabled = false;
                        UltraTweaker.AllTweaks.Add(t, tw);
                    }
                }
            }
        }

        public Dictionary<string, Subsetting> Subsettings = new();

        /// <summary>
        /// Used to get the instance of the tweak.
        /// </summary>
        public static T GetInstance<T>() where T : Tweak
        {
            return TypeToTweak[typeof(T)] as T;
        }

        public static Dictionary<Type, Tweak> TypeToTweak = new();

        public bool IsEnabled
        {
            get
            {
                return isEnabled;
            }

            set
            {
                if (isEnabled != value)
                {
                    isEnabled = value;

                    if (value == false)
                    {
                        Debug.Log($"Disabling tweak {GetType().Name}.");
                        OnTweakDisabled();
                    }
                    else
                    {
                        Debug.Log($"Enabling tweak {GetType().Name}.");
                        OnTweakEnabled();
                    }
                }
            }
        }

        private bool isEnabled = false;

        /// <summary>
        /// The current UI element for this tweak.
        /// </summary>
        public TweakUIElement Element
        {
            get
            {
                if (element == null)
                {
                    element = new(this);
                }
                return element;
            }
        }

        private TweakUIElement element;

        /// <summary>
        /// Called whenever the tweak is toggled on.
        /// </summary>
        public virtual void OnTweakEnabled()
        {
            SceneManager.sceneLoaded += OnSceneLoad;
            if (!TypeToTweak.ContainsKey(GetType()))
            {
                TypeToTweak.Add(GetType(), this);
            }
            enabled = true;
        }

        /// <summary>
        /// Called whenever the tweak is toggled off.
        /// </summary>
        public virtual void OnTweakDisabled()
        {
            SceneManager.sceneLoaded -= OnSceneLoad;
            enabled = false;
        }

        /// <summary>
        /// Called whenever any subsetting's value is changed.
        /// </summary>
        public virtual void OnSubsettingUpdate()
        {

        }

        public virtual void OnSceneLoad(Scene scene, LoadSceneMode mode)
        {

        }

        /// <summary>
        /// True if the current scene has gameplay.
        /// This means not the Main Menu, any intermission, or 2-S.
        /// </summary>
        public static bool IsGameplayScene()
        {
            string[] NonGameplay =
            {
                "Intro",
                "Bootstrap",
                "Main Menu",
                "Level 2-S",
                "Intermission1",
                "Intermission2"
            };

            return !NonGameplay.Contains(SceneHelper.CurrentScene);
        }

        /// <summary>
        /// Enables or disables all the controls for the tweak.
        /// </summary>
        public void SetControls(bool active)
        {
            element.SetControlsActive(active);

            foreach (Subsetting sub in Subsettings.Values)
            {
                sub.element.SetControlsActive(active);
            }
        }
    }
}

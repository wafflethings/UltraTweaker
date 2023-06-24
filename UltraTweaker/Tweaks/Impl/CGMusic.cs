using HarmonyLib;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace UltraTweaker.Tweaks.Impl
{
    [TweakMetadata("CG Music", $"{UltraTweaker.GUID}.cg_music", "Replace the music for The Cyber Grind.", $"{UltraTweaker.GUID}.cybergrind", 1)]
    public class CGMusic : Tweak
    {
        private Harmony harmony = new($"{UltraTweaker.GUID}.cg_music");
        public static readonly string MusicPath = Path.Combine(PathUtils.ModPath(), "Cybergrind Music");

        public List<AudioClip> Music;
        public List<AudioClip> MusicPool;

        private AudioClip LastClip;
        public AudioSource Source;

        public Coroutine MCL;

        public CGMusic()
        {
            string ModifiedPath = MusicPath;

            if (ModifiedPath.Contains(@"AppData\Roaming"))
            {
                ModifiedPath = ModifiedPath.Substring(ModifiedPath.IndexOf(@"AppData\Roaming") + @"AppData\Roaming".Length);
                ModifiedPath = @"%appdata%\" + ModifiedPath;
            } 
            else if (ModifiedPath.Contains(PathUtils.GameDirectory()))
            {
                ModifiedPath = ModifiedPath.Replace(PathUtils.GameDirectory(), "ULTRAKILL");
            }

            Subsettings = new()
            {
                { "path", new CommentSubsetting(this, new Metadata("Path", "path", ModifiedPath), new CommentSubsettingElement(), OpenFolder, "OPEN") }
            };
        }

        public void OpenFolder()
        {
            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                Arguments = MusicPath,
                FileName = "explorer.exe"
            };

            Process.Start(startInfo);
        }

    public override void OnTweakEnabled()
        {
            base.OnTweakEnabled();
            harmony.PatchAll(typeof(CGMusicPatches));
        }

        public override void OnTweakDisabled()
        {
            base.OnTweakDisabled();
            harmony.UnpatchSelf();
        }

        public override void OnSceneLoad(Scene scene, LoadSceneMode mode)
        {
            if (MCL != null)
            {
                StopCoroutine(MCL);
            }

            if (SceneHelper.CurrentScene == "Endless")
            {
                MusicPool = new();
                Music = GetClipsFromFolder();
                MusicPool.AddRange(Music);
                Source = new GameObject("UltraTweaker: CG Music Manager").AddComponent<AudioSource>();
                Source.outputAudioMixerGroup = MusicManager.Instance.bossTheme.outputAudioMixerGroup;
                StartCoroutine(StartWithTimer());
            }
        }

        public static List<AudioClip> GetClipsFromFolder()
        {
            string[] allFiles = Directory.GetFiles(MusicPath);

            string[] supportedFileExtensions = new string[]
            {
                ".mp3",
                ".ogg",
                ".wav",
                ".aiff",
                ".mod",
                ".it",
                ".s3m",
                ".xm"
            };

            List<AudioClip> clips = new();

            foreach (string file in allFiles)
            {
                foreach (string fileext in supportedFileExtensions)
                {
                    if (file.EndsWith(fileext))
                    {
                        WWW www = new("file:///" + file);
                        while (!www.isDone)
                        {
                        }
                        clips.Add(www.GetAudioClip());
                    }
                }
            }

            return clips;
        }

        public AudioClip RandomClip()
        {
            if (MusicPool.Count == 0)
            {
                MusicPool.AddRange(Music);
            }
            LastClip = MusicPool[UnityEngine.Random.Range(0, MusicPool.Count)];
            MusicPool.Remove(LastClip);
            return LastClip;
        }

        public IEnumerator StartWithTimer()
        {
            while (!StatsManager.Instance.timer)
            {
                yield return null;
            }
            MCL = StartCoroutine(MusicCheckLoop());
        }

        private IEnumerator MusicCheckLoop()
        {
            while (true)
            {
                if (!Source.isPlaying)
                {
                    if (StatsManager.Instance.timer)
                    {
                        Source.PlayOneShot(RandomClip());
                    }
                    else
                    {
                        yield break;
                    }
                    yield return null;
                }
                else
                {
                    yield return null;
                }
            }
        }

        public class CGMusicPatches
        {
            [HarmonyPatch(typeof(CustomMusicPlayer), nameof(CustomMusicPlayer.OnEnable)), HarmonyPrefix]
            private static void DestroyOriginalSong(CustomMusicPlayer __instance)
            {
                if (SceneHelper.CurrentScene == "Endless")
                {
                    __instance.changer.muman.volume = 0;
                    Destroy(__instance.gameObject);
                }
            }
        }
    }
}

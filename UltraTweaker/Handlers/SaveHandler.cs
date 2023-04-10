using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UltraTweaker.Tweaks;
using UnityEngine;

namespace UltraTweaker.Handlers
{
    internal class SaveHandler
    {
        public static readonly string SavePath = Path.Combine(PathUtils.ModPath(), "Data");
        public static readonly string FilePath = Path.Combine(SavePath, "savedata.jkto");
        public static string[] RandomMessages =
        {
            "this mod's source code is awful",
            "mrrp meow nya :3",
            "worship jakito",
            "look at this cat https://youtu.be/RL6-etT59w4",
            "holy shit look at this cat https://youtu.be/4BrQGoEJNaU",
            ":3 https://youtu.be/fUza3qYpw8Q",
            "access denied :3 https://youtu.be/VM5hIhFpbZE",
            "half life interact sound effect :3 https://youtu.be/Ni_33w4KL2Q"
        };

        public static void SaveData()
        {
            string data = $"# {RandomMessages[new System.Random().Next(RandomMessages.Length)]} <{UltraTweaker.Name} {UltraTweaker.Version}>\n\n";
            foreach (Type t in UltraTweaker.AllTweaks.Keys)
            {
                Metadata meta = Attribute.GetCustomAttribute(t, typeof(Metadata)) as Metadata;
                data += $"Tweak: {meta.ID}|{UltraTweaker.AllTweaks[t].IsEnabled}\n";
                foreach (Subsetting sub in UltraTweaker.AllTweaks[t].Subsettings.Values)
                {
                    data += $"Subsetting: {sub.metadata.ID}|{sub.Serialize()}\n";
                }
            }

            if (!Directory.Exists(SavePath))
            {
                Directory.CreateDirectory(SavePath);
            }
            File.WriteAllText(FilePath, data);
        }

        public static void LoadData()
        {
            Tweak currentTweak = null;

            if (File.Exists(FilePath))
            {
                foreach (string line in File.ReadLines(FilePath))
                {
                    if (!line.StartsWith("#"))
                    {
                        if (line.StartsWith("Tweak: "))
                        {
                            string[] split = line.Split('|');
                            string thisId = split[0].Replace("Tweak: ", "");
                            List<Tweak> tweaks = UltraTweaker.AllTweaks.Values.Where(tw => ((Metadata)Attribute.GetCustomAttribute(tw.GetType(), typeof(Metadata))).ID == thisId).ToList();
                            if (tweaks.Count == 1)
                            {
                                currentTweak = UltraTweaker.AllTweaks.Values.Where(tw => ((Metadata)Attribute.GetCustomAttribute(tw.GetType(), typeof(Metadata))).ID == thisId).First();
                                currentTweak.IsEnabled = Convert.ToBoolean(split[1]);
                            }
                        }

                        if (line.StartsWith("Subsetting: ") && currentTweak != null)
                        {
                            string[] split = line.Split('|');
                            string thisId = split[0].Replace("Subsetting: ", "");
                            if (currentTweak.Subsettings.ContainsKey(thisId))
                            {
                                currentTweak.Subsettings[thisId].Deserialize(split[1]);
                            }
                        }
                    }
                }
            }
        }
    }
}

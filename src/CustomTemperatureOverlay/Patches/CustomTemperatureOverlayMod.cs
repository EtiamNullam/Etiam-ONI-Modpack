using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Harmony;
using Newtonsoft.Json;
using UnityEngine;

namespace CustomTemperatureOverlay.Patches
{
    public static class CustomTemperatureOverlayMod
    {
        public const string ModName = "CustomTemperatureOverlay";
        public const string configFileName = "Config.json";

        public static string configDirectoryPath = "Mods" + Path.DirectorySeparatorChar + ModName;

        public static string ConfigFilePath
            => configDirectoryPath + Path.DirectorySeparatorChar + configFileName;

        [HarmonyPatch(typeof(SimDebugView))]
        [HarmonyPatch("OnPrefabInit")]
        public static class SimDebugView_OnPrefabInit
        {
            public static void Postfix()
            {
                SetModRootPath();
                SetWatcher();
                ReloadConfig();
            }

            // TODO: extract to some common library
            private static void SetModRootPath()
            {
                try
                {
                    var directories = Directory.GetDirectories("Mods", ModName, SearchOption.AllDirectories);
                    var modRootPath = directories.FirstOrDefault();

                    if (modRootPath != null)
                    {
                        configDirectoryPath = modRootPath;
                    }
                    else
                    {
                        Debug.Log(ModName + ": Couldn't find mod root path.");
                    }
                }
                catch (Exception e)
                {
                    Debug.Log(ModName + ": Error while searching for mod root path." + Environment.NewLine + e);
                }
            }


            private static void ReloadConfig()
            {
                if (File.Exists(ConfigFilePath))
                {
                    State.Thresholds = JsonConvert.DeserializeObject<SimDebugView.ColorThreshold[]>(File.ReadAllText(ConfigFilePath));
                }

                int stateThresholdsLength = State.Thresholds.Length;
                int requiredThresholdsLength = SimDebugView.Instance.temperatureThresholds.Length;
                object[] logObject = new object[requiredThresholdsLength];

                for (int i = 0; i < requiredThresholdsLength; i++)
                {
                    SimDebugView.Instance.temperatureThresholds[i] = i < stateThresholdsLength
                        ? State.Thresholds[i]
                        : State.Thresholds[stateThresholdsLength - 1];

                    var threshold = SimDebugView.Instance.temperatureThresholds[i];

                    logObject[i] = new
                    {
                        color = new
                        {
                            threshold.color.r,
                            threshold.color.g,
                            threshold.color.b,
                            threshold.color.a
                        },
                        threshold.value
                    };
                }

                Debug.Log(ModName + ": Config loaded: " + Environment.NewLine + JsonConvert.SerializeObject(logObject));
            }

            private static void SetWatcher()
            {
                var watcher = new FileSystemWatcher(configDirectoryPath, "*.json");
                watcher.Changed += (o, e) => ReloadConfig();
                watcher.EnableRaisingEvents = true;
            }
        }
    }
}

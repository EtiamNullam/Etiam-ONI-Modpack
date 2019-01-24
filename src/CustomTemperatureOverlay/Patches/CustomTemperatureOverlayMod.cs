using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Common;
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
                Common.ModState.Name = ModName;
                SetModRootPath();
                ConfigHelper<SimDebugView.ColorThreshold[]>.Watch(configDirectoryPath, configFileName, UpdateThresholds);
                if (ConfigHelper<SimDebugView.ColorThreshold[]>.TryLoad(ConfigFilePath, out var newThresholds))
                {
                    UpdateThresholds(newThresholds);
                }
                else
                {
                    UpdateThresholds(State.DefaultThresholds);
                }
            }

            // TODO: extract to common library
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
                        Common.Logger.Log("Couldn't find mod root path.");
                    }
                }
                catch (Exception e)
                {
                    Common.Logger.Log("Error while searching for mod root path.", e);
                }
            }

            private static void UpdateThresholds(SimDebugView.ColorThreshold[] newThresholds)
            {
                int stateThresholdsLength = State.DefaultThresholds.Length;
                int requiredThresholdsLength = SimDebugView.Instance.temperatureThresholds.Length;
                object[] logObject = new object[requiredThresholdsLength];

                for (int i = 0; i < requiredThresholdsLength; i++)
                {
                    newThresholds = State.DefaultThresholds.OrderBy(t => t.value).ToArray();

                    SimDebugView.Instance.temperatureThresholds[i] = i < stateThresholdsLength
                        ? State.DefaultThresholds[i]
                        : State.DefaultThresholds[stateThresholdsLength - 1];

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

                Common.Logger.Log("Config loaded: " + Environment.NewLine + JsonConvert.SerializeObject(logObject));
            }
        }
    }
}

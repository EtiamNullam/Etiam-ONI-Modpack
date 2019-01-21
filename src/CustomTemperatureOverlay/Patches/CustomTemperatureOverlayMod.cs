using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Common;
using CustomTemperatureOverlay.HSV;
using Harmony;
using Newtonsoft.Json;
using UnityEngine;

namespace CustomTemperatureOverlay.Patches
{
    public static class CustomTemperatureOverlayMod
    {
        // TODO: set keybind to set ranges to currently visible (adaptive overlay)

        public const string ModName = "CustomTemperatureOverlay";
        public const string configFileName = "Config.json";

        public static string configDirectoryPath = "Mods" + Path.DirectorySeparatorChar + ModName;

        public static string ConfigFilePath
            => configDirectoryPath + Path.DirectorySeparatorChar + configFileName;

        [HarmonyPatch(typeof(SimDebugView), "OnPrefabInit")]
        public static class SimDebugView_OnPrefabInit
        {
            public static void Postfix()
            {
                Common.ModState.Name = ModName;
                SetModRootPath();
                ConfigWatcher.SetWatcher(configDirectoryPath, configFileName, (o, e) => ReloadConfig());
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
                        Common.Logger.Log("Couldn't find mod root path.");
                    }
                }
                catch (Exception e)
                {
                    Common.Logger.Log("Error while searching for mod root path." + Environment.NewLine + e);
                }
            }

            private static void ReloadConfig()
            {
                if (File.Exists(ConfigFilePath))
                {
                    State.Thresholds = JsonConvert.DeserializeObject<SimDebugView.ColorThreshold[]>(File.ReadAllText(ConfigFilePath));
                    Common.Logger.Log("Config loaded");//: " + Environment.NewLine + JsonConvert.SerializeObject(State.Thresholds));
                }
                SimDebugView.Instance.temperatureThresholds = State.Thresholds;
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Harmony;
using Newtonsoft.Json;
using UnityEngine;

namespace GasOverlay
{
    public static class GasOverlayMod
    {
        private static Color[] LastColors;
        private static readonly Color NotGasColor = new Color(0.4f, 0.4f, 0.4f);
        private static Config Config = new Config();
        private const string ModName = "GasOverlay";

        [HarmonyPatch(typeof(SplashMessageScreen), "OnSpawn")]
        public static class SplashMessageScreen_OnSpawn
        {
            public const string configFileName = "Config.json";
            public static string configDirectoryPath = "Mods" + Path.DirectorySeparatorChar + "GasOverlay";
            public static string ConfigFilePath => configDirectoryPath + Path.DirectorySeparatorChar + configFileName;

            public static void Postfix()
            {
                try
                {
                    SetModRootPath();
                    SetWatcher();
                }
                catch (Exception e)
                {
                    Debug.Log(ModName + ": Error while starting file watcher: " + e);
                }

                try
                {
                    ReloadConfig();
                }
                catch (Exception e)
                {
                    Debug.Log(ModName + ": Error while loading config: " + e);
                }
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

            private static void OnConfigChange(object sender, EventArgs e)
            {
                ReloadConfig();
            }

            private static void ReloadConfig()
            {
                if (File.Exists(ConfigFilePath))
                {
                    Config = JsonConvert.DeserializeObject<Config>(File.ReadAllText(ConfigFilePath));

                    Config.InterpFactor = Mathf.Clamp(Config.InterpFactor, float.Epsilon, 1);
                    Config.MinimumIntensity = Mathf.Clamp01(Config.MinimumIntensity);

                    Debug.Log(ModName + ": Config loaded: " + Environment.NewLine + JsonConvert.SerializeObject(Config));
                }
            }

            private static void SetWatcher()
            {
                var watcher = new FileSystemWatcher(configDirectoryPath, "*.json");
                watcher.Changed += OnConfigChange;
                watcher.EnableRaisingEvents = true;
            }
        }

        [HarmonyPatch(typeof(SimDebugView), "GetOxygenMapColour")]
        public static class SimDebugView_GetOxygenMapColour
        {
            // TODO: add error handling
            public static bool Prefix(int cell, ref Color __result)
            {
                Element element = Grid.Element[cell];

                if (element.IsGas)
                {
                    if (LastColors == null || cell > LastColors.Length)
                    {
                        ResetLastColors();
                    }

                    float mass = Grid.Mass[cell];
                    float maxMass = Config.MaxMass;

                    float pressureFraction = GetPressureFraction(mass, maxMass);

                    Color color = GetSubstanceColor(element);

                    ScaleToPressure(ref color, pressureFraction);

                    if (Config.ShowEarDrumPopMarker && mass > Config.EarPopMass)
                    {
                        MarkEarDrumPop(ref color, element.id, mass);
                    }

                    TransitColor(ref color, LastColors[cell]);

                    __result = LastColors[cell] = color;
                }
                else
                {
                    __result = NotGasColor;
                }

                return false;
            }

            private static Color32 GetSubstanceColor(Element element)
            {
                var color = element.substance.colour;
                return new Color32(color.r, color.g, color.b, byte.MaxValue);
            }

            private static float GetPressureFraction(float mass, float maxMass)
            {
                float minFraction = Config.MinimumIntensity;
                float fraction = mass / maxMass;

                return Mathf.Lerp(minFraction, 1, fraction);
            }

            private static void ResetLastColors()
            {
                LastColors = new Color[Grid.CellCount];

                for (int i = 0; i < LastColors.Length; i++)
                {
                    LastColors[i] = NotGasColor;
                }
            }

            private static void ScaleToPressure(ref Color color, float fraction)
            {
                color = Color.LerpUnclamped(Color.white, color, fraction);
            }

            private static void MarkEarDrumPop(ref Color color, SimHashes elementID, float mass)
            {
                switch (elementID)
                {
                    case SimHashes.CarbonDioxide:
                    case SimHashes.SourGas:
                        color.r = Mathf.Clamp01(color.r + Config.EarPopChange);
                        break;
                    case SimHashes.Methane:
                    case SimHashes.ContaminatedOxygen:
                        color.g = Mathf.Clamp01(color.g + Config.EarPopChange);
                        break;
                    default:
                        color.g = Mathf.Clamp01(color.g - Config.EarPopChange);
                        break;
                }
            }

            private static void TransitColor(ref Color newColor, Color lastColor)
            {
                newColor = Color.LerpUnclamped(lastColor, newColor, Config.InterpFactor);
            }
        }
    }
}
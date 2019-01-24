using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Common;
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
                Common.ModState.Name = ModName;
                SetModRootPath();
                ConfigHelper<Config>.Watch(configDirectoryPath, configFileName, LoadConfig);
                if (ConfigHelper<Config>.TryLoad(ConfigFilePath, out var newConfig))
                {
                    LoadConfig(newConfig);
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
                        Common.Logger.Log("Couldn't find mod root path.");
                    }
                }
                catch (Exception e)
                {
                    Common.Logger.Log("Error while searching for mod root path." + Environment.NewLine + e);
                }
            }

            private static void LoadConfig(Config config)

            {
                Config = config;
                Common.Logger.Log("Config loaded: " + Environment.NewLine + JsonConvert.SerializeObject(Config));
            }
        }

        [HarmonyPatch(typeof(SimDebugView), "GetOxygenMapColour")]
        public static class SimDebugView_GetOxygenMapColour
        {
            // TODO: add error handling
            public static bool Prefix(int cell, ref Color __result)
            {
                Element element = Grid.Element[cell];

                if (LastColors == null || Grid.CellCount > LastColors.Count())
                {
                    ResetLastColors();
                }

                if (element.IsGas)
                {
                    float mass = Grid.Mass[cell];
                    float maxMass = Config.MaxMass;

                    float pressureFraction = GetPressureFraction(mass, maxMass);

                    Color newGasColor = GetGasColor(element, pressureFraction, mass);

                    TransitColor(ref newGasColor, LastColors[cell]);

                    __result = LastColors[cell] = newGasColor;
                }
                else
                {
                    __result = NotGasColor;
                }

                return false;
            }

            private static Color GetGasColor(Element element, float pressureFraction, float mass)
            {
                Color color = GetCellOverlayColor(element);

                ScaleToPressure(ref color, pressureFraction);

                if (Config.ShowEarDrumPopMarker && mass > Config.EarPopMass)
                {
                    MarkEarDrumPop(ref color, mass);
                }

                return color;
            }

            private static Color GetCellOverlayColor(Element element)
            {
                Color32 overlayColor = element.substance.colour;

                overlayColor.a = byte.MaxValue;

                return overlayColor;
            }

            private static float GetPressureFraction(float mass, float maxMass)
            {
                float minFraction = Config.MinimumIntensity;
                float fraction = mass / maxMass;

                fraction = Mathf.Lerp(minFraction, 1, fraction);

                return fraction;
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

            private static void MarkEarDrumPop(ref Color color, float mass)
            {
                color = new Color
                (
                    GetEarDrumPopValue(color.r),
                    GetEarDrumPopValue(color.g),
                    GetEarDrumPopValue(color.b),
                    color.a
                );
            }

            private static float GetEarDrumPopValue(float colorComponentValue)
            {
                return colorComponentValue > Config.EarPopInversePoint
                    ? colorComponentValue - Config.EarPopChange
                    : colorComponentValue + Config.EarPopChange;
            }

            private static void TransitColor(ref Color newColor, Color lastColor)
            {
                newColor = Color.Lerp(lastColor, newColor, Config.InterpFactor);
            }
        }
    }
}
using System;
using System.IO;
using System.Linq;
using GasOverlay.HSV;
using Harmony;
using Newtonsoft.Json;
using UnityEngine;

namespace GasOverlay
{
    public static class GasOverlayMod
    {
        public static Color[] LastColors;
        private static readonly Color NotGasColor = new Color(0.6f, 0.6f, 0.6f);
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

            private static void ReloadConfig()
            {
                if (File.Exists(ConfigFilePath))
                {
                    Config = JsonConvert.DeserializeObject<Config>(File.ReadAllText(ConfigFilePath));
                    Debug.Log(ModName + ": Config loaded: " + Environment.NewLine + JsonConvert.SerializeObject(Config));
                }
            }

            private static void SetWatcher()
            {
                var watcher = new FileSystemWatcher(configDirectoryPath, "*.json");
                watcher.Changed += (o, e) => ReloadConfig();
                watcher.EnableRaisingEvents = true;
            }
        }

        [HarmonyPatch(typeof(SimDebugView), "GetOxygenMapColour")]
        public static class SimDebugView_GetOxygenMapColour
        {
            public static bool Prefix(int cell, ref Color __result)
            {
                Element element = Grid.Element[cell];

                if (element.IsGas)
                {
                    var newGasColor = GetGasColor(cell, element);

                    try
                    {
                        __result = Color.Lerp(LastColors[cell], newGasColor, Config.InterpFactor);
                        LastColors[cell] = __result;
                    }
                    catch
                    {
                        ResetLastColors();
                    }
                }
                else
                {
                    __result = NotGasColor;
                }

                return false;
            }

            private static void ResetLastColors()
            {
                LastColors = new Color[Grid.CellCount];

                for (int i = 0; i < LastColors.Length; i++)
                {
                    LastColors[i] = NotGasColor;
                }
            }

            private static Color GetGasColor(int cell, Element element)
            {
                SimHashes elementID = element.id;
                Color primaryColor = GetCellOverlayColor(cell, element);
                float mass = Grid.Mass[cell];
                float maxMass = Config.MaxMass;
                float pressureFraction = GetPressureFraction(mass, maxMass);

                ColorHSV colorHSV = primaryColor.ToHSV();

                colorHSV = ScaleColorToPressure(colorHSV, pressureFraction, elementID);
				
                if (Config.ShowEarDrumPopMarker && mass > Config.EarPopMass)
                {
                    colorHSV = MarkEarDrumPopPressure(colorHSV, mass, elementID);
                }

                colorHSV = colorHSV.Clamp();

                return colorHSV.ToRgb();
            }

            private static ColorHSV ScaleColorToPressure(ColorHSV color, float fraction, SimHashes elementID)
            {
                if (elementID == SimHashes.CarbonDioxide)
                {
					color.V *= (1 - fraction) * Config.ValueFactorCarbonDioxide;
				}
                else
                {
                    color.S *= fraction * Config.SaturationFactor;
					color.V -= (1 - fraction) * Config.ValueFactor;
				}

                return color;
            }

            public static Color GetCellOverlayColor(int cellIndex, Element element)
            {
                Substance substance = element.substance;
                Color32 overlayColor = substance.colour;

                overlayColor.a = byte.MaxValue;

                return overlayColor;
            }

            public static float GetPressureFraction(float mass, float maxMass)
            {
                float minFraction = Config.MinimumIntensity;
                float fraction = mass / maxMass;

                fraction = Mathf.Lerp(minFraction, 1, fraction);

                return fraction;
            }

            private static ColorHSV MarkEarDrumPopPressure(ColorHSV color, float mass, SimHashes elementID)
            {
                if (elementID == SimHashes.CarbonDioxide)
                {
                    color.V += 0.3f;
                    color.S += 0.4f;
                }
                else
                {
                    color.H += 0.1f;
                }

                return color;
            }
        }
    }
}
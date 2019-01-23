using System;
using System.Collections.Generic;
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

        private static void ResetLastColors()
        {
            LastColors = new Color[Grid.CellCount];

            for (int i = 0; i < LastColors.Length; i++)
            {
                LastColors[i] = NotGasColor;
            }
        }

        private static ColorHSV ScaleToPressure_Other(ColorHSV hsv, float fraction, float mass)
        {
            hsv = new ColorHSV
            (
                hsv.H,
                hsv.S = Mathf.LerpUnclamped(0, hsv.S, fraction),
                hsv.V = Mathf.LerpUnclamped(0, hsv.V, fraction),
                hsv.A
            );

            if (Config.ShowEarDrumPopMarker && mass > Config.EarPopMass)
            {
                hsv = MarkEarDrumPopPressure_Other(hsv, mass);
            }

            return hsv;
        }

        private static ColorHSV ScaleToPressure_CO2(ColorHSV hsv, float fraction, float mass)
        {
            hsv.V = Mathf.LerpUnclamped(hsv.V, 0, fraction);

            if (Config.ShowEarDrumPopMarker && mass > Config.EarPopMass)
            {
                hsv = MarkEarDrumPopPressure_CO2(hsv, mass);
            }

            return hsv;
        }

        private static ColorHSV MarkEarDrumPopPressure_CO2(ColorHSV color, float mass)
        {
            color.V += 0.3f;
            color.S += 0.4f;

            return color;
        }

        private static ColorHSV MarkEarDrumPopPressure_Other(ColorHSV color, float mass)
        {
            color.H += 0.1f;

            return color;
        }

        private static Color ProcessColor(Color newColor, int cell)
        {
            try
            {
                // TODO: lerp over HSV?
                var result = Color.Lerp(LastColors[cell], newColor, Config.InterpFactor);
                LastColors[cell] = result;
                return result;
            }
            catch
            {
                ResetLastColors();
                return NotGasColor;
            }
        }

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
            public static bool Prefix(int cell, ref Color __result)
            {
                Element element = Grid.Element[cell];

                if (element.IsGas)
                {
                    var newGasColor = GetGasColor(cell, element);
                    __result = ProcessColor(newGasColor, cell);
                }
                else
                {
                    // TODO: reset lastcolor at cell?
                    __result = NotGasColor;
                }

                return false;
            }

            private static Color GetGasColor(int cell, Element element)
            {
                float mass = Grid.Mass[cell];
                float maxMass = Config.MaxMass;

                float pressureFraction = GetPressureFraction(mass, maxMass);

                Color primaryColor = GetCellOverlayColor(element);
                ColorHSV hsv = primaryColor.ToHSV();

                SimHashes elementID = element.id;

                if (elementID == SimHashes.CarbonDioxide)
                {
                    hsv = ScaleToPressure_CO2(hsv, pressureFraction, mass);
                }
                else
                {
                    hsv = ScaleToPressure_Other(hsv, pressureFraction, mass);
                }

                hsv = CheapClamp(hsv);

                return hsv.ToRgb();
            }

            private static ColorHSV CheapClamp(ColorHSV hsv)
            {
                return new ColorHSV
                (
                    hsv.H > 1 ? hsv.H - 1 : hsv.H,
                    Mathf.Clamp01(hsv.S),
                    Mathf.Clamp01(hsv.V),
                    hsv.A
                );
            }

            public static Color GetCellOverlayColor(Element element)
            {
                Color32 overlayColor = element.substance.colour;

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
        }
    }
}
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
        private static Color?[] LastColors;
        private const string ConfigFileName = "Config.json";

        private static void ResetLastColors()
        {
            LastColors = new Color?[Grid.CellCount];
        }

        public static void OnLoad()
        {
            if (State.Common.ConfigPath == null)
            {
                return;
            }

            State.Common.WatchConfig<Config>(ConfigFileName, LoadConfig);

            try
            {
                LoadConfig(State.Common.LoadConfig<Config>(ConfigFileName));
            }
            catch (Exception e)
            {
                State.Common.Logger.Log("Error while loading config." + e);
            }
        }

        private static void LoadConfig(Config config)
        {
            State.Config = config;

            State.Config.InterpFactor = Mathf.Clamp(State.Config.InterpFactor, float.Epsilon, 1);
            State.Config.MinimumIntensity = Mathf.Clamp01(State.Config.MinimumIntensity);

            ResetLastColors();

            State.Common.Logger.Log("Config loaded.");
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
                    float maxMass = State.Config.MaxMass;

                    float pressureFraction = GetPressureFraction(mass, maxMass);

                    Color color = GetSubstanceColor(element);

                    ScaleToPressure(ref color, pressureFraction);

                    if (State.Config.ShowEarDrumPopMarker && mass > State.Config.EarPopMass)
                    {
                        MarkEarDrumPop(ref color, element.id, mass);
                    }

                    Color? lastColor = LastColors[cell];

                    if (lastColor.HasValue)
                    {
                        TransitColor(ref color, lastColor.Value);
                    }

                    LastColors[cell] = color;
                    __result = color;
                }
                else
                {
                    __result = State.Config.NotGasColor;
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
                float minFraction = State.Config.MinimumIntensity;
                float fraction = mass / maxMass;

                float lerped = Mathf.Lerp(minFraction, 1, fraction);

                float exponentRootOf = (float)Math.Pow(lerped, State.Config.ExponentRootOf);

                return exponentRootOf;
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
                        color.r = Mathf.Clamp01(color.r + State.Config.EarPopChange);
                        break;
                    case SimHashes.Methane:
                    case SimHashes.ContaminatedOxygen:
                        color.g = Mathf.Clamp01(color.g + State.Config.EarPopChange);
                        break;
                    default:
                        color.g = Mathf.Clamp01(color.g - State.Config.EarPopChange);
                        break;
                }
            }

            private static void TransitColor(ref Color newColor, Color lastColor)
            {
                newColor = Color.LerpUnclamped(lastColor, newColor, State.Config.InterpFactor);
            }
        }
    }
}
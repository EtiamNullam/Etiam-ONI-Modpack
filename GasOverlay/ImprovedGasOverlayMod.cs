using System;
using Harmony;
using UnityEngine;

namespace ImprovedGasColourMod
{
    public static class HarmonyPatches
    {
        public static ColorHSV?[] LastColors;

        private static readonly Color NotGasColor = new Color(0.6f, 0.6f, 0.6f);

        [HarmonyPatch(typeof(SimDebugView), "GetOxygenMapColour")]
        public static class ImprovedGasOverlayMod
        {
            public const float EarPopFloat = 5;

            public static bool Prefix(int cell, ref Color __result)
            {
                float maxMass = ImprovedGasOverlayConfig.GasPressureEnd;

                Element element = Grid.Element[cell];

                ColorHSV newGasColor;

                if (!element.IsGas)
                {
                    newGasColor = NotGasColor;
                }
                else
                {
                    float mass = Grid.Mass[cell];
                    SimHashes elementID = element.id;
                    Color primaryColor = GetCellOverlayColor(cell);
                    float pressureFraction = GetPressureFraction(mass, maxMass);

                    newGasColor = GetGasColor(elementID, primaryColor, pressureFraction, mass);
                }

                if (LastColors == null)
                {
                    ResetLastColors();
                }

                try
                {
                    if (LastColors[cell].HasValue)
                    {
                        LastColors[cell] = __result = TransitToNewColor(LastColors[cell].Value, newGasColor);
                    }
                    else
                    {
                        LastColors[cell] = __result = newGasColor;
                    }
                }
                catch (ArgumentOutOfRangeException)
                {
                    ResetLastColors();
                }

                return false;
            }

            private static void ResetLastColors()
            {
                LastColors = new ColorHSV?[Grid.CellCount];
            }

            private static ColorHSV GetGasColor(SimHashes elementID, Color primaryColor, float pressureFraction, float mass)
            {
                ColorHSV colorHSV = primaryColor.ToHSV();

                colorHSV = ScaleColorToPressure(colorHSV, pressureFraction, elementID);
				
                if (ImprovedGasOverlayConfig.ShowEarDrumPopMarker)
                {
                    colorHSV = MarkEarDrumPopPressure(colorHSV, mass, elementID);
                }

                colorHSV = colorHSV.Clamp();

                return colorHSV;
            }

            private static ColorHSV TransitToNewColor(ColorHSV oldColor, ColorHSV targetColor)
            {
                var step = 0.025f;

                return new ColorHSV
                (
                    Mathf.Lerp(oldColor.H, targetColor.H, step),
                    Mathf.Lerp(oldColor.S, targetColor.S, step),
                    Mathf.Lerp(oldColor.V, targetColor.V, step),
                    Mathf.Lerp(oldColor.A, targetColor.A, step)
                );
            }

            private static ColorHSV ScaleColorToPressure(ColorHSV color, float fraction, SimHashes elementID)
            {
                if (elementID == SimHashes.CarbonDioxide)
                {
					color.V *= (1 - fraction) * ImprovedGasOverlayConfig.FactorValueHSVCarbonDioxide;
				}
                else
                {
                    color.S *= fraction * 1.25f;
					color.V -= (1 - fraction) * ImprovedGasOverlayConfig.FactorValueHSVGases;
				}

                return color;
            }

            public static Color GetCellOverlayColor(int cellIndex)
            {
                Element element = Grid.Element[cellIndex];
                Substance substance = element.substance;

                Color32 overlayColor = substance.conduitColour;

                overlayColor.a = byte.MaxValue;

                return overlayColor;
            }

            private static float GetPressureFraction(float mass, float maxMass)
            {
                float minFraction = ImprovedGasOverlayConfig.MinimumGasColorIntensity;
                float fraction = mass / maxMass;

                fraction = Mathf.Lerp(minFraction, 1, fraction);

                return fraction;
            }

            private static ColorHSV MarkEarDrumPopPressure(ColorHSV color, float mass, SimHashes elementID)
            {
                if (mass > EarPopFloat)
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
                }

                return color;
            }
        }
    }
}
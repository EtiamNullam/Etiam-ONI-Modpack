using System;
using System.Collections.Generic;
using HarmonyLib;
using UnityEngine;

namespace CustomTemperatureOverlay
{
    public class Patches : KMod.UserMod2
    {
        public override void OnLoad(Harmony harmony)
        {
            base.OnLoad(harmony);

            Mod.Config.Load();
            Mod.Config.Watch();
        }

        [HarmonyPatch(typeof(SimDebugView))]
        [HarmonyPatch("NormalizedTemperature")]
        public static class SimDebugView_NormalizedTemperature
        {
            public static bool Prefix(float actualTemperature, ref Color __result)
            {
                try
                {
                    if (Game.Instance.temperatureOverlayMode != Game.TemperatureOverlayModes.AbsoluteTemperature)
                    {
                        return true;
                    }

                    var firstStep = State.Steps[0];

                    if (actualTemperature <= firstStep.value)
                    {
                        __result = firstStep.color;

                        return false;
                    }

                    var lastStep = State.Steps[State.Steps.Length - 1];

                    if (actualTemperature >= lastStep.value)
                    {
                        __result = lastStep.color;

                        return false;
                    }

                    FindStepsAround(actualTemperature, out var previousStep, out var nextStep);

                    var previousToActualTemperatureDelta = actualTemperature - previousStep.value;
                    var previousToNextDelta = nextStep.value - previousStep.value;

                    var factor = previousToActualTemperatureDelta / previousToNextDelta;

                    __result = Color.Lerp(
                        previousStep.color,
                        nextStep.color,
                        factor
                    );

                    return false;
                }
                catch (Exception e)
                {
                    State.Common.Logger.LogOnce("Failed to calculate normalized temperature", e);
                }

                return true;
            }

            private static void FindStepsAround(float temperature, out Data.TemperatureStep previousStep, out Data.TemperatureStep nextStep)
            {
                var stepsCount = State.Steps.Length;
                var firstStep = State.Steps[0];
                previousStep = firstStep;

                var currentStep = 1;

                while (currentStep < stepsCount - 1)
                {
                    var currentTemperatureStep = State.Steps[currentStep];

                    if (currentTemperatureStep.value > temperature)
                    {
                        break;
                    }

                    previousStep = currentTemperatureStep;
                    currentStep++;
                }

                nextStep = State.Steps[currentStep];
            }
        }

        [HarmonyPatch(typeof(OverlayModes.Temperature))]
        [HarmonyPatch("RefreshLegendValues")]
        public static class OverlayModes_RefreshLegendValues
        {
            public static void Postfix(List<LegendEntry> ___temperatureLegend)
            {
                try
                {
                    var stepsLength = State.Steps.Length;
                    var legendLength = 8;

                    for (int i = 0; i < legendLength; i++)
                    {
                        var step = stepsLength > i
                            ? State.Steps[i]
                            : State.Steps[stepsLength - 1];

                        var formattedTemperature = GameUtil.GetFormattedTemperature(
                            step.value,
                            GameUtil.TimeSlice.None,
                            GameUtil.TemperatureInterpretation.Absolute,
                            true,
                            false
                        );

                        ___temperatureLegend[legendLength - i - 1].colour = step.color;
                        ___temperatureLegend[legendLength - i - 1].desc_arg = formattedTemperature;
                    }
                }
                catch (Exception e)
                {
                    State.Common.Logger.LogOnce("Failed to update temperature legend", e);
                }
            }
        }
    }
}

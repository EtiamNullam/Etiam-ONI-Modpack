using System;
using System.Collections.Generic;
using CustomTemperatureOverlay.Data;
using UnityEngine;

namespace CustomTemperatureOverlay
{
    public static class Mod
    {
        public static Color GetTemperatureColor(float temperature)
        {
            var firstStep = State.Steps[0];

            if (temperature <= firstStep.value)
            {
                return firstStep.color;
            }

            var lastStep = State.Steps[State.Steps.Length - 1];

            if (temperature >= lastStep.value)
            {
                return lastStep.color;
            }

            FindStepsAround(temperature, out var previousStep, out var nextStep);

            var previousToActualTemperatureDelta = temperature - previousStep.value;
            var previousToNextDelta = nextStep.value - previousStep.value;

            var factor = previousToActualTemperatureDelta / previousToNextDelta;

            var temperatureColor = Color.Lerp
            (
                previousStep.color,
                nextStep.color,
                factor
            );

            return temperatureColor;
        }

        public static void UpdateTemperatureLegend(ref List<LegendEntry> temperatureLegendEntries)
        {
            var stepsCount = State.Steps.Length;
            var entriesCountToModify = 8;

            for (int i = 0; i < entriesCountToModify; i++)
            {
                var step = stepsCount > i
                    ? State.Steps[i]
                    : State.Steps[stepsCount - 1];

                var formattedTemperature = GameUtil.GetFormattedTemperature
                (
                    step.value,
                    GameUtil.TimeSlice.None,
                    GameUtil.TemperatureInterpretation.Absolute,
                    true,
                    false
                );

                var entry = temperatureLegendEntries[entriesCountToModify - i - 1];

                entry.colour = step.color;
                entry.desc_arg = formattedTemperature;
            }
        }

        private static void FindStepsAround(float temperature, out TemperatureStep previousStep, out TemperatureStep nextStep)
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

        public static class Config
        {
            public static void Watch()
            {
                try
                {
                    State.Common.WatchConfig<TemperatureStep[]>(State.ConfigFileName, Reload);
                }
                catch (Exception e)
                {
                    State.Common.Logger.Log("Failed to start config watch", e);
                }
            }

            public static void Reload(TemperatureStep[] steps)
            {
                try
                {
                    Config.Load(steps);

                    State.Common.Logger.Log("Config changed and reloaded");
                }
                catch (Exception e)
                {
                    State.Common.Logger.Log("Failed to reload config", e);
                }
            }

            public static bool Load(TemperatureStep[] steps = null)
            {
                try
                {
                    if (steps == null)
                    {
                        steps = State.Common.LoadConfig<TemperatureStep[]>(State.ConfigFileName);
                    }

                    if (!IsValid(steps))
                    {
                        return false;
                    }

                    Config.SortByTemperature(steps);
                    State.Steps = steps;

                    return true;
                }
                catch (Exception e)
                {
                    State.Common.Logger.Log("Failed to load config", e);

                    return false;
                }
            }

            private static bool IsValid(TemperatureStep[] steps)
            {
                if (steps == null || steps.Length == 0)
                {
                    State.Common.Logger.Log("Config is invalid");

                    return false;
                }

                return true;
            }

            private static void SortByTemperature(TemperatureStep[] steps)
            {
                Array.Sort
                (
                    steps,
                    (step1, step2) => step1.value.CompareTo(step2.value)
                );
            }
        }
    }
}

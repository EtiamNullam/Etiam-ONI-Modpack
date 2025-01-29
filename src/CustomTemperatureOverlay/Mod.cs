using CustomTemperatureOverlay.Data;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace CustomTemperatureOverlay
{
    public static class Mod
    {

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

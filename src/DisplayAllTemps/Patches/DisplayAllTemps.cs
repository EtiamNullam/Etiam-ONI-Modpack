using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Harmony;
using UnityEngine;

namespace DisplayAllTemps.Patches
{
    [HarmonyPatch(typeof(GameUtil))]
    [HarmonyPatch(nameof(GameUtil.GetFormattedTemperature))]
    public static class DisplayAllTemps
    {
        public static void Postfix(float temp, ref string __result, GameUtil.TemperatureInterpretation interpretation, GameUtil.TimeSlice timeSlice)
        {
            try
            {
                if (interpretation != GameUtil.TemperatureInterpretation.Absolute || timeSlice != GameUtil.TimeSlice.None)
                {
                    return;
                }

                string formatString = "##0.#";
                float kelvin = GameUtil.GetTemperatureConvertedToKelvin(temp);

                string kelvinString = kelvin.ToString(formatString);
                string celsiusString = GameUtil.GetTemperatureConvertedFromKelvin(kelvin, GameUtil.TemperatureUnit.Celsius).ToString(formatString);
                string fahrenheitString = GameUtil.GetTemperatureConvertedFromKelvin(kelvin, GameUtil.TemperatureUnit.Fahrenheit).ToString(formatString);

                string first;
                string second;

                switch (GameUtil.temperatureUnit)
                {
                    case GameUtil.TemperatureUnit.Celsius:
                        first = kelvinString + STRINGS.UI.UNITSUFFIXES.TEMPERATURE.KELVIN;
                        second = fahrenheitString + STRINGS.UI.UNITSUFFIXES.TEMPERATURE.FAHRENHEIT;
                        break;
                    case GameUtil.TemperatureUnit.Fahrenheit:
                        first = kelvinString + STRINGS.UI.UNITSUFFIXES.TEMPERATURE.KELVIN;
                        second = celsiusString + STRINGS.UI.UNITSUFFIXES.TEMPERATURE.CELSIUS;
                        break;
                    case GameUtil.TemperatureUnit.Kelvin:
                        first = celsiusString + STRINGS.UI.UNITSUFFIXES.TEMPERATURE.CELSIUS;
                        second = fahrenheitString + STRINGS.UI.UNITSUFFIXES.TEMPERATURE.FAHRENHEIT;
                        break;
                    default:
                        return;
                }

                __result += $", ({first}, {second})";
            }
            catch (Exception e)
            {
                // TODO: log once instead
                Debug.Log("DisplayAllTemps: " + e);
            }
        }
    }
}

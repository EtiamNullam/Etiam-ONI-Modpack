using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Harmony;
using Newtonsoft.Json;
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

                var temperatures = new List<string>();

                if (GameUtil.temperatureUnit != GameUtil.TemperatureUnit.Celsius && (TemperatureUnitMultiple.Celsius & State.Unit) != 0)
                {
                    string celsiusString = GameUtil.GetTemperatureConvertedFromKelvin(kelvin, GameUtil.TemperatureUnit.Celsius).ToString(formatString) + STRINGS.UI.UNITSUFFIXES.TEMPERATURE.CELSIUS;

                    temperatures.Add(celsiusString);
                }
                
                if (GameUtil.temperatureUnit != GameUtil.TemperatureUnit.Fahrenheit && (TemperatureUnitMultiple.Fahrenheit & State.Unit) != 0)
                {
                    string fahrenheitString = GameUtil.GetTemperatureConvertedFromKelvin(kelvin, GameUtil.TemperatureUnit.Fahrenheit).ToString(formatString) + STRINGS.UI.UNITSUFFIXES.TEMPERATURE.FAHRENHEIT;

                    temperatures.Add(fahrenheitString);
                }

                if (GameUtil.temperatureUnit != GameUtil.TemperatureUnit.Kelvin && (TemperatureUnitMultiple.Kelvin & State.Unit) != 0)
                {
                    string kelvinString = kelvin.ToString(formatString) + STRINGS.UI.UNITSUFFIXES.TEMPERATURE.KELVIN;

                    temperatures.Add(kelvinString);
                }

                if (temperatures.Count <= 0)
                {
                    return;
                }

                var builder = new StringBuilder(__result);

                foreach (var temperature in temperatures)
                {
                    builder.Append(", ");
                    builder.Append(temperature);
                }

                __result = builder.ToString();
            }
            catch (Exception e)
            {
                State.Common.Logger.LogOnce("DisplayAllTemps failed.", e);
            }
        }
    }
}

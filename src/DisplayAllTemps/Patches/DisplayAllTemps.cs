using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DisplayAllTemps.Helpers;
using Harmony;
using Newtonsoft.Json;
using UnityEngine;

namespace DisplayAllTemps.Patches
{
    [HarmonyPatch(typeof(GameUtil))]
    [HarmonyPatch(nameof(GameUtil.GetFormattedTemperature))]
    public static class DisplayAllTemps
    {
        private const string separator = ", ";

        public static void Postfix(float temp, bool displayUnits, ref string __result, GameUtil.TemperatureInterpretation interpretation, GameUtil.TimeSlice timeSlice)
        {
            try
            {
                if (interpretation != GameUtil.TemperatureInterpretation.Absolute || timeSlice != GameUtil.TimeSlice.None || !displayUnits)
                {
                    return;
                }

                var format = "##0.#";

                var kelvinTemperature = GameUtil.GetTemperatureConvertedToKelvin(temp);

                var temperaturesWithSymbol = TemperatureHelper.GetTemperatureUnits()
                    .Where(ShouldDisplay)
                    .Select(temperatureUnit => ConvertAndAddSymbol(kelvinTemperature, temperatureUnit, format));

                __result = BuildResult(__result, temperaturesWithSymbol);
            }
            catch (Exception e)
            {
                State.Common.Logger.LogOnce("DisplayAllTemps failed.", e);
            }
        }

        private static string ConvertAndAddSymbol(float kelvinTemperature, GameUtil.TemperatureUnit temperatureUnit, string temperatureFormat)
        {
            var convertedTemperature = GameUtil.GetTemperatureConvertedFromKelvin(kelvinTemperature, temperatureUnit);
            return TemperatureHelper.GetTemperatureWithSymbol(convertedTemperature, temperatureUnit, temperatureFormat);
        }

        private static bool ShouldDisplay(GameUtil.TemperatureUnit temperatureUnit)
        {
            return GameUtil.temperatureUnit != temperatureUnit 
                && (temperatureUnit.ToMultiple() & State.Unit) != 0;
        }

        private static string BuildResult(string result, IEnumerable<string> temperaturesWithSymbol)
        {
            var stringBuilder = new StringBuilder(result);
            foreach (var temperatureWithSymbol in temperaturesWithSymbol)
            {
                stringBuilder.Append(separator);
                stringBuilder.Append(temperatureWithSymbol);
            }
           return stringBuilder.ToString();
        }
    }
}
















/*
 

                var stringBuilder = new StringBuilder(__result);

                var enabledTemperatureUnits = TemperatureHelper.GetTemperatureUnits().Where(IsEnabled);
                foreach (GameUtil.TemperatureUnit temperatureUnit in enabledTemperatureUnits)
                {
                    var convertedTemperature = GameUtil.GetTemperatureConvertedFromKelvin(kelvinTemperature, temperatureUnit);
                    var temperatureWithSymbol = TemperatureHelper.GetTemperatureWithSymbol(convertedTemperature, temperatureUnit, format);

                    stringBuilder.Append(", ");
                    stringBuilder.Append(temperatureWithSymbol);
                }

                __result = stringBuilder.ToString();
*/
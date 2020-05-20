using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DisplayAllTemps.Helpers
{
    public static class TemperatureHelper
    {
        public static GameUtil.TemperatureUnit[] GetTemperatureUnits()
        {
            return (GameUtil.TemperatureUnit[])Enum.GetValues(typeof(GameUtil.TemperatureUnit));
        }

        public static string GetTemperatureWithSymbol(float temperature, GameUtil.TemperatureUnit temperatureUnit, string temperatureFormat)
        {
            return temperature.ToString(temperatureFormat) + temperatureUnit.GetSymbol();
        }

       public  static GameUtil.TemperatureUnit ToTemperatureUnit(this TemperatureUnitMultiple multiple)
        {
            return (State.Unit & TemperatureUnitMultiple.Celsius) != 0
               ? GameUtil.TemperatureUnit.Celsius
               : (State.Unit & TemperatureUnitMultiple.Fahrenheit) != 0
                   ? GameUtil.TemperatureUnit.Fahrenheit
                   : GameUtil.TemperatureUnit.Kelvin;
        }

        public static TemperatureUnitMultiple ToMultiple(this GameUtil.TemperatureUnit temperatureUnit)
        {
            switch (temperatureUnit)
            {
                case GameUtil.TemperatureUnit.Celsius:
                    return TemperatureUnitMultiple.Celsius;
                case GameUtil.TemperatureUnit.Kelvin:
                    return TemperatureUnitMultiple.Kelvin;
                case GameUtil.TemperatureUnit.Fahrenheit:
                    return TemperatureUnitMultiple.Fahrenheit;
                default:
                    State.Common.Logger.LogOnce("TemperatureHelper.ToMultiple: invalid unit");
                    return TemperatureUnitMultiple.All;
            }
        }

        public static string GetSymbol(this GameUtil.TemperatureUnit temperatureUnit)
        {
            switch (temperatureUnit)
            {
                case GameUtil.TemperatureUnit.Celsius:
                    return STRINGS.UI.UNITSUFFIXES.TEMPERATURE.CELSIUS;
                case GameUtil.TemperatureUnit.Kelvin:
                    return STRINGS.UI.UNITSUFFIXES.TEMPERATURE.KELVIN;
                case GameUtil.TemperatureUnit.Fahrenheit:
                    return STRINGS.UI.UNITSUFFIXES.TEMPERATURE.FAHRENHEIT;
                default:
                    State.Common.Logger.LogOnce("TemperatureHelper.GetSymbol: invalid unit");
                    return "Unknown";
            }
        }

        public static void Flip(this TemperatureUnitMultiple unit)
        {
            try
            {
                if (State.Unit == unit)
                {
                    return;
                }

                State.Unit ^= unit;
            }
            catch (Exception e)
            {
                State.Common.Logger.Log("Flip unit failed.", e);
            }

            return;
        }
    }
}

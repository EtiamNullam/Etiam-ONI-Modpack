using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DisplayAllTemps
{
    public static class State
    {
        public static Core Common = new Core("DisplayAllTemps", "1737903327", null, false);

        public static TemperatureUnitMultiple Unit = TemperatureUnitMultiple.All;

        public static GameUtil.TemperatureUnit LastMainUnit = GameUtil.temperatureUnit;

        public static readonly string ConfigFileName = "Unit.json";
    }
}

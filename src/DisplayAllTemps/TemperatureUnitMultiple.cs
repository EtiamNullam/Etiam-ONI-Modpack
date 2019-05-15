using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DisplayAllTemps
{
    [Flags]
    public enum TemperatureUnitMultiple
    {
        None = 0,

        Celsius = 1,
        Fahrenheit = 2,
        Kelvin = 4,

        All = Celsius | Fahrenheit | Kelvin
    }
}

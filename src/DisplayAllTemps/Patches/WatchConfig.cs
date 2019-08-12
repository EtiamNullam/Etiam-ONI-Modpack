using Harmony;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace DisplayAllTemps.Patches
{
    public static class GameLaunch
    {
        public static void OnLoad()
        {
            try
            {
                if (State.Common.ConfigPath == null)
                {
                    return;
                }

                if (State.Common.ConfigExists(State.ConfigFileName))
                {
                    State.Unit = State.Common.LoadConfig<TemperatureUnitMultiple>(State.ConfigFileName);
                }
                else
                {
                    State.Common.SaveConfig(State.ConfigFileName, TemperatureUnitMultiple.All);
                }

                State.Common.WatchConfig<TemperatureUnitMultiple>(State.ConfigFileName, (unit) => { State.Unit = unit; });
            }
            catch (Exception e)
            {
                State.Common.Logger.Log("Failed to start config watch.", e);
            }
        }
    }
}

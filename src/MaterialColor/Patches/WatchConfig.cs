using Common;
using Harmony;
using MaterialColor.Data;
using MaterialColor.IO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace MaterialColor.Patches
{
    public static class WatchConfig
    {
        private static PeriodicRefresher Refresher;

        [HarmonyPatch(typeof(SplashMessageScreen), "OnSpawn")]
        public static class GameLaunch
        {
            private static readonly string MainConfigFilename = "Config.json";
            private static readonly string ElementColorsFilename = "ElementColors.json";

            public static void Postfix()
            {
                if (State.Common.ConfigPath == null)
                {
                    return;
                }

                State.Common.WatchConfig<Config>(MainConfigFilename, LoadMainConfig);
                State.Common.WatchConfig<Dictionary<SimHashes, ElementColor>>(ElementColorsFilename, LoadElementColors);

                try
                {
                    LoadMainConfig(State.Common.LoadConfig<Config>(MainConfigFilename));
                }
                catch (Exception e)
                {
                    State.Common.Logger.Log("Error while loading config." + e);
                }

                try
                {
                    LoadElementColors(State.Common.LoadConfig<Dictionary<SimHashes, ElementColor>>(ElementColorsFilename));
                }
                catch (Exception e)
                {
                    State.Common.Logger.Log("Error while loading config." + e);
                }
            }

            private static void LoadMainConfig(Config newConfig)
            {
                State.Config = newConfig;
                State.ConfigChanged = true;
            }

            private static void LoadElementColors(Dictionary<SimHashes, ElementColor> newElementColors)
            {
                State.ElementColors = newElementColors;
                State.ConfigChanged = true;
            }
        }

        [HarmonyPatch(typeof(Game), "OnSpawn")]
        public static class Game_OnSpawn
        {
            public static void Postfix()
            {
                Refresher = new PeriodicRefresher();
                SimAndRenderScheduler.instance.render1000ms.Add(Refresher);
            }
        }

        [HarmonyPatch(typeof(Game), "DestroyInstances")]
        public static class Game_DestroyInstances
        {
            public static void Postfix()
            {
                try
                {
                    SimAndRenderScheduler.instance.render1000ms.Remove(Refresher);
                    Refresher = null;
                }
                catch (Exception e)
                {
                    State.Common.Logger.LogOnce("Game_DestroyInstances.Postfix failed", e);
                }
            }
        }
    }
}

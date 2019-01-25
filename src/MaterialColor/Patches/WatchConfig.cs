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
            public static void Postfix()
            {
                Common.ModState.Initialize("MaterialColor", "Config");
                
                string mainConfigFilename = "Config.json";
                string elementColorsFilename = "ElementColors.json";

                Common.ConfigHelper<Config>.Watch(mainConfigFilename, LoadMainConfig);
                Common.ConfigHelper<Dictionary<SimHashes, ElementColor>>.Watch(elementColorsFilename, LoadElementColors);

                if (Common.ConfigHelper<Config>.TryLoad(mainConfigFilename, out var newConfig))
                {
                    LoadMainConfig(newConfig);
                }

                if (Common.ConfigHelper<Dictionary<SimHashes, ElementColor>>.TryLoad(elementColorsFilename, out var newElementColors))
                {
                    LoadElementColors(newElementColors);
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
                    Common.Logger.LogOnce("Game_DestroyInstances.Postfix failed", e);
                }
            }
        }
    }
}

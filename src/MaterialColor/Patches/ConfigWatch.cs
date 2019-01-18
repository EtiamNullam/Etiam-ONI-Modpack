using Harmony;
using MaterialColor.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MaterialColor.Patches
{
    public static class ConfigWatch
    {
        private static ConfigWatcher Watcher;

        [HarmonyPatch(typeof(SplashMessageScreen), "OnSpawn")]
        public static class GameLaunch
        {
            public static void Postfix()
            {
                TryStartConfigWatch();
                TryLoadConfig();
            }

            private static void TryLoadConfig()
            {
                try
                {
                    State.LoadMainConfig();
                    State.LoadElementColors();
                }
                catch (Exception e)
                {
                    Logger.Log(e);
                }
            }

            private static void TryStartConfigWatch()
            {
                try
                {
                    Watcher = new ConfigWatcher();
                    SimAndRenderScheduler.instance.render1000ms.Add(Watcher);
                }
                catch (Exception e)
                {
                    Logger.Log(e);
                }
            }
        }

        [HarmonyPatch(typeof(Game), "DestroyInstances")]
        public static class Game_DestroyInstances
        {
            public static void Postfix()
            {
                try
                {
                    SimAndRenderScheduler.instance.render1000ms.Remove(Watcher);
                    Watcher.Dispose();
                    Watcher = null;
                }
                catch (Exception e)
                {
                    Logger.LogOnce("Game_DestroyInstances.Postfix failed", e);
                }
            }
        }
    }
}

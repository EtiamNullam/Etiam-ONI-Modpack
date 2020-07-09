using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Common;
using Harmony;
using Newtonsoft.Json;
using UnityEngine;

namespace CustomTemperatureOverlay
{
    public static class Patches
    {
        public static void OnLoad()
        {
            Mod.Config.Load();
            Mod.Config.Watch();
        }

        [HarmonyPatch(typeof(SimDebugView))]
        [HarmonyPatch("OnPrefabInit")]
        public static class GlobalAssets_OnPrefabInit
        {
            public static void Postfix()
            {
                Mod.UpdateColorSet();
            }
        }
    }
}

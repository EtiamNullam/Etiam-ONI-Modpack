using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Harmony;

namespace CustomTemperatureOverlay
{
    public static class CustomTemperatureOverlayMod
    {
        // TODO: set keybind to set ranges to currently visible

        // TODO: switch color interpolation to HSV
        // RGB interpolation is very bad

        [HarmonyPatch(typeof(SimDebugView), "OnPrefabInit")]
        public static class SimDebugView_OnPrefabInit
        {
            public static void Postfix(SimDebugView __instance)
            {
                __instance.temperatureThresholds = new SimDebugView.ColorThreshold[]
                // TODO: whiten some colors
                {
                    new SimDebugView.ColorThreshold // Exact Absolute Zero
                    {
                        color = new UnityEngine.Color(1,1,1,1),
                        value = 0
                    },
                    new SimDebugView.ColorThreshold // Near Absolute Zero
                    {
                        color = new UnityEngine.Color(0.7f,0,1,1),
                        value = 1
                    },
                    new SimDebugView.ColorThreshold // Coldest Ice Biome
                    {
                        color = new UnityEngine.Color(0,0,1,1),
                        value = 273-60
                    },
                    new SimDebugView.ColorThreshold // Temperate
                    {
                        color = new UnityEngine.Color(0,1,0,1),
                        value = 273+20
                    },
                    new SimDebugView.ColorThreshold // Hot
                    {
                        color = new UnityEngine.Color(1,0.5f,0,1),
                        value = 273+45
                    },
                    new SimDebugView.ColorThreshold // Steam
                    {
                        color = new UnityEngine.Color(1,0,0,1),
                        value = 273+150
                    },
                    new SimDebugView.ColorThreshold // Magma
                    {
                        color = new UnityEngine.Color(1,0,0.35f,1),
                        value = 273+1250
                    },
                    new SimDebugView.ColorThreshold // Spare
                    {
                        color = new UnityEngine.Color(1,0,0.35f,1),
                        value = 273+1250+1
                    },
                };
            }
        }
    }
}

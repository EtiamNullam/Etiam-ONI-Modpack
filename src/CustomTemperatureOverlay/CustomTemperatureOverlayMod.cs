using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CustomTemperatureOverlay.HSV;
using Harmony;
using UnityEngine;

namespace CustomTemperatureOverlay
{
    public static class CustomTemperatureOverlayMod
    {
        // TODO: load config from file
        // TODO: set filewatcher on config
        // TODO: set keybind to set ranges to currently visible (adaptive overlay)

        [HarmonyPatch(typeof(SimDebugView), nameof(SimDebugView.NormalizedTemperature))]
        public static class SimDebugView_NormalizedTemperature
        {
            /// <summary>
            /// Copied from decompiled code, just changed interpolation from RGB to HSV
            /// </summary>
            // TODO: use transpiler instead
            public static bool Prefix(SimDebugView __instance, float temperature, ref Color __result)
            {
                int num = 0;
                int num2 = 0;
                for (int i = 0; i < __instance.temperatureThresholds.Length; i++)
                {
                    if (temperature <= __instance.temperatureThresholds[i].value)
                    {
                        num2 = i;
                        break;
                    }
                    num = i;
                    num2 = i;
                }
                float num3 = 0f;
                if (num != num2)
                {
                    num3 = (temperature - __instance.temperatureThresholds[num].value) / (__instance.temperatureThresholds[num2].value - __instance.temperatureThresholds[num].value);
                }
                num3 = Mathf.Max(num3, 0f);
                num3 = Mathf.Min(num3, 1f);

                __result = LerpHSV(__instance.temperatureThresholds[num].color, __instance.temperatureThresholds[num2].color, num3);

                return false;
            }

            private static ColorHSV LerpHSV(ColorHSV first, ColorHSV second, float factor)
            {
                return new ColorHSV
                (
                    Mathf.Lerp(first.H, second.H, factor),
                    Mathf.Lerp(first.S, second.S, factor),
                    Mathf.Lerp(first.V, second.V, factor),
                    Mathf.Lerp(first.A, second.A, factor)
                );
            }
        }

        [HarmonyPatch(typeof(SimDebugView), "OnPrefabInit")]
        public static class SimDebugView_OnPrefabInit
        {
            public static void Postfix(SimDebugView __instance)
            {
                __instance.temperatureThresholds = new SimDebugView.ColorThreshold[]
                {
                    new SimDebugView.ColorThreshold // Exact Absolute Zero
                    {
                        color = new UnityEngine.Color(1,1,1,1),
                        value = 0
                    },
                    new SimDebugView.ColorThreshold // Near Absolute Zero
                    {
                        color = new UnityEngine.Color(0.7f,0,1,1),
                        value = 5
                    },
                    new SimDebugView.ColorThreshold // Coldest Ice Biome
                    {
                        color = new UnityEngine.Color(0,0,1,0.75f),
                        value = 273-60
                    },
                    new SimDebugView.ColorThreshold // Temperate
                    {
                        color = new UnityEngine.Color(0,1,0,0.75f),
                        value = 273+20
                    },
                    new SimDebugView.ColorThreshold // Spare2
                    {
                        color = new UnityEngine.Color(1,0.75f,0,0.75f),
                        value = 273+50
                    },
                    new SimDebugView.ColorThreshold // Hot Steam
                    {
                        color = new UnityEngine.Color(1,0,0,0.75f),
                        value = 273+125
                    },
                    new SimDebugView.ColorThreshold // Magma
                    {
                        color = new UnityEngine.Color(1,0,0.35f,0.75f),
                        value = 273+1250
                    },
                    new SimDebugView.ColorThreshold // Hot Magma
                    {
                        color = new UnityEngine.Color(1,0,0.7f,0.75f),
                        value = 273+2000
                    },
                };
            }
        }
    }
}

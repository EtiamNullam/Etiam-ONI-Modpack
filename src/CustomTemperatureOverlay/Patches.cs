using System;
using System.Collections.Generic;
using HarmonyLib;
using UnityEngine;

namespace CustomTemperatureOverlay
{
    public class Patches : KMod.UserMod2
    {
        public override void OnLoad(Harmony harmony)
        {
            base.OnLoad(harmony);

            Mod.Config.Load();
            Mod.Config.Watch();
        }

        [HarmonyPatch(typeof(SimDebugView))]
        [HarmonyPatch("NormalizedTemperature")]
        public static class SimDebugView_NormalizedTemperature
        {
            public static bool Prefix(float actualTemperature, ref Color __result)
            {
                try
                {
                    if (Game.Instance.temperatureOverlayMode != Game.TemperatureOverlayModes.AbsoluteTemperature)
                    {
                        return true;
                    }

                    __result = Mod.GetTemperatureColor(actualTemperature);

                    return false;
                }
                catch (Exception e)
                {
                    State.Common.Logger.LogOnce("Failed to calculate normalized temperature", e);
                }

                return true;
            }
        }

        [HarmonyPatch(typeof(OverlayModes.Temperature))]
        [HarmonyPatch("RefreshLegendValues")]
        public static class OverlayModes_RefreshLegendValues
        {
            public static void Postfix(List<LegendEntry> ___temperatureLegend)
            {
                try
                {
                    Mod.UpdateTemperatureLegend(ref ___temperatureLegend);
                }
                catch (Exception e)
                {
                    State.Common.Logger.LogOnce("Failed to update temperature legend", e);
                }
            }
        }
    }
}

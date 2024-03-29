﻿using System;
using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using UnityEngine;

namespace GasOverlay
{
    public static class GasOverlayMod
    {
        private const string ConfigFileName = "Config.json";
        private const string OverlayTitle = "Gas Overlay";

        private static Color[] LastColors;

        private static void ResetLastColors()
        {
            LastColors = Enumerable
                .Repeat(State.Config.NotGasColor, Grid.CellCount)
                .ToArray();
        }

        public static void OnLoad()
        {
            if (State.Common.ConfigPath == null)
            {
                return;
            }

            State.Common.WatchConfig<Config>(ConfigFileName, LoadConfig);

            try
            {
                LoadConfig(State.Common.LoadConfig<Config>(ConfigFileName));
            }
            catch (Exception e)
            {
                State.Common.Logger.Log("Error while loading config." + e);
            }
        }

        private static void LoadConfig(Config config)
        {
            State.Config = config;

            State.Config.InterpFactor = Mathf.Clamp(State.Config.InterpFactor, float.Epsilon, 1);
            State.Config.MinimumIntensity = Mathf.Clamp01(State.Config.MinimumIntensity);

            ResetLastColors();

            State.Common.Logger.Log("Config loaded.");
        }

        private static Color32 GetSubstanceColor(Element element)
        {
            var color = element.substance.colour;

            return new Color32
            (
                color.r,
                color.g,
                color.b,
                byte.MaxValue
            );
        }

        [HarmonyPatch(typeof(SimDebugView), "GetOxygenMapColour")]
        public static class SimDebugView_GetOxygenMapColour
        {
            // TODO: add error handling
            public static bool Prefix(int cell, ref Color __result)
            {
                var element = Grid.Element[cell];

                if (LastColors == null || cell >= LastColors.Length)
                {
                    ResetLastColors();
                }

                var color = element.IsGas
                    ? GetGasColor(element, cell)
                    : State.Config.NotGasColor;

                var lastColor = LastColors[cell];

                if (lastColor != color)
                {
                    TransitColor(ref color, lastColor);
                    LastColors[cell] = color;
                }

                __result = color;

                return false;
            }

            private static Color GetGasColor(Element element, int cellIndex)
            {
                float mass = Grid.Mass[cellIndex];
                float maxMass = State.Config.MaxMass;

                float pressure = GetPressureFraction(mass, maxMass);

                Color color = GetSubstanceColor(element);

                ScaleToPressure(ref color, pressure);

                if (State.Config.ShowEarDrumPopMarker && mass > State.Config.EarPopMass)
                {
                    MarkEarDrumPop(ref color, element.id);
                }

                return color;
            }

            private static float GetPressureFraction(float mass, float maxMass)
            {
                float minFraction = State.Config.MinimumIntensity;
                float fraction = mass / maxMass;

                float lerped = Mathf.Lerp(minFraction, 1, fraction);

                float exponentRootOf = (float)Math.Pow(lerped, State.Config.ExponentRootOf);

                return exponentRootOf;
            }

            private static void ScaleToPressure(ref Color color, float fraction)
            {
                color = Color.LerpUnclamped(Color.white, color, fraction);
            }

            private static void MarkEarDrumPop(ref Color color, SimHashes elementID)
            {
                switch (elementID)
                {
                    case SimHashes.CarbonDioxide:
                    case SimHashes.SourGas:
                        color.r = Mathf.Clamp01(color.r + State.Config.EarPopChange);
                        break;
                    case SimHashes.Methane:
                    case SimHashes.ContaminatedOxygen:
                        color.g = Mathf.Clamp01(color.g + State.Config.EarPopChange);
                        break;
                    default:
                        color.g = Mathf.Clamp01(color.g - State.Config.EarPopChange);
                        break;
                }
            }

            private static void TransitColor(ref Color newColor, Color lastColor)
            {
                newColor = Color.LerpUnclamped(lastColor, newColor, State.Config.InterpFactor);
            }
        }

        [HarmonyPatch(typeof(OverlayLegend), "OnSpawn")]
        public static class OverlayLegend_OnSpawn
        {
            public static void Postfix(List<OverlayLegend.OverlayInfo> ___overlayInfoList)
            {
                var oxygenOverlay = ___overlayInfoList
                    .Find(info => info.mode == OverlayModes.Oxygen.ID);

                var icon = oxygenOverlay.infoUnits[0].icon;

                oxygenOverlay.name = OverlayTitle.ToUpper();
                oxygenOverlay.infoUnits = ElementLoader.elements
                    .Where(ShouldBeListedInOverlayLegend)
                    .OrderBy(element => element.molarMass)
                    .Select(element => new OverlayLegend.OverlayInfoUnit(
                        icon,
                        Util.StripTextFormatting(element.name),
                        GetSubstanceColor(element),
                        Color.white))
                    .ToList();
            }

            private static bool ShouldBeListedInOverlayLegend(Element element)
                => element.disabled == false
                && element.IsGas
                && element.lowTemp < 1000;
        }

        [HarmonyPatch(typeof(OverlayMenu), "InitializeToggles")]
        public static class OverlayMenu_InitializeToggles
        {
            public static void Postfix(List<KIconToggleMenu.ToggleInfo> ___overlayToggleInfos)
            {
                var toggle = ___overlayToggleInfos
                    .Find(info => Traverse
                        .Create(info)
                        .Field("simView")
                        .GetValue<HashedString>() == OverlayModes.Oxygen.ID);

                toggle.text = OverlayTitle;
                toggle.tooltipHeader = OverlayTitle;
                toggle.tooltip = GameUtil.ReplaceHotkeyString(
                    "Displays gasses {Hotkey}",
                    toggle.hotKey
                );
            }
        }
    }
}

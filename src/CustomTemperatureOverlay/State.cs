﻿using Common;
using CustomTemperatureOverlay.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace CustomTemperatureOverlay
{
    public static class State
    {
        public static Core Common = new Core("CustomTemperatureOverlay", "1878592057", null, true);

        public const string ConfigFileName = "Config.json";

        public static TemperatureStep[] Steps = new[]
        {
            new TemperatureStep // Exact Absolute Zero
            (
                new Color(1, 1, 1, 1),
                0
            ),
            new TemperatureStep // Near Absolute Zero
            (
                new Color(0.35f, 0, 1, 1),
                5
            ),
            new TemperatureStep // Coldest Ice Biome
            (
                new Color(0.1f, 0.1f, 1, 1),
                273-60
            ),
            new TemperatureStep // Temperate
            (
                new Color(0, 1, 0, 1),
                273+20
            ),
            new TemperatureStep // Warm
            (
                new Color(1, 1, 0, 1),
                273+35
            ),
            new TemperatureStep // Hot
            (
                new Color(1, 0.5f, 0, 1),
                273+60
            ),
            new TemperatureStep // Hot Steam
            (
                new Color(0.9f, 0, 0, 1),
                273+125
            ),
            new TemperatureStep // Hot Magma
            (
                new Color(1, 0.1f, 0.45f, 1),
                273+2000
            ),
        };
    }
}


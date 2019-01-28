using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace CustomTemperatureOverlay
{
    public static class State
    {
        public static Core Common = new Core("CustomTemperatureOverlay", null, true);

        public static readonly SimDebugView.ColorThreshold[] DefaultThresholds = new[]
        {
            new SimDebugView.ColorThreshold // Exact Absolute Zero
            {
                color = new Color(1,1,1,0.85f),
                value = 0
            },
            new SimDebugView.ColorThreshold // Near Absolute Zero
            {
                color = new Color(0.35f,0,1,0.9f),
                value = 5
            },
            new SimDebugView.ColorThreshold // Coldest Ice Biome
            {
                color = new Color(0,0,1,0.8f),
                value = 273-60
            },
            new SimDebugView.ColorThreshold // Temperate
            {
                color = new Color(0,1,0,0.6f),
                value = 273+20
            },
            new SimDebugView.ColorThreshold // Warm
            {
                color = new Color(0.9f,0.6f,0,0.75f),
                value = 273+50
            },
            new SimDebugView.ColorThreshold // Hot Steam
            {
                color = new Color(0.9f,0,0,0.85f),
                value = 273+125
            },
            new SimDebugView.ColorThreshold // Hot Magma
            {
                color = new Color(1,0,0.35f,0.9f),
                value = 273+2000
            },
        };
    }
}


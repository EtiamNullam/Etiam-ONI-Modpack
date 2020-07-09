using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace CustomTemperatureOverlay.Data
{
    public class TemperatureStep
    {
        public TemperatureStep(Color color, float value)
        {
            this.color = color;
            this.value = value;
        }

        public Color color { get; set; }

        public float value { get; set; }
    }
}

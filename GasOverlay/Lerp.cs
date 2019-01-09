using GasOverlay.HSV;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace GasOverlay
{
    public static class Lerp
    {
        public static ColorHSV HSV(ColorHSV a, ColorHSV b, float t)
        {
            // Hue interpolation
            float h;
            float d = b.H - a.H;
            if (a.H > b.H)
            {
                // Swap (a.h, b.h)
                var h3 = b.H;
                b.H = a.H;
                a.H = h3;

                d = -d;
                t = 1 - t;
            }

            if (d > 0.5) // 180deg
            {
                a.H = a.H + 1; // 360deg
                h = (a.H + t * (b.H - a.H)) % 1; // 360deg
            }
            else //if (d <= 0.5) // 180deg
            {
                h = a.H + t * d;

            }

            // Interpolates the rest
            return new ColorHSV
            (
                h,          // H
                a.S + t * (b.S - a.S),  // S
                a.V + t * (b.V - a.V),  // V
                a.A + t * (b.A - a.A)   // A
            );
        }
    }
}

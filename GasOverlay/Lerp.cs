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
        public static ColorHSV HSV(ColorHSV first, ColorHSV second, float factor)
        {
            return new ColorHSV
            (
                Mathf.Lerp(first.H, second.H, factor),
                Mathf.Lerp(first.S, second.S, factor),
                Mathf.Lerp(first.V, second.V, factor),
                1
            );
        }

        public static Color RGB(Color first, Color second, float factor)
        {
            return new Color
            (
                Mathf.Lerp(first.r, second.r, factor),
                Mathf.Lerp(first.g, second.g, factor),
                Mathf.Lerp(first.b, second.b, factor),
                1
            );
        }
    }
}

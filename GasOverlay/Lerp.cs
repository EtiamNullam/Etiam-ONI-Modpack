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
        public static ColorHSV HSV(ColorHSV a, ColorHSV b, float factor)
        {
            var resultVector = Vector3.Lerp(new Vector3(a.H, a.S, a.V), new Vector3(b.H, b.S, b.V), factor);

            return new ColorHSV
            (
                resultVector.x,
                resultVector.y,
                resultVector.z,
                1
            );
        }
    }
}

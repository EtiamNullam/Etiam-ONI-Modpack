using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace MaterialColor.Extensions
{
    public static class ColorExtensions
    {
        public static Color ToTileColor(this Color color)
        {
            if (color.a == 0)
            {
                color = new Color
                (
                    ProcessTileColorCompponent(color.r),
                    ProcessTileColorCompponent(color.g),
                    ProcessTileColorCompponent(color.b),
                    1
                );
            }
            else
            {
                color.a = 1;
            }

            return color;
        }

        private static float ProcessTileColorCompponent(float colorComponent)
        {
            return (colorComponent + State.Config.TileColorShift) * State.Config.TileColorFactor;
        }
    }
}

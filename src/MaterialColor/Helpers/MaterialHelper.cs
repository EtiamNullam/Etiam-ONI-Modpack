﻿namespace MaterialColor.Helpers
{
    using System;

    using UnityEngine;

    public static class MaterialHelper
    {
        public static bool CellIndexToElement(int cellIndex, out Element element)
        {
            ushort cellElementIndex = Grid.ElementIdx[cellIndex];

            element = ElementLoader.elements?[cellElementIndex];

            return element != null;
        }
    }
}
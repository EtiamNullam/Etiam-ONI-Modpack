namespace MaterialColor.Helpers
{
    using Extensions;
    using MaterialColor.Data;
    using System;
    using UnityEngine;

    public static class ColorHelper
    {
        public static readonly Color DefaultColor = new Color(1, 1, 1, 1);
        public static readonly Color InvalidColor = new Color(1, 0, 1, 1);

        /// <summary>
        /// Tries to get material color for given component, if not possible extracts substance.conduitColour, then uses white color as last fallback.
        /// </summary>
        public static Color GetComponentMaterialColor(Component component)
        {
            if (State.Config.Enabled)
            {
                PrimaryElement primaryElement = component.GetComponent<PrimaryElement>();

                if (primaryElement != null)
                {
                    SimHashes material = primaryElement.ElementID;

                    return State.ElementColors.TryGetValue(material, out ElementColor elementColor)
                        ? elementColor.ToColor()
                        : ExtractGameColor(primaryElement);
                }
            }

            return ColorHelper.DefaultColor;
        }

        private static Color ExtractGameColor(PrimaryElement primaryElement)
        {
            Color resultColor = primaryElement.Element.substance.colour;
            resultColor.a = 1;

            return resultColor;
        }
    }
}
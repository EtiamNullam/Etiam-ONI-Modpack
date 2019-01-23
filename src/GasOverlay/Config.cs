namespace GasOverlay
{
    public class Config
    {
        public float MaxMass = 2.5f;
        public float MinimumIntensity = 0.4f;
        public bool ShowEarDrumPopMarker = true;
        public float EarPopMass = 5;
        public float EarPopChange = 0.25f;
        public float EarPopInversePoint = 0.65f;

        /// <summary>
        /// 0-1, where 1 means instant and 0 means no change.
        /// </summary>
        public float InterpFactor = 0.05f;
    }
}
namespace GasOverlay
{
    public class Config
    {
        public float MaxMass = 2.5f;
        public float MinimumIntensity = 0.3f;
        public bool ShowEarDrumPopMarker = true;
        public float EarPopMass = 5;
        public float EarPopChange = 0.2f;

        /// <summary>
        /// 0-1, where 1 means instant and 0 means no change.
        /// </summary>
        public float InterpFactor = 0.05f;
    }
}
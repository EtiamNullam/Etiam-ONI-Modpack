namespace GasOverlay
{
    public class Config
    {
        public float MaxMass = 2.5f;
        public float MinimumIntensity = 0.5f;
        public bool ShowEarDrumPopMarker = true;
        public float ValueFactor = 0.5f;
        public float ValueFactorCarbonDioxide = 2f;
        public float SaturationFactor = 1.25f;
        public float EarPopMass = 5;

        /// <summary>
        /// 0-1, where 1 means instant and 0 means no change.
        /// </summary>
        public float InterpFactor = 0.025f;
    }
}
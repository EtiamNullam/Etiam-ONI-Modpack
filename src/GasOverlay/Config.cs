namespace GasOverlay
{
    public class Config
    {
        public float MaxMass { get; set; } = 2.5f;
        public float MinimumIntensity { get; set; } = 0.25f;
        public bool ShowEarDrumPopMarker { get; set; } = true;
        public float ValueFactor { get; set; } = 0.5f;
        public float ValueFactorCarbonDioxide { get; set; } = 2f;
        public float SaturationFactor { get; set; } = 1.25f;
        public float EarPopMass { get; set; } = 5;

        /// <summary>
        /// 0-1, where 1 means instant and 0 means no change.
        /// </summary>
        public float InterpFactor { get; set; } = 0.08f;
    }
}
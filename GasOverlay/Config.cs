namespace GasOverlay
{
    public class Config
    {
        public float GasPressureEnd = 2.5f;
        public float MinimumGasColorIntensity { get; set; } = 0.25f;
        public bool ShowEarDrumPopMarker { get; set; } = true;
        public float FactorValueHSVGases { get; set; } = 0.5f;
        public float FactorValueHSVCarbonDioxide { get; set; } = 2f;
        public float EarPopFloat { get; set; } = 5;

        /// <summary>
        /// 0-1, where 1 means instant and 0 means no change.
        /// </summary>
        public float Interpolation = 0.08f;
    }
}
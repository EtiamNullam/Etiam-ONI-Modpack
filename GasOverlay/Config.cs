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
    }
}
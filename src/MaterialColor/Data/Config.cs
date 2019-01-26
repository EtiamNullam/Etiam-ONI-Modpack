using System.Collections.Generic;

namespace MaterialColor.Data
{
    public class Config
    {
        public bool Enabled { get; set; } = true;

        public bool Debug { get; set; } = true;

        public bool LogElementsData { get; set; } = false;

        public bool ShowMissingElements { get; set; }

        public FilterInfo TypeFilterInfo { get; set; } = new FilterInfo();

        public float TileColorFactor { get; set; } = 2.5f;

        public float TileColorShift { get; set; } = 0.275f;
    }
}
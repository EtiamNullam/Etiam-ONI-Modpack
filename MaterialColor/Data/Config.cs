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
    }
}
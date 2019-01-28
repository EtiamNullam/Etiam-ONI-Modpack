using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GasOverlay
{
    public static class State
    {
        public static Core Common = new Core("GasOverlay", null, false);

        public static Config Config = new Config();
    }
}

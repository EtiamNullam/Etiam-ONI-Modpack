using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ChainedDeconstruction
{
    public static class State
    {
        public static string[] Chainables = new string[]
        {
            "Ladder",
            "LadderFast",
            "SteelLadder",
            "TravelTube",
            "FirePole"
        };

        public static Core Common = new Core("ChainedDeconstruction", "1737893485", null, false);

        public static bool ChainAll = false;
    }
}

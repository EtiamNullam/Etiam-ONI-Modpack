using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Common
{
    public static class ModState
    {
        public static void Initialize(string name)
        {
            Name = name;
            RootPath = Pathfinder.FindModRootPath(name);
        }

        public static string Name { get; private set; } = "UnspecifiedMod";
        public static string RootPath { get; private set; } = null;
        public static bool Debug { get; set; } = true;
    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Common
{
    public static class ModState
    {
        public static void Initialize(string modName, string configDirectoryPath)
        {
            Name = modName;
            RootPath = ConfigPath = Pathfinder.FindModRootPath(modName);

            if (configDirectoryPath != null)
            {
                ConfigPath += Path.DirectorySeparatorChar + configDirectoryPath;
            }

            Logger.LogDebug($"Initialized {modName} mod at path {RootPath}, with config root at {ConfigPath}");
        }

        public static string Name { get; private set; } = "UnspecifiedMod";
        public static string RootPath { get; private set; } = null;
        public static string ConfigPath { get; private set; } = null;
        public static bool Debug { get; set; } = true;
    }
}

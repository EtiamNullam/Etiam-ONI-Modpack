using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Common
{
    public static class Pathfinder
    {
        public static bool FindModRootPath(string modName, string workshopID, out string rootPath)
        {
            try
            {
                string steamModsPath = MergePath(
                    Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                    "Klei",
                    "OxygenNotIncluded",
                    "mods"
                );

                var possibleLocations = new string[]
                {
                    MergePath(steamModsPath, "Steam"),
                    MergePath(steamModsPath, "Local"),
                    MergePath(steamModsPath, "Dev"),
                    "Mods"
                };

                var possibleNames = new string[]
                {
                    workshopID,
                    modName
                };

                foreach (var location in possibleLocations)
                {
                    foreach (var name in possibleNames)
                    {
                        string[] directories = Directory.GetDirectories(
                            location,
                            name,
                            SearchOption.TopDirectoryOnly
                        );

                        rootPath = directories.FirstOrDefault();

                        if (rootPath != null)
                        {
                            return true;
                        }
                    }
                }

                Logger.Default.Log("Couldn't find mod root path.");
                rootPath = null;
                return false;
            }
            catch (Exception e)
            {
                Logger.Default.Log("Error while searching for mod root path.", e);
            }

            rootPath = string.Empty;
            return false;
        }

        public static string MergePath(params string[] pathSegments)
        {
            var builder = new StringBuilder(pathSegments[0]);

            for (int i = 1; i < pathSegments.Length; i++)
            {
                builder.Append(Path.DirectorySeparatorChar + pathSegments[i]);
            }

            return builder.ToString();
        }
    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Common
{
    public static class Pathfinder
    {
        public static bool FindModRootPath(string workshopID, out string rootPath)
        {
            try
            {
                var steamModsPath = MergePath(
                    Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                    "Klei",
                    "OxygenNotIncluded",
                    "mods",
                    "Steam"
                );

                var directories = Directory.GetDirectories(
                    steamModsPath,
                    workshopID,
                    SearchOption.AllDirectories
                );

                rootPath = directories.FirstOrDefault();

                if (rootPath == null)
                {
                    Logger.Default.Log("Couldn't find mod root path.");
                    return false;
                }

                return true;
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

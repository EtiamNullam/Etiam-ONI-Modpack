using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Common
{
    public static class Pathfinder
    {
        public static string FindModRootPath(string name)
        {
            try
            {
                // TODO: search by .dll location instead, then get its containing directory
                var directories = Directory.GetDirectories("Mods", name, SearchOption.AllDirectories);
                var modRootPath = directories.FirstOrDefault();

                if (modRootPath != null)
                {
                    return modRootPath;
                }
                else
                {
                    Logger.Default.Log("Couldn't find mod root path.");
                }
            }
            catch (Exception e)
            {
                Logger.Default.Log("Error while searching for mod root path.", e);
            }

            return "Mods" + Path.DirectorySeparatorChar + name;
        }

        // TODO: test with interactive
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

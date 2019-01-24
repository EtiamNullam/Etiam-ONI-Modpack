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
                var directories = Directory.GetDirectories("Mods", ModState.Name, SearchOption.AllDirectories);
                var modRootPath = directories.FirstOrDefault();

                if (modRootPath != null)
                {
                    return modRootPath;
                }
                else
                {
                    Logger.Log("Couldn't find mod root path.");
                }
            }
            catch (Exception e)
            {
                Logger.Log("Error while searching for mod root path.", e);
            }

            return "Mods" + Path.DirectorySeparatorChar + name;
        }

        public static void ToRelativePath(ref string path)
        {
            path = ModState.RootPath + Path.DirectorySeparatorChar + path;
        }
    }
}

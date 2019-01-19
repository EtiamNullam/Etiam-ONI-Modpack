using System.IO;

namespace MaterialColor.IO
{
    // TODO: refactor, split
    public static class Paths
    {
        public const string ModsPath = "Mods";
        public const string ModName = "MaterialColor";

        /// <summary>
        /// Replaced on runtime by ConfigWatch.SetPaths
        /// </summary>
        public static string MaterialMainPath = ModsPath + Path.DirectorySeparatorChar + ModName;

        public const string MainConfigFileName = "Config.json";

        public static string MaterialConfigPath
            => MaterialMainPath + Path.DirectorySeparatorChar + "Config";

        public static string SpritesPath
            => MaterialMainPath + Path.DirectorySeparatorChar + "Sprites";

        public static string ElementColorsDirectory
            => MaterialConfigPath + Path.DirectorySeparatorChar + "ElementColors";

        public static string MainConfigPath
            => MaterialConfigPath + Path.DirectorySeparatorChar + MainConfigFileName;

        public static string IconPath
            => SpritesPath + Path.DirectorySeparatorChar + "overlay_materialColor.png";
    }
}
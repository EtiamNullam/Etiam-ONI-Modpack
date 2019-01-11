using System.IO;

namespace MaterialColor.IO
{
    // TODO: refactor, split
    public static class Paths
    {
        public static readonly string MaterialMainPath = "Mods" + Path.DirectorySeparatorChar + "MaterialColor";

        public const string MainConfigFileName = "Config.json";

        public static readonly string MaterialConfigPath = MaterialMainPath + Path.DirectorySeparatorChar + "Config";
        public static readonly string SpritesPath = MaterialMainPath + Path.DirectorySeparatorChar + "Sprites";

        public static readonly string ElementColorsDirectory = MaterialConfigPath + Path.DirectorySeparatorChar + "ElementColors";

        public static readonly string MainConfigPath = MaterialConfigPath + Path.DirectorySeparatorChar + MainConfigFileName;

        public static readonly string IconPath = SpritesPath + Path.DirectorySeparatorChar + "overlay_materialColor.png";
    }
}
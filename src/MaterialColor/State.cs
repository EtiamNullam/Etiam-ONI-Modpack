namespace MaterialColor
{
    using JetBrains.Annotations;
    using MaterialColor.Data;
    using MaterialColor.IO;
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using UnityEngine;
    using System.Linq;

    public static class State
    {
        // TODO: load from file instead
        [NotNull]
        public static readonly List<string> TileNames = new List<string>
        {
            "Tile",
            "MeshTile",
            "GlassTile",
            "BunkerTile",
            "InsulationTile",
            "GasPermeableMembrane",
            "TilePOI",
            "PlasticTile",
            "MetalTile",
            "CarpetTile",
        };

        private static Config _config = new Config();

        [NotNull]
        public static Config Config
        {
            get
            {
                return _config;
            }
            private set
            {
                _config = value;

                try
                {
                    TypeFilter = new TextFilter(_config.TypeFilterInfo);
                }
                catch (Exception e)
                {
                    Logger.LogOnce("Error while creating new TextFilter object", e);
                }
			}
		}

        public static TextFilter TypeFilter { get; private set; }

        [NotNull]
        public static Dictionary<SimHashes, ElementColor> ElementColors { get; private set; } = new Dictionary<SimHashes, ElementColor>();

        public static void LoadMainConfig()
        {
            string path = Paths.MainConfigPath;

            if (File.Exists(path))
            {
                Config = JsonConvert.DeserializeObject<Config>(File.ReadAllText(path));
                Logger.Log("Main config loaded: ", JsonConvert.SerializeObject(Config));
            }
            else
            {
                Logger.Log("Trying to load config from " + path + " but it doesn't exist.");
            }
        }

        public static void LoadElementColors()
        {
            var newElementColors = new Dictionary<SimHashes, ElementColor>();
            foreach (string filePath in Directory.GetFiles(Paths.ElementColorsDirectory, "*.json"))
            {
                string json = File.ReadAllText(filePath);
                var fileElementColors = JsonConvert.DeserializeObject<Dictionary<SimHashes, ElementColor>>(json);
                newElementColors = newElementColors.Concat(fileElementColors).ToDictionary(pair => pair.Key, pair => pair.Value);
            }

            ElementColors = newElementColors;

            Logger.Log("Element colors loaded.");
        }
    }
}
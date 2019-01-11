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
        // TODO: load on game start
        public static Config Config
        {
            get
            {
                return _config;
            }
            set
            {
                _config = value;

                try
                {
                    TypeFilter = new TextFilter(_config.TypeFilterInfo);
                }
                catch (Exception e)
                {
                    Logger.LogOnce("Error while creating new TextFilter object");
                    Logger.LogDebug(e);
                }
			}
		}

        public static TextFilter TypeFilter { get; set; }

        [NotNull]
        // TODO: load on game start
        public static Dictionary<SimHashes, ElementColor> ElementColors { get; set; } = new Dictionary<SimHashes, ElementColor>();

        public static Config LoadMainConfig()
        {
            return File.Exists(Paths.MaterialConfigPath)
                ? JsonConvert.DeserializeObject<Config>(File.ReadAllText(Paths.MaterialConfigPath))
                : new Config();
        }

        public static Dictionary<SimHashes, ElementColor> LoadElementColors()
        {
            var newElementColors = new Dictionary<SimHashes, ElementColor>();
            foreach (string filePath in Directory.GetFiles(Paths.ElementColorsDirectory, "*.json"))
            {
                string json = File.ReadAllText(filePath);
                var fileElementColors = JsonConvert.DeserializeObject<Dictionary<SimHashes, ElementColor>>(json);
                newElementColors = newElementColors.Concat(fileElementColors).ToDictionary(pair => pair.Key, pair => pair.Value);
            }
            return newElementColors;
        }
    }
}
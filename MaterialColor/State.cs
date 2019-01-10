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

        private static Config _config;
        private static Dictionary<SimHashes, ElementColor> _elementColors;

        [NotNull]
        public static Config Config
        {
            get
            {
                if (_config != null)
                {
                    return _config;
                }

                return LoadMainConfig();
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
                    Debug.Log("Error while creating new TextFilter object");
                    Debug.Log(e);
                }
			}
		}

        public static TextFilter TypeFilter { get; set; }

        [NotNull]
        public static Dictionary<SimHashes, ElementColor> ElementColors
        {
            get
            {
                if (_elementColors != null)
                {
                    return _elementColors;
                }

                return LoadElementColors();
            }
			set
			{
				_elementColors = value;
			}
		}

        public static Config LoadMainConfig()
        {
            return File.Exists(Paths.MaterialConfigPath)
                ? JsonConvert.DeserializeObject<Config>(File.ReadAllText(Paths.MaterialConfigPath))
                : new Config();
        }

        public static Dictionary<SimHashes, ElementColor> LoadElementColors()
        {
            var newElementColors = new Dictionary<SimHashes, ElementColor>();
            foreach (string filePath in Directory.GetFiles(Paths.ElementColorInfosDirectory))
            {
                string json = File.ReadAllText(filePath);
                var fileElementColors = JsonConvert.DeserializeObject<Dictionary<SimHashes, ElementColor>>(json);
                newElementColors.Concat(fileElementColors);
            }
            return newElementColors;
        }
    }
}
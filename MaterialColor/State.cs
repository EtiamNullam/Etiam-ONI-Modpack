namespace MaterialColor
{
    using JetBrains.Annotations;
    using MaterialColor.Data;
    using MaterialColor.IO;
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using UnityEngine;

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

        private static MaterialColorState _configuratorState;

        private static Dictionary<SimHashes, ElementColor> _materialColors;

        [NotNull]
        public static MaterialColorState ConfiguratorState
        {
            get
            {
                if (_configuratorState != null)
                {
                    return _configuratorState;
                }

                MaterialColorState state;

                state = JsonConvert.DeserializeObject<MaterialColorState>(Paths.MaterialColorStatePath);

                ConfiguratorState = state;

                return state;
            }

            private set
            {
                _configuratorState = value;

                try
                {
                    TypeFilter = new TextFilter(_configuratorState.TypeFilterInfo);
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
                if (_materialColors != null)
                {
                    return _materialColors;
                }

                _materialColors = JsonConvert.DeserializeObject<Dictionary<SimHashes, ElementColor>>(Paths.ElementColorInfosDirectory);

                return _materialColors;
            }

			private set
			{
				_materialColors = value;
			}
		}

        public static bool TryReloadConfiguratorState()
        {
            try
            {
                ConfiguratorState = JsonConvert.DeserializeObject<MaterialColorState>(Paths.MaterialConfigPath);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static bool TryReloadElementColorInfos()
        {
            try
            {
                ElementColors = JsonConvert.DeserializeObject<Dictionary<SimHashes, ElementColor>>(Paths.ElementColorInfosDirectory);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
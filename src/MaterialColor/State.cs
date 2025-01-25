﻿namespace MaterialColor
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
    using Common;

    public static class State
    {
        public static Color?[] TileColors;

        public static bool ConfigChanged;

        public static Core Common = new Core("MaterialColor", "1739635014", "Config", false);

        private static Config _config = new Config();

        [NotNull]
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
                    Common.Logger.LogOnce("Error while creating new TextFilter object", e);
                }
            }
        }

        public static TextFilter TypeFilter { get; private set; }

        [NotNull]
        public static Dictionary<SimHashes, ElementColor> ElementColors { get; set; } = new Dictionary<SimHashes, ElementColor>();
    }
}
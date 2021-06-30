using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Common;
using HarmonyLib;
using Newtonsoft.Json;
using UnityEngine;

namespace CustomTemperatureOverlay
{
    public class Patches : KMod.UserMod2
    {
        public override void OnLoad(Harmony harmony)
        {
            base.OnLoad(harmony);

            Mod.Config.Load();
            Mod.Config.Watch();
        }

        [HarmonyPatch(typeof(SimDebugView))]
        [HarmonyPatch("OnPrefabInit")]
        public static class GlobalAssets_OnPrefabInit
        {
            public static void Postfix()
            {
                Mod.UpdateColorSet();
            }
        }
    }
}

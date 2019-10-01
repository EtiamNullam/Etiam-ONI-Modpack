using Harmony;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;

namespace ChainedDeconstruction
{
    [HarmonyPatch(typeof(Deconstructable))]
    [HarmonyPatch("TriggerDestroy")]
    public static class ChainedDeconstruction
    {
        private static readonly MethodInfo ForceDeconstruct = AccessTools.Method(typeof(Deconstructable), "OnCompleteWork");
        private static readonly object[] NullWorkerParameter = new[] { (Worker)null };
        private static readonly AccessTools.FieldRef<Deconstructable, bool> DestroyedGetter = AccessTools.FieldRefAccess<Deconstructable, bool>("destroyed");

        private static readonly string ConfigFileName = "Chainables.json";

        public static void OnLoad()
        {
            if (State.Common.ConfigPath == null)
            {
                return;
            }

            State.Common.WatchConfig<string[]>(ConfigFileName, SetChainables);

            try
            {
                SetChainables(State.Common.LoadConfig<string[]>(ConfigFileName));
            }
            catch (Exception e)
            {
                State.Common.Logger.Log("Error while loading config.", e);
            }
        }

        private static void SetChainables(string[] chainables)
        {
            if (chainables.Any(c => c == "*"))
            {
                State.ChainAll = true;
            }
            else
            {
                State.Chainables = chainables.Select(c => c + "Complete").ToArray();
                State.ChainAll = false;
            }

            State.Common.Logger.Log("Set chainables");
        }

        public static void Postfix(Deconstructable __instance, Building building)
        {
            try
            {
                if (__instance == null || building == null)
                {
                    return;
                }

                var name = __instance.name;

                if (
                    DestroyedGetter(__instance) == false
                    || (
                        State.Chainables.Contains(name) == false
                        && State.ChainAll == false)
                    )
                {
                    return;
                }

                var cell = building.GetCell();

                var adjacentBuildings = new[]
                {
                    Grid.Objects[Grid.CellAbove(cell), (int)ObjectLayer.Building],
                    Grid.Objects[Grid.CellBelow(cell), (int)ObjectLayer.Building],
                    Grid.Objects[Grid.CellLeft(cell), (int)ObjectLayer.Building],
                    Grid.Objects[Grid.CellRight(cell), (int)ObjectLayer.Building],
                };

                foreach (var buildingGameObject in adjacentBuildings)
                {
                    if (buildingGameObject != null && buildingGameObject.name == name)
                    {
                        var deconstructable = buildingGameObject.GetComponent<Deconstructable>();

                        if (deconstructable != null && deconstructable.IsMarkedForDeconstruction() && !DestroyedGetter(deconstructable))
                        {
                            ForceDeconstruct.Invoke(deconstructable, NullWorkerParameter);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Debug.Log("ChainedDeconstruction failed: " + e);
            }
        }
    }
}

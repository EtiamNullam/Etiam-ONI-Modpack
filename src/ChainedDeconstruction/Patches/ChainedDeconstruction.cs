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
        // TODO: move this to config
        private static readonly string[] chainableBuildings = new string[]
        {
            "LadderComplete",
            "LadderFastComplete",
            "TravelTubeComplete",
            "FirePoleComplete"
        };

        private static readonly MethodInfo ForceDeconstruct = AccessTools.Method(typeof(Deconstructable), "OnCompleteWork");
        private static readonly object[] NullWorkerParameter = new[] { (Worker)null };
        private static readonly AccessTools.FieldRef<Deconstructable, bool> DestroyedGetter = AccessTools.FieldRefAccess<Deconstructable, bool>("destroyed");

        public static void Postfix(Deconstructable __instance, Building building)
        {
            var name = __instance.name;

            if (!DestroyedGetter(__instance) || !chainableBuildings.Contains(name))
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
    }
}

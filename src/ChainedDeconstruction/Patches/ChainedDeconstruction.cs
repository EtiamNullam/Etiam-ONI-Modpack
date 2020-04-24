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

        private static bool Busy = false;

        private static readonly List<int> CellsDeconstructed = new List<int>();

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

                State.Common.Logger.Log("Set chainables:", "all");
            }
            else
            {
                State.Chainables = chainables
                    .Select(c => c + "Complete")
                    .ToArray();

                State.ChainAll = false;

                State.Common.Logger.Log("Set chainables:", State.Chainables);
            }
        }

        public static void Postfix(Deconstructable __instance)
        {
            if (Busy)
            {
                return;
            }

            Busy = true;

            try
            {
                if (__instance == null)
                {
                    State.Common.Logger.LogOnce("__instance is null at Deconstructable.TriggerDestroy.Postfix");

                    return;
                }

                var name = __instance.name;

                if (DestroyedGetter(__instance) == false)
                {
                    State.Common.Logger.LogOnce("DestroyedGetter is null:", name);

                    return;
                }

                if (State.ChainAll == false)
                {
                    if (State.Chainables.Contains(name) == false)
                    {
                        return;
                    }
                }

                var layer = GetLayerForDeconstructable(__instance);

                CellsDeconstructed.Clear();

                DeconstructAdjacent(__instance, name, layer);
            }
            catch (Exception e)
            {
                State.Common.Logger.Log("Deconstructable.TriggerDestroy.Postfix", e);
            }
            finally
            {
                Busy = false;
            }
        }

        private static void DeconstructAdjacent(Deconstructable rootDeconstructable, string name, int layer)
        {
            GetAdjacentCells(rootDeconstructable.GetCell())
                .Select(cell => Grid.Objects[cell, layer])
                .Where(gameObject => gameObject != null)
                .Select(gameObject => gameObject.GetComponent<Deconstructable>())
                .Where(deconstructable =>
                    deconstructable != null
                    && deconstructable.IsMarkedForDeconstruction()
                    && deconstructable.name == name
                    && false == DestroyedGetter(deconstructable)
                )
                .Do(deconstructable =>
                {
                    var cell = deconstructable.GetCell();

                    if (false == CellsDeconstructed.Contains(cell))
                    {
                        ForceDeconstruct.Invoke(deconstructable, NullWorkerParameter);
                        CellsDeconstructed.Add(cell);
                        DeconstructAdjacent(deconstructable, name, layer);
                    }
                });
        }

        private static int GetLayerForDeconstructable(Deconstructable deconstructable)
        {
            var cell = deconstructable.GetCell();

            for (int layer = 1; layer <= 27; layer++)
            {
                var obj = Grid.Objects[cell, layer];

                if (obj != null && obj.GetComponent<Deconstructable>() == deconstructable)
                {
                    return layer;
                }
            }

            return (int)ObjectLayer.Building;
        }

        private static IEnumerable<int> GetAdjacentCells(int cell)
        {
            yield return Grid.CellAbove(cell);
            yield return Grid.CellBelow(cell);
            yield return Grid.CellLeft(cell);
            yield return Grid.CellRight(cell);
        }
    }
}

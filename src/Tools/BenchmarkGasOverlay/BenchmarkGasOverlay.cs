using Harmony;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace BenchmarkGasOverlay
{
    [HarmonyPatch(typeof(AttackTool))]
    [HarmonyPatch("OnActivateTool")]
    public static class BenchmarkGasOverlay
    {
        public static void Prefix()
        {
            BenchmarkCell(FindCell(SimHashes.CarbonDioxide, 1, 3), "CO2 no-pop");
            BenchmarkCell(FindCell(SimHashes.CarbonDioxide, 6, 50), "CO2 pop");
            BenchmarkCell(FindCell(SimHashes.Oxygen, 1, 3), "O2 no-pop");
            BenchmarkCell(FindCell(SimHashes.SourGas, 6, 50), "SourGass pop");
            BenchmarkCell(FindCell(SimHashes.SandStone), "Solid(Sandstone)");
        }

        private static int FindCell(SimHashes material, float minMass = -1, float maxMass = -1)
        {
            int resultCell = -1;
            for (int i = 0; i < Grid.CellCount; i++)
            {
                var element = Grid.Element[i];
                var mass = Grid.Mass[i];

                var validMinMass = minMass < 0 || mass > minMass;
                var validMaxMass = maxMass < 0 || mass < maxMass;

                if 
                (
                    element.id == material
                    && validMinMass
                    && validMaxMass
                )
                {
                    resultCell = i;
                    break;
                }
            }

            if (resultCell == -1)
            {
                Debug.Log($"BenchmarkGasOverlay: Can't find gas cell on the map: {Enum.GetName(typeof(SimHashes), material)}, in mass range: {minMass}-{maxMass}");
            }

            return resultCell;
        }

        private static void BenchmarkCell(int cell, string message)
        {

            var instance = SimDebugView.Instance;
            int sample = 100000;
            int tries = 20;
            var traverseMethod = Traverse.Create(instance).Method("GetOxygenMapColour", instance, cell);
            var results = new long[tries];
            var stopWatch = new Stopwatch();

            for (int j = 0; j < tries; j++)
            {
                stopWatch.Start();
                for (int i = 0; i < sample; i++)
                {
                    traverseMethod.GetValue();
                }
                var elapsed = stopWatch.ElapsedMilliseconds;
                results[j] = elapsed;
                stopWatch.Reset();
            }

            var result = new
            {
                time_ms = new
                {
                    min = results.Min(),
                    max = results.Max(),
                    average_ms = results.Average(),
                    total = results.Sum()
                },
                tries,
                sample,
                message
            };

            Debug.Log("BenchmarkGasOverlay: result: " + JsonConvert.SerializeObject(result));
        }
    }
}

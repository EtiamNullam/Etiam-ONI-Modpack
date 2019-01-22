using Harmony;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace BenchmarkGasOverlay
{
    [HarmonyPatch(typeof(SimDebugView))]
    [HarmonyPatch("OnSpawn")]
    public static class BenchmarkGasOverlay
    {
        public static void Postfix()
        {
            BenchmarkElement(SimHashes.CarbonDioxide);
            BenchmarkElement(SimHashes.Oxygen);
            BenchmarkElement(SimHashes.Hydrogen);
            BenchmarkElement(SimHashes.SandStone);
        }

        private static void BenchmarkElement(SimHashes element)
        {
            int benchmarkCell;
            for (benchmarkCell = 0; benchmarkCell < Grid.CellCount; benchmarkCell++)
            {
                if (Grid.Element[benchmarkCell].id == element) break;
            }

            var instance = SimDebugView.Instance;
            int sample = 1000000;
            int tries = 10;
            var traverseMethod = Traverse.Create(instance).Method("GetOxygenMapColour", instance, benchmarkCell);
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
                tries,
                sample,
                time_ms = new
                {
                    min = results.Min(),
                    max = results.Max(),
                    average_ms = results.Average(),
                },
                element = Enum.GetName(typeof(SimHashes), Grid.Element[benchmarkCell].id)
            };

            Debug.Log("Benchmark result: " + JsonConvert.SerializeObject(result));
        }
    }
}

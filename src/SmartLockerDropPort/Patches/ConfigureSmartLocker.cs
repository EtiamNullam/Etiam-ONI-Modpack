using Harmony;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace SmartLockerDropPort.Patches
{
    public static class SmartContainerConfig
    {
        [HarmonyPatch(typeof(StorageLockerSmartConfig))]
        [HarmonyPatch(nameof(StorageLockerSmartConfig.DoPostConfigurePreview))]
        public class StorageLockerSmartConfig_DoPostConfigurePreview
        {
            public static void Postfix(GameObject go, LogicPorts.Port ___OUTPUT_PORT)
            {
                AddResetPort(go, ___OUTPUT_PORT);
            }
        }

        [HarmonyPatch(typeof(StorageLockerSmartConfig))]
        [HarmonyPatch(nameof(StorageLockerSmartConfig.DoPostConfigureUnderConstruction))]
        public class StorageLockerSmartConfig_DoPostConfigureUnderConstruction
        {
            public static void Postfix(GameObject go, LogicPorts.Port ___OUTPUT_PORT)
            {
                AddResetPort(go, ___OUTPUT_PORT);
            }
        }

        [HarmonyPatch(typeof(StorageLockerSmartConfig))]
        [HarmonyPatch(nameof(StorageLockerSmartConfig.DoPostConfigureComplete))]
        public class StorageLockerSmartConfig_DoPostConfigureComplete
        {
            public static void Postfix(GameObject go, LogicPorts.Port ___OUTPUT_PORT)
            {
                AddResetPort(go, ___OUTPUT_PORT);
            }
        }

        [HarmonyPatch(typeof(StorageLockerSmart))]
        [HarmonyPatch("OnSpawn")]
        private static class StorageLockerSmart_OnSpawn
        {
            public static void Postfix(StorageLockerSmart __instance, LogicPorts ___ports, FilteredStorage ___filteredStorage)
            {
                __instance.Subscribe(Convert.ToInt32(GameHashes.LogicEvent), logicValueChanged =>
                {
                    var changed = (LogicValueChanged)logicValueChanged;

                    if (changed.portID == DropPortID
                        && changed.newValue == 1
                        && ___ports.GetInputValue(DropPortID) == 1)
                    {
                        var storage = Traverse.Create(___filteredStorage).Field<Storage>("storage").Value;
                        storage.DropAll();
                    }
                });
            }
        }

        private static HashedString DropPortID = "LockerDropStorage";

        private static void AddResetPort(GameObject gameObject, LogicPorts.Port defaultOutputPort)
        {
            var inputPorts = new[]
            {
                new LogicPorts.Port(DropPortID, new CellOffset(0, 0), "Drops contents", true, LogicPortSpriteType.ResetUpdate)
            };

            var outputPorts = new[]
            {
                defaultOutputPort
            };

            GeneratedBuildings.RegisterLogicPorts(gameObject, inputPorts, outputPorts);
        }
    }
}

using HarmonyLib;
using Klei;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;
using Common;

namespace DoorIcons.Patches
{
    // TODO: allow disabling of all icons with some button and/or hotkey
    public static class DoorPatches
    {
        private static readonly AccessTools.FieldRef
        <
            AccessControlSideScreen,
            Door
        > GetAccessSideScreenDoorTarget = AccessTools.FieldRefAccess
        <
            AccessControlSideScreen,
            Door
        >("doorTarget");

        [HarmonyPatch(typeof(Workable))]
        [HarmonyPatch("OnSpawn")]
        public static class Door_OnSpawn
        {
            public static void Postfix(Workable __instance)
            {
                try
                {
                    if (Game.Instance == null)
                    {
                        return;
                    }

                    var door = __instance.GetComponent<Door>();

                    if (door != null && !State.DoorIcons.ContainsKey(door))
                    {
                        var icon = IconManager.CreateIcon(door);

                        State.DoorIcons.Add
                        (
                            door,
                            icon
                        );

                        __instance.gameObject.Subscribe((int)GameHashes.LogicEvent, data => IconManager.UpdateIcon(door));
                        __instance.gameObject.Subscribe((int)GameHashes.DoorStateChanged, data => IconManager.UpdateIcon(door));
                        __instance.gameObject.Subscribe((int)GameHashes.CopySettings, data => IconManager.UpdateIcon(door));
                        __instance.gameObject.Subscribe((int)GameHashes.DoorControlStateChanged, data => IconManager.UpdateIcon(door));
                        __instance.gameObject.Subscribe((int)GameHashes.ObjectDestroyed, data => IconManager.RemoveIcon(door));
                    }
                }
                catch (Exception e)
                {
                    State.Common.Logger.LogOnce("Error while trying to create door icon", e);
                }
            }
        }

        [HarmonyPatch(typeof(AccessControlSideScreen))]
        [HarmonyPatch("OnPermissionChanged")]
        public static class Door_Update_OnMinionPermissionChange
        {
            public static void Postfix(AccessControlSideScreen __instance)
            {
                var door = GetAccessSideScreenDoorTarget(__instance);

                if (door != null)
                {
                    IconManager.UpdateIcon(door);
                }
            }
        }

        [HarmonyPatch(typeof(AccessControlSideScreen))]
        [HarmonyPatch("RefreshOnline")]
        public static class Door_Update_OnDefaultPermissionChange
        {
            public static void Postfix(AccessControlSideScreen __instance)
            {
                var door = GetAccessSideScreenDoorTarget(__instance);

                if (door != null)
                {
                    IconManager.UpdateIcon(door);
                }
            }
        }

        [HarmonyPatch(typeof(Deconstructable))]
        [HarmonyPatch("OnCompleteWork")]
        public static class Door_Remove_OnDeconstruct
        {
            public static void Postfix(Deconstructable __instance)
            {
                var door = __instance.GetComponent<Door>();

                if (door != null)
                {
                    IconManager.RemoveIcon(door);
                }
            }
        }

        [HarmonyPatch(typeof(StructureTemperatureComponents))]
        [HarmonyPatch(nameof(StructureTemperatureComponents.DoMelt))]
        public static class Door_Remove_OnMelt
        {
            public static void Postfix(PrimaryElement primary_element)
            {
                var door = primary_element.gameObject.GetComponent<Door>();

                if (door != null)
                {
                    IconManager.RemoveIcon(door);
                }
            }
        }
    }
}

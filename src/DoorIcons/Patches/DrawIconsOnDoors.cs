using Harmony;
using Klei;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;

namespace DoorIcons.Patches
{
    // TODO: allow disabling of all icons with some button and/or hotkey
    // TODO: move sprites into project (and into proper directory)
    // TODO: not refreshing on any access change
    // TODO: updates on automation change (when it actually changes state), but not on wire disconnect
    public static class DrawIconsOnDoors
    {
        [HarmonyPatch(typeof(Door))]
        [HarmonyPatch("RefreshControlState")]
        public static class Door_RefreshControlState
        {
            public static void Postfix(Door __instance)
            {
                UpdateIcon(__instance);
            }
        }

        [HarmonyPatch(typeof(AccessControl))]
        [HarmonyPatch("OnControlStateChanged")]
        public static class AccessControl_OnControlStateChanged
        {
            public static void Postfix(AccessControl __instance)
            {
                var door = __instance.GetComponent<Door>();

                if (door != null)
                {
                    UpdateIcon(door);
                }
            }
        }
        //[HarmonyPatch(typeof(Door))]
        //[HarmonyPatch(nameof(Door.OnLogicValueChanged))]
        //public static class x
        //{
        //    public static void Postfix(Door __instance)
        //    {
        //        UpdateIcon(__instance);
        //    }
        //}

        //[HarmonyPatch(typeof(Door))]
        //[HarmonyPatch(nameof(Door.QueueStateChange))]
        //public static class y
        //{
        //    public static void Postfix(Door __instance)
        //    {
        //        UpdateIcon(__instance);
        //    }
        //}

        private static readonly AccessTools.FieldRef
        <
            Door,
            Door.ControlState
        > GetDoorStatus = AccessTools.FieldRefAccess
        <
            Door,
            Door.ControlState
        >("controlState");

        private static readonly AccessTools.FieldRef
        <
            AccessControl,
            List
            <
                KeyValuePair
                <
                    Ref<KPrefabID>,
                    AccessControl.Permission
                >
            >
        > GetSavedPermissions = AccessTools.FieldRefAccess
        <
            AccessControl,
            List
            <
                KeyValuePair
                <
                    Ref<KPrefabID>,
                    AccessControl.Permission
                >
            >
        >("savedPermissions");

        // open, closed, locked, restricted both, restricted left, restricted right, automation open, automation closed
        // automation > restrict > other
        private static void UpdateIcon(Door door)
        {
            ExtendedDoorState state = GetExtendedDoorState(door);
            SetDoorIcon(door, state);
        }

        private static ExtendedDoorState GetExtendedDoorState(Door door)
        {
            var logicPorts = door.GetComponent<LogicPorts>();
            var accessControl = door.GetComponent<AccessControl>();

            if (accessControl == null)
            {
                return ExtendedDoorState.Invalid;
            }

            if (logicPorts != null && logicPorts.IsPortConnected(Door.OPEN_CLOSE_PORT_ID))
            {
                return ExtendedDoorState.Automation;
            }
            else
            {
                switch (GetDoorStatus(door))
                {
                    case Door.ControlState.Auto:
                        if (HasCustomDupePermissions(accessControl))
                        {
                            return ExtendedDoorState.AccessCustom;
                        }

                        if (HasCustomGlobalPermissions(accessControl, out var permission))
                        {
                            return permission;
                        }

                        return ExtendedDoorState.Auto;

                    case Door.ControlState.Opened:
                        if (HasCustomDupePermissions(accessControl))
                        {
                            return ExtendedDoorState.AccessCustom;
                        }

                        if (HasCustomGlobalPermissions(accessControl, out var permission2))
                        {
                            return permission2;
                        }

                        return ExtendedDoorState.Open;

                    case Door.ControlState.Closed:
                        return ExtendedDoorState.Locked;
                }
            }

            return ExtendedDoorState.Invalid;
        }

        private static bool HasCustomGlobalPermissions(AccessControl access, out ExtendedDoorState doorState)
        {
            switch (access.DefaultPermission)
            {
                case AccessControl.Permission.GoLeft:
                    doorState = ExtendedDoorState.AccessLeft;
                    return true;

                case AccessControl.Permission.GoRight:
                    doorState = ExtendedDoorState.AccessRight;
                    return true;

                case AccessControl.Permission.Neither:
                    doorState = ExtendedDoorState.AccessRestricted;
                    return true;

                default:
                    doorState = ExtendedDoorState.Invalid;
                    return false;
            }
        }

        private static bool HasCustomDupePermissions(AccessControl access)
        {
            return GetSavedPermissions(access).Any(p => p.Value != access.DefaultPermission);
        }

        private enum ExtendedDoorState
        {
            Invalid,
            Auto,
            Open,
            Locked,
            Automation,
            AccessLeft,
            AccessRight,
            AccessRestricted,
            AccessCustom,
        }

        private static Dictionary<Door, GameObject> DoorIcons = new Dictionary<Door, GameObject>();

        private static Dictionary<ExtendedDoorState, Sprite> DoorSprites = new Dictionary<ExtendedDoorState, Sprite>
        {
            {ExtendedDoorState.Open, CreateSprite("open.png")},
            {ExtendedDoorState.Locked, CreateSprite("locked.png")},
            {ExtendedDoorState.Automation, CreateSprite("automation.png")},
            {ExtendedDoorState.AccessLeft, CreateSprite("access_left.png")},
            {ExtendedDoorState.AccessRight, CreateSprite("access_right.png")},
            {ExtendedDoorState.AccessRestricted, CreateSprite("access_restricted.png")},
            {ExtendedDoorState.AccessCustom, CreateSprite("access_custom.png")},
        };

        private static void SetDoorIcon(Door door, ExtendedDoorState targetState)
        {
            if (DoorIcons.TryGetValue(door, out var go))
            {
                var renderer = go.GetComponent<SpriteRenderer>();

                if (renderer != null)
                {
                    if (DoorSprites.TryGetValue(targetState, out var newSprite))
                    {
                        renderer.sprite = newSprite;
                        renderer.enabled = newSprite != null;
                    }
                    else
                    {
                        renderer.enabled = false;
                    }
                }
            }
        }

        private static Sprite CreateSprite(string path)
        {
            var width = 256;
            var height = 256;

            if (File.Exists(path))
            {
                byte[] bytes = File.ReadAllBytes(path);

                Texture2D texture = new Texture2D(width, height, TextureFormat.RGB24, false)
                {
                    filterMode = FilterMode.Trilinear
                };

                texture.LoadImage(bytes);

                return Sprite.Create(texture, new Rect(0, 0, width, height), new Vector2(0.5f, 0.0f), 1.0f);
            }
            else
            {
                Debug.Log("File doesn't exist: " + path);
                return null;
            }
        }

        // TODO: remove icon on door deconstruct
        // TODO: check for other ways to remove door, and remove icon there as well
        [HarmonyPatch(typeof(Door))]
        [HarmonyPatch("OnSpawn")]
        public static class DrawSpriteOnWorldCanvas
        {
            public static void Postfix(Door __instance)
            {
                DoorIcons.Add
                (
                    __instance,
                    CreateDoorIcon(__instance)
                );
            }

            private static GameObject CreateDoorIcon(Door door)
            {
                var go = new GameObject("DoorIcon");
                var renderer = go.AddComponent<SpriteRenderer>();

                ExtendedDoorState state = GetExtendedDoorState(door);

                if (DoorSprites.TryGetValue(state, out var newSprite))
                {
                    renderer.sprite = newSprite;
                }
;
                renderer.material.renderQueue = 5000;

                Util.KInstantiate(renderer, GameScreenManager.Instance.worldSpaceCanvas);

                var pos = Grid.PosToXY(door.transform.position);

                //Debug.Log($"x: {pos.X}, y: {pos.Y}");
                go.transform.position = new Vector3
                (
                    pos.X + 0.5f,
                    pos.Y + 0.4f,
                    Grid.GetLayerZ(Grid.SceneLayer.SceneMAX)
                );

                go.transform.localScale = new Vector3
                (
                    0.005f,
                    0.005f,
                    1
                );

                //LogRectTransformOfWorldSpaceCanvasComponents();

                return go;
            }

            private static void LogRectTransformOfWorldSpaceCanvasComponents()
            {
                var rectTransform = GameScreenManager.Instance.worldSpaceCanvas.GetComponent<RectTransform>();
                var comps = rectTransform.GetComponentsInChildren<Component>();

                for (int i = 0; i < comps.Length; i++)
                {
                    Component comp = comps[i];
                    Debug.Log($"component:{i}-{comp}-{comp.name}-{comp.gameObject.name}");
                }
            }
        }
    }
}

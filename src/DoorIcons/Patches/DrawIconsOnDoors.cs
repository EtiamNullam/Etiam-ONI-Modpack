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
    // TODO: move sprites into project (and into proper directory)
    public static class DrawIconsOnDoors
    {
        // TODO: not refreshing on any access change
        // TODO: test automation
        [HarmonyPatch(typeof(Door))]
        [HarmonyPatch("RefreshControlState")]
        public static class x
        {
            public static void Postfix(Door __instance)
            {
                UpdateIcon(__instance);
            }
        }

        [HarmonyPatch(typeof(AccessControl))]
        [HarmonyPatch("OnControlStateChanged")]
        public static class y
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
                if (GetSavedPermissions(accessControl)
                    .Any(p => p.Value != accessControl.DefaultPermission))
                {
                    return ExtendedDoorState.AccessCustom;
                }
                // invalid order here?
                switch (accessControl.DefaultPermission)
                {
                    case AccessControl.Permission.GoLeft:
                        return ExtendedDoorState.AccessLeft;
                    case AccessControl.Permission.GoRight:
                        return ExtendedDoorState.AccessRight;
                    case AccessControl.Permission.Neither:
                        return ExtendedDoorState.AccessRestricted;
                    case AccessControl.Permission.Both:
                        switch (GetDoorStatus(door))
                        {
                            case Door.ControlState.Auto:
                                return ExtendedDoorState.Auto;
                            case Door.ControlState.Closed:
                                return ExtendedDoorState.Locked;
                            case Door.ControlState.Opened:
                                return ExtendedDoorState.Open;
                        }
                        break;
                }
            }

            return ExtendedDoorState.Invalid;
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
            if (DoorIcons.TryGetValue(door, out var go) && DoorSprites.TryGetValue(targetState, out var sprite))
            {
                var renderer = go.GetComponent<SpriteRenderer>();

                if (renderer != null)
                {
                    renderer.sprite = sprite;
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

                //for (int i = 0; i < Enum.GetValues(typeof(GameScreenManager.UIRenderTarget)).Length; i++)
                //{
                //    try
                //    {
                //        Debug.Log("camera z: "
                //            + GameScreenManager.Instance
                //            .GetCamera((GameScreenManager.UIRenderTarget)i)
                //            .transform
                //            .position
                //            .z);
                //    }
                //    catch { }
                //}

                // TODO: horizontal door needs different offset
                // AND rotated icon
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

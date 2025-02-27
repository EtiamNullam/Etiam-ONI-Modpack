﻿using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace DoorIcons
{
    public static class IconManager
    {
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

        private static readonly AccessTools.FieldRef
        <
            Door,
            Door.ControlState
        > GetDoorStatus = AccessTools.FieldRefAccess
        <
            Door,
            Door.ControlState
        >("controlState");

        public static GameObject CreateIcon(Door door)
        {
            var go = new GameObject("DoorIcon");
            var renderer = go.FindOrAddComponent<SpriteRenderer>();

            UpdateIcon(door);

            renderer.material.renderQueue = 5000;

            Util.KInstantiate(renderer, GameScreenManager.Instance.worldSpaceCanvas);

            go.transform.localScale = new Vector3
            (
                0.005f,
                0.005f,
                1
            );

            var pos = Grid.PosToXY(door.transform.position);

            var anchorX = pos.X + 0.5f;
            var anchorY = pos.Y + 0.5f;

            var def = door.building.Def;

            var width = def.WidthInCells;
            var height = def.HeightInCells;

            var rotatable = door.GetComponent<Rotatable>();

            go.transform.position = new Vector3
            (
                anchorX + ((width - 1) / 2f),
                anchorY + ((height - 1) / 2f),
                Grid.GetLayerZ(Grid.SceneLayer.SceneMAX)
            );

            if (rotatable == null)
            {
                State.Common.Logger.LogOnce("Vanilla doors usually have Rotatable component");

                return go;
            }

            switch (rotatable.GetOrientation())
            {
                case Orientation.R90:
                    go.transform.position = new Vector3
                    (
                        anchorX + ((height - 1) / 2f),
                        anchorY + ((width - 1) / 2f),
                        Grid.GetLayerZ(Grid.SceneLayer.SceneMAX)
                    );

                    renderer.transform.Rotate(0, 0, -90);

                    break;
                case Orientation.R180:
                    go.transform.position = new Vector3
                    (
                        anchorX - ((width - 1) / 2f),
                        anchorY - ((height - 1) / 2f),
                        Grid.GetLayerZ(Grid.SceneLayer.SceneMAX)
                    );

                    break;
                case Orientation.R270:
                    go.transform.position = new Vector3
                    (
                        anchorX - ((height - 1) / 2f),
                        anchorY - ((width - 1) / 2f),
                        Grid.GetLayerZ(Grid.SceneLayer.SceneMAX)
                    );

                    renderer.transform.Rotate(0, 0, -90);

                    break;
            }

            return go;
        }

        public static void UpdateIcon(Door door)
        {
            try
            {
                ExtendedDoorState state = GetExtendedDoorState(door);
                SetIcon(door, state);
            }
            catch (Exception e)
            {
                State.Common.Logger.LogOnce("Update icon failed", e);
            }
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

                    case Door.ControlState.Locked:
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

        private static void SetIcon(Door door, ExtendedDoorState targetState)
        {
            if (State.DoorIcons.TryGetValue(door, out var go))
            {
                var renderer = go.GetComponent<SpriteRenderer>();

                if (renderer != null)
                {
                    if (State.DoorSprites.TryGetValue(targetState, out var newSprite))
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

        // TODO: remove also when removed via sandbox mode
        public static void RemoveIcon(Door door)
        {
            if (State.DoorIcons.TryGetValue(door, out var go))
            {
                GameObject.Destroy(go);

                State.DoorIcons.Remove(door);
            }
        }
    }
}

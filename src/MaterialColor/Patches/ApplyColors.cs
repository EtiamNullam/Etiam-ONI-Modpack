using HarmonyLib;
using MaterialColor.Extensions;
using MaterialColor.Helpers;
using Rendering;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using UnityEngine;
using MaterialColor.IO;

namespace MaterialColor.Patches
{
    public static class ApplyColors
    {
        [HarmonyPatch(typeof(Game), "OnSpawn")]
        public static class GameStart
        {
            public static void Postfix()
            {
                TryInitMod();
            }

            private static void TryInitMod()
            {
                try
                {
                    State.TileColors = new Color?[Grid.CellCount];
                    Components.BuildingCompletes.OnAdd += Painter.UpdateBuildingColor;
                    Painter.Refresh();
                }
                catch (Exception e)
                {
                    State.Common.Logger.Log(e);
                }
            }
        }

        [HarmonyPatch(typeof(Ownable), "UpdateTint")]
        public static class Ownable_UpdateTint
        {
            public static void Postfix(Ownable __instance)
            {
                try
                {
                    if (IsOwned(__instance))
                    {
                        Painter.ApplyColorToKAnimControllerBase(__instance);
                    }
                }
                catch (Exception e)
                {
                    State.Common.Logger.LogOnce("Ownable_UpdateTint.Postfix", e);
                }
            }

            private static bool IsOwned(Ownable ownable) => ownable.assignee != null;
        }

        [HarmonyPatch(typeof(FilteredStorage), "OnFilterChanged")]
        public static class FilteredStorage_OnFilterChanged
        {
            public static void Postfix(KMonoBehaviour ___root, Tag[] tags)
            {
                try
                {
                    if (IsActive(tags))
                    {
                        Painter.ApplyColorToKAnimControllerBase(___root);
                    }
                }
                catch (Exception e)
                {
                    State.Common.Logger.LogOnce("FilteredStorage_OnFilterChanged.Postfix", e);
                }
            }

            private static bool IsActive(Tag[] tags)
                => tags != null && tags.Length != 0;
        }

        [HarmonyPatch(typeof(BlockTileRenderer), nameof(BlockTileRenderer.GetCellColour))]
        public static class BlockTileRenderer_GetCellColour
        {
            public static void Postfix(int cell, SimHashes element, BlockTileRenderer __instance, ref Color __result)
            {
                try
                {
                    var cellColor = State.TileColors[cell];
                    if
                    (
                        State.Config.Enabled &&
                        cellColor.HasValue
                    )
                    {
                        __result *= cellColor.Value;
                    }
                }
                catch (Exception e)
                {
                    State.Common.Logger.LogOnce("EnterCell failed.", e);
                }
            }
        }

        [HarmonyPatch(typeof(Deconstructable), "OnCompleteWork")]
        public static class Deconstructable_OnCompleteWork_MatCol
        {
            public static void Prefix(Deconstructable __instance)
            {
                try
                {
                    if (__instance.gameObject.TryGetComponent<KAnimGridTileVisualizer>(out _))
                    {
                        State.TileColors[__instance.GetCell()] = null;
                    }
                }
                catch (Exception e)
                {
                    State.Common.Logger.LogOnce(e);
                }
            }
        }
    }
}
using Harmony;
using MaterialColor.Extensions;
using MaterialColor.Helpers;
using Rendering;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using JetBrains.Annotations;

using UnityEngine;
using static KInputController;
using System.Reflection;
using MaterialColor.Data;
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
                    Color color = ColorHelper.GetComponentMaterialColor(__instance);
                    bool owned = __instance.assignee != null;

                    if (owned)
                    {
                        KAnimControllerBase animBase = __instance.GetComponent<KAnimControllerBase>();
                        if (animBase != null)
                        {
                            animBase.TintColour = color;
                        }
                    }
                }
                catch (Exception e)
                {
                    State.Common.Logger.LogOnce("Ownable_UpdateTint.Postfix", e);
                }
            }
        }

        [HarmonyPatch(typeof(FilteredStorage), "OnFilterChanged")]
        public static class FilteredStorage_OnFilterChanged
        {
            public static void Postfix(KMonoBehaviour ___root, Tag[] tags)
            {
                try
                {
                    Color color = ColorHelper.GetComponentMaterialColor(___root);
                    bool active = tags != null && tags.Length != 0;

                    if (active)
                    {
                        KAnimControllerBase animBase = ___root.GetComponent<KAnimControllerBase>();
                        if (animBase != null)
                        {
                            animBase.TintColour = color;
                        }
                    }
                }
                catch (Exception e)
                {
                    State.Common.Logger.LogOnce("FilteredStorage_OnFilterChanged.Postfix", e);
                }
            }
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
            public static void Postfix(Deconstructable __instance)
            {
                try
                {
                    var buildingComplete = __instance.GetComponent<BuildingComplete>();
                    var buildingName = buildingComplete.name.Replace("Complete", string.Empty);

                    if (State.TileNames.Contains(buildingName))
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
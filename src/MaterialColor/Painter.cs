using Harmony;
using MaterialColor.Extensions;
using MaterialColor.Helpers;
using MaterialColor.Patches;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace MaterialColor
{
    /// <summary>
    /// Provides main mod functionality - applying colors to buildings and tiles.
    /// </summary>
    public static class Painter
    {
        private static readonly List<Type> StorageTypes = new List<Type>
        {
            typeof(RationBox),
            typeof(Refrigerator),
            typeof(SolidConduitInbox),
            typeof(StorageLocker),
            typeof(TinkerStation)
        };

        private static readonly string ExcludeKeyword = "NoPaint";
        private static readonly Tag ExcludedTag = new Tag("NoPaint");

        public static void Refresh()
        {
            UpdateBuildingsColors();
            RebuildAllTiles();
        }

        public static void UpdateBuildingColor(Component component)
        {
            var buildingComplete = component.GetComponent<BuildingComplete>();

            if (buildingComplete != null)
            {
                UpdateBuildingColor(buildingComplete);
            }
            else
            {
                State.Common.Logger.LogOnce("Couldn't find BuildingComplete on component", component.name);
            }
        }

        public static void UpdateBuildingColor(BuildingComplete building)
        {
            if (building.name == "PixelPackComplete" || building.name == "WallpaperComplete" || building.name.Contains(ExcludeKeyword) || building.HasTag(ExcludedTag))
            {
                return;
            }

            Color color = ColorHelper.GetComponentMaterialColor(building);

            Filter(building.name, ref color);

            if (State.TileNames.Contains(building.name))
            {
                ApplyColorToTile(building, color);
            }
            else
            {
                ApplyColorToBuilding(building, color);
            }
        }

        private static FilteredStorage ExtractFilteredStorage(Component building)
        {
            foreach (Type storageType in StorageTypes)
            {
                Component comp = building.GetComponent(storageType);

                if (comp != null)
                {
                    return Traverse.Create(comp).Field<FilteredStorage>("filteredStorage").Value;
                }
            }
            return null;
        }

        private static void ApplyColorToBuilding(BuildingComplete building, Color color)
        {
            TreeFilterable treeFilterable;
            Ownable ownable;
            KAnimControllerBase kAnimBase;

            if ((ownable = building.GetComponent<Ownable>()) != null)
            {
                Traverse.Create(ownable).Field("ownedTint").SetValue(color);
                Traverse.Create(ownable).Method("UpdateTint").GetValue();
            }
            else if ((treeFilterable = building.GetComponent<TreeFilterable>()) != null)
            {
                FilteredStorage filteredStorage = ExtractFilteredStorage(treeFilterable);

                if (filteredStorage != null)
                {
                    filteredStorage.filterTint = color;
                    filteredStorage.FilterChanged();
                }
            }
            else if ((kAnimBase = building.GetComponent<KAnimControllerBase>()) != null)
            {
                kAnimBase.TintColour = color;
            }
            else
            {
                State.Common.Logger.LogOnce($"Invalid building <{building}> and its not a registered tile.");
            }
        }

        private static void ApplyColorToTile(BuildingComplete building, Color color)
        {
            try
            {
                State.TileColors[Grid.PosToCell(building.gameObject)] = color.ToTileColor();
            }
            catch (Exception e)
            {
                State.Common.Logger.LogOnce("Error while getting cell color", e);
            }
        }

        public static void ApplyColorToKAnimControllerBase(Component component)
        {
            KAnimControllerBase animBase = component.GetComponent<KAnimControllerBase>();

            if (animBase != null)
            {
                Color color = ColorHelper.GetComponentMaterialColor(component);

                Painter.Filter(animBase.name, ref color);

                animBase.TintColour = color;
            }
            else
            {
                State.Common.Logger.LogOnce("Failed to find KAnimControllerBase", component.name);
            }
        }

        private static void RebuildAllTiles()
        {
            for (int i = 0; i < Grid.CellCount; i++)
            {
                World.Instance.blockTileRenderer.Rebuild(ObjectLayer.FoundationTile, i);
            }

            State.Common.Logger.LogDebug("All tiles rebuilt.");
        }

        private static void UpdateBuildingsColors()
        {
            try
            {
                foreach (BuildingComplete building in Components.BuildingCompletes.Items)
                {
                    UpdateBuildingColor(building);
                }

                State.Common.Logger.Log(Components.BuildingCompletes.Count + " buildings updated successfully.");
            }
            catch (Exception e)
            {
                State.Common.Logger.Log("Buildings colors update failed.", e);
            }
        }

        public static void Filter(string buildingName, ref Color color)
        {
            try
            {
                buildingName = buildingName.Replace("Complete", string.Empty);

                if (State.TypeFilter != null)
                {
                    if (!State.TypeFilter.Check(buildingName))
                    {
                        color = ColorHelper.DefaultColor;
                    }
                }
                else
                {
                    State.Common.Logger.LogOnce("State.TypeFilter is null");
                }
            }
            catch (Exception e)
            {
                State.Common.Logger.LogOnce("Error while filtering for: " + buildingName, e);
            }
        }
    }
}

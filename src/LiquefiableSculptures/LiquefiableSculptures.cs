﻿using HarmonyLib;

namespace LiquefiableSculptures
{
    [HarmonyPatch(typeof(BuildingTemplates))]
    [HarmonyPatch(nameof(BuildingTemplates.CreateBuildingDef))]
    public static class LiquefiableSculptures
    {
        public static void Prefix(string id, ref string[] construction_materials)
        {
            if (id == IceSculptureConfig.ID)
            {
                construction_materials = new string[]
                {
                    nameof(STRINGS.MISC.TAGS.LIQUIFIABLE)
                };
            }
        }
    }
}

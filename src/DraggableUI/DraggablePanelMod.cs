using HarmonyLib;
using System;

namespace DraggablePanelMod
{
    [HarmonyPatch(typeof(KScreen), "OnPrefabInit")]
    public static class DraggablePanelModInit
    {
        public static void Prefix(KScreen __instance)
        {
            DraggablePanel.Attach(__instance);
        }
    }

    [HarmonyPatch(typeof(KScreen), "OnSpawn")]
    public static class DraggablePanelModSpawn
    {
        public static void Postfix(KScreen __instance)
        {
            DraggablePanel.SetPositionFromFile(__instance);
        }
    }
}


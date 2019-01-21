using Harmony;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace ClearRegolith
{
    [HarmonyPatch(typeof(Storage))]
    [HarmonyPatch("OnSpawn")]
    public static class ClearRegolith
    {
        public static void Postfix(Storage __instance)
        {
            foreach (GameObject gameObject in __instance.items)
            {
                PrimaryElement element = gameObject.GetComponent<PrimaryElement>();

                if (element != null && element.ElementID == SimHashes.Regolith)
                {
                    element.Units /= 4;
                }
            }
        } 
    }
}

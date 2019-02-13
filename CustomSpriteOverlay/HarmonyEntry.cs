using Harmony;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace CustomSpriteOverlay
{
    public static class HarmonyEntry
    {
        //[HarmonyPatch(typeof(CameraController), "Update")]
        public static class CameraController_Update
        {
            public static void Postfix(CameraController __instance)
            {
                //__instance.
            }
        }

        [HarmonyPatch(typeof(PopFXManager), "SpawnFX")]
        public static class PopFXManager_SpawnFX
        {
            public static void Postfix(PopFXManager __instance)
            {
                GameObject gameObject = Util.KInstantiate(__instance.Prefab_PopFX, __instance.gameObject, "Pooled_PopFX");
                //gameObject.transform.localScale = Vector3.one;

                foreach (var comp in gameObject.GetComponents<Component>())
                {
                    Debug.Log(comp.name);
                }

                Debug.Log("===============\n");
            }
        }
    }
}

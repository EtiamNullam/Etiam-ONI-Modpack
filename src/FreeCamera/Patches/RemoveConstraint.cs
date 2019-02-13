using Harmony;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FreeCamera.Patches
{
    public static class RemoveConstraint
    {
        /// <summary>
        /// Stop constraining camera to the world.
        /// </summary>
        [HarmonyPatch(typeof(CameraController))]
        [HarmonyPatch("ConstrainToWorld")]
        public static class FreeTheCamera
        {
            public static bool Prefix()
            {
                return false;
            }
        }
    }
}

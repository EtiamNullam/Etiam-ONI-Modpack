using Harmony;

namespace FreeCamera
{
    public static class FreeCamera
    {
        private const float MaxZoom = 350f;

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

        [HarmonyPatch(typeof(CameraController))]
        [HarmonyPatch("OnPrefabInit")]
        public static class CameraController_OnPrefabInit_SetMaxZoom
        {
            public static void Postfix(ref float ___maxOrthographicSize)
            {
                ___maxOrthographicSize = MaxZoom;
            }
        }

        [HarmonyPatch(typeof(WattsonMessage))]
        [HarmonyPatch("OnDeactivate")]
        public static class WattsonMessage_OnDeactivate_SetMaxZoom_AfterDelay
        {
            public static void Postfix()
            {
                UIScheduler.Instance.Schedule
                (
                    "setZoomAfter_fadeInUI",
                    3,
                    data => CameraController.Instance.SetMaxOrthographicSize(MaxZoom)
                );
            }
        }
    }
}

using Harmony;

namespace FreeCamera.Patches
{
    [HarmonyPatch(typeof(CameraController))]
    [HarmonyPatch("OnPrefabInit")]
    public static class CameraController_OnPrefabInit_SetMaxZoom
    {
        public static void Postfix(ref float ___maxOrthographicSize)
        {
            ___maxOrthographicSize = Config.MaxZoom;
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
                data => CameraController.Instance.SetMaxOrthographicSize(Config.MaxZoom)
            );
        }
    }
}

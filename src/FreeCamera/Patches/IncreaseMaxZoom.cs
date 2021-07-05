﻿using HarmonyLib;
using System;

namespace FreeCamera.Patches
{
    [HarmonyPatch(typeof(CameraController))]
    [HarmonyPatch("OnPrefabInit")]
    public static class CameraController_OnPrefabInit_SetMaxZoom
    {
        public static void Postfix()
        {
            CameraController.Instance.SetMaxOrthographicSize(Config.MaxZoom);
        }
    }

    [HarmonyPatch(typeof(NewBaseScreen))]
    [HarmonyPatch(nameof(NewBaseScreen.SetInitialCamera))]
    public static class NewBaseScreen_SetInitialCamera_SetMaxZoom
    {
        public static void Postfix()
        {
            CameraController.Instance.SetMaxOrthographicSize(Config.MaxZoom);
        }
    }

    [HarmonyPatch(typeof(WattsonMessage))]
    [HarmonyPatch("OnDeactivate")]
    public static class WattsonMessage_OnDeactivate_SetMaxZoom_AfterDelay
    {
        public static void Postfix()
        {
            try
            {
                UIScheduler.Instance.Schedule
                (
                    "setZoomAfter_fadeInUI",
                    3,
                    data => CameraController.Instance.SetMaxOrthographicSize(Config.MaxZoom)
                );
            }
            catch (Exception e)
            {
                Debug.Log("FreeCamera: Failed to schedule a change of max camera zoom after world start." + e);
            }
        }
    }
}

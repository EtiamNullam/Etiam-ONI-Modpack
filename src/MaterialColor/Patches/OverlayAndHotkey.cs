using Harmony;
using MaterialColor.IO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;
using static KInputController;

namespace MaterialColor.Patches
{
    public static class OverlayAndHotkey
    {
        // TODO: still needs rework
        [HarmonyPatch(typeof(Global), "GenerateDefaultBindings")]
        public static class Global_GenerateDefaultBindings
        {
            public static void Postfix(ref BindingEntry[] __result)
            {
                try
                {
                    if (State.Config.LogElementsData)
                    {
                        var values = Enum.GetNames(typeof(SimHashes));
                        Array.Sort(values);
                        string elementsLog = "";
                        foreach (var name in values)
                        {
                            elementsLog += Environment.NewLine + name;
                        }
                        State.Common.Logger.Log("Elements list: " + elementsLog);
                    }

                    try
                    {
                        List<BindingEntry> bind = __result.ToList();
                        BindingEntry entry = new BindingEntry
                        (
                            "Root",
                            GamepadButton.NumButtons,
                            KKeyCode.F6,
                            Modifier.Alt,
                            (Action)IDs.ToggleMaterialColorOverlayAction,
                            true,
                            true
                        );
                        bind.Add(entry);
                        __result = bind.ToArray();
                    }
                    catch (Exception e)
                    {
                        State.Common.Logger.LogOnce("Keybindings failed:\n", e);
                        throw;
                    }
                }
                catch (Exception e)
                {
                    State.Common.Logger.LogOnce("Global_GenerateDefaultBindings.Postfix", e);
                }
            }
        }

        // TODO: still needs rework
        [HarmonyPatch(typeof(OverlayMenu), "InitializeToggles")]
        public static class OverlayMenu_InitializeToggles
        {
            // TODO: read from file instead
            public static void Postfix(OverlayMenu __instance)
            {
                try
                {
                    FieldInfo overlayToggleInfosFI = AccessTools.Field(typeof(OverlayMenu), "overlayToggleInfos");
                    var overlayToggleInfos = (List<KIconToggleMenu.ToggleInfo>)overlayToggleInfosFI.GetValue(__instance);

                    Type oti = AccessTools.Inner(typeof(OverlayMenu), "OverlayToggleInfo");

                    ConstructorInfo ci = oti.GetConstructor(new Type[] { typeof(string), typeof(string), typeof(HashedString), typeof(string), typeof(Action), typeof(string), typeof(string) });
                    object ooti = ci.Invoke(new object[] {
                        "Toggle MaterialColor",
                        "overlay_materialcolor",
                        IDs.MaterialColorOverlayHS,
                        string.Empty,
                        (Action)IDs.ToggleMaterialColorOverlayAction,
                        "Toggles MaterialColor overlay",
                        "MaterialColor"
                });
                    ((KIconToggleMenu.ToggleInfo)ooti).getSpriteCB = GetUISprite;

                    overlayToggleInfos.Add((KIconToggleMenu.ToggleInfo)ooti);

                    /*
                    __result.Add(
                                 new OverlayMenu.OverlayToggleInfo(
                                                                   "Toggle MaterialColor",
                                                                   "overlay_materialcolor",
                                                                   (SimViewMode)IDs.ToggleMaterialColorOverlayID,
                                                                   string.Empty,
                                                                   (Action)IDs.ToggleMaterialColorOverlayAction,
                                                                   "Toggles MaterialColor overlay",
                                                                   "MaterialColor") {
                                                                                       getSpriteCB = () => GetUISprite()
                                                                                    });
                    */
                }
                catch (Exception e)
                {
                    State.Common.Logger.LogOnce("OverlayMenu_InitializeToggles.Postfix: Icon set error", e);
                }
			}

            // TODO: extract
            private static Sprite GetUISprite()
            {
                try
                {
                    var width = 256;
                    var height = 256;

                    byte[] bytes = File.ReadAllBytes(Paths.IconPath);

                    Texture2D texture = new Texture2D(width, height, TextureFormat.RGB24, false)
                    {
                        filterMode = FilterMode.Trilinear
                    };
                    ImageConversion.LoadImage(texture, bytes);

                    return Sprite.Create(texture, new Rect(0, 0, width, height), new Vector2(0.5f, 0.0f), 1.0f);
                }
                catch (Exception e)
                {
                    State.Common.Logger.LogOnce("GetUISprite failed", e);

                    // TODO: set some placeholder sprite here
                    return null;
                }
            }
        }

        [HarmonyPatch(typeof(OverlayMenu), "OnOverlayChanged")]
        public static class OverlayMenu_OnOverlayChanged_OverlayChangedEntry
        {
            public static void Prefix()
            {
                try
                {
                    var currentOverlayMode = OverlayScreen.Instance.GetMode();
                    if (OverlayModes.Power.ID.Equals(currentOverlayMode) ||
                        OverlayModes.GasConduits.ID.Equals(currentOverlayMode) ||
                        OverlayModes.LiquidConduits.ID.Equals(currentOverlayMode) ||
                        OverlayModes.Logic.ID.Equals(currentOverlayMode)
                        )
                    { 
                        Painter.Refresh();
                    }
                }
                catch (Exception e)
                {
                    State.Common.Logger.LogOnce("OverlayChangedEntry.Prefix failed", e);
                }
            }
        }

		[HarmonyPatch(typeof(KeyDef), MethodType.Constructor)]
		[HarmonyPatch(new Type[] {typeof(KKeyCode), typeof(Modifier) })]
		public static class KeyDef_Constructor
		{
            // ReSharper disable once InconsistentNaming
            public static void Postfix(KeyDef __instance)
            {
                try
                {
                    __instance.mActionFlags = new bool[1000];
                }
                catch (Exception e)
                {
                    State.Common.Logger.LogOnce("KeyDef_Constructor.Postfix failed", e);
                }
            }
        }

        [HarmonyPatch(typeof(KInputController), MethodType.Constructor)]
		[HarmonyPatch(new Type[] { typeof(bool) })]
		public static class KInputController_Constructor
		{
			[HarmonyPostfix]
			// ReSharper disable once InconsistentNaming
			public static void KInputControllerMod(ref bool[] ___mActionState)
			{
                try
                {
                    ___mActionState = new bool[1000];
                }
                catch (Exception e)
                {
                    State.Common.Logger.LogOnce("KInputController_Constructor.KInputControllerMod failed", e);
                }
			}
		}

		[HarmonyPatch(typeof(OverlayMenu), "OnToggleSelect")]
        public static class OverlayMenu_OnToggleSelect_MatCol
        {
            [HarmonyPrefix]
            // ReSharper disable once InconsistentNaming
            public static bool EnterToggle(OverlayMenu __instance, KIconToggleMenu.ToggleInfo toggle_info)
            {
                try
                {
					bool toggleMaterialColor = Traverse.Create(toggle_info).Field<HashedString>("simView").Value
                                            == IDs.MaterialColorOverlayHS;

                    if (!toggleMaterialColor)
                    {
                        return true;
                    }

                    State.Config.Enabled = !State.Config.Enabled;

                    Painter.Refresh();

                    return false;
                }
                catch (Exception e)
                {
                    State.Common.Logger.LogOnce("EnterToggle failed.", e);
                    return true;
                }
            }
        }
    }
}

using Common;
using Harmony;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace DisplayAllTemps.Patches
{
    public static class Settings
    {
        [HarmonyPatch(typeof(UnitConfigurationScreen))]
        [HarmonyPatch("DisplayCurrentUnit")]
        public static class UnitConfigurationScreen_DisplayCurrentUnit
        {
            public static bool Prefix(GameObject ___celsiusToggle, GameObject ___kelvinToggle, GameObject ___fahrenheitToggle)
            {
                ___celsiusToggle.GetComponent<HierarchyReferences>().GetReference("Checkmark").gameObject.SetActive((State.Unit & TemperatureUnitMultiple.Celsius) != 0);
                ___kelvinToggle.GetComponent<HierarchyReferences>().GetReference("Checkmark").gameObject.SetActive((State.Unit & TemperatureUnitMultiple.Kelvin) != 0);
                ___fahrenheitToggle.GetComponent<HierarchyReferences>().GetReference("Checkmark").gameObject.SetActive((State.Unit & TemperatureUnitMultiple.Fahrenheit) != 0);

                return false;
            }
        }

        [HarmonyPatch(typeof(UnitConfigurationScreen))]
        [HarmonyPatch("OnCelsiusClicked")]
        public static class UnitConfigurationScreen_OnCelsiusClicked
        {
            public static void Prefix()
            {
                OnClicked_Prefix(GameUtil.TemperatureUnit.Celsius);
            }

            public static void Postfix()
            {
                OnClicked_Postfix(GameUtil.TemperatureUnit.Celsius);
            }
        }

        [HarmonyPatch(typeof(UnitConfigurationScreen))]
        [HarmonyPatch("OnFahrenheitClicked")]
        public static class UnitConfigurationScreen_OnFahrenheitClicked
        {
            public static void Prefix()
            {
                OnClicked_Prefix(GameUtil.TemperatureUnit.Fahrenheit);
            }

            public static void Postfix()
            {
                OnClicked_Postfix(GameUtil.TemperatureUnit.Fahrenheit);
            }
        }

        [HarmonyPatch(typeof(UnitConfigurationScreen))]
        [HarmonyPatch("OnKelvinClicked")]
        public static class UnitConfigurationScreen_OnKelvinClicked
        {
            public static void Prefix()
            {
                OnClicked_Prefix(GameUtil.TemperatureUnit.Kelvin);
            }

            public static void Postfix()
            {
                OnClicked_Postfix(GameUtil.TemperatureUnit.Kelvin);
            }
        }

        private static void OnClicked_Prefix(GameUtil.TemperatureUnit unit)
        {
            var multipleUnit = VanillaToMultiple(unit);

            if ((State.Unit ^ multipleUnit) != 0)
            {
                FlipUnit(multipleUnit);
            }

            Save();
        }

        private static void OnClicked_Postfix(GameUtil.TemperatureUnit unit)
        {
            var multipleUnit = VanillaToMultiple(unit);

            if ((State.Unit & multipleUnit) == 0)
            {
                GameUtil.temperatureUnit = State.LastMainUnit;
            }
            else
            {
                State.LastMainUnit = unit;
            }
        }

        private static void FlipUnit(TemperatureUnitMultiple unit)
        {
            try
            {
                if (State.Unit == unit)
                {
                    return;
                }

                State.Unit ^= unit;
            }
            catch (Exception e)
            {
                State.Common.Logger.Log("Flip unit failed.", e);
            }

            return;
        }

        private static GameUtil.TemperatureUnit MultipleToVanilla(TemperatureUnitMultiple multiple)
        {
            return (State.Unit & TemperatureUnitMultiple.Celsius) != 0
               ? GameUtil.TemperatureUnit.Celsius
               : (State.Unit & TemperatureUnitMultiple.Fahrenheit) != 0
                   ? GameUtil.TemperatureUnit.Fahrenheit
                   : GameUtil.TemperatureUnit.Kelvin;
        }

        private static TemperatureUnitMultiple VanillaToMultiple(GameUtil.TemperatureUnit vanilla)
        {
            switch(vanilla)
            {
                case GameUtil.TemperatureUnit.Celsius:
                    return TemperatureUnitMultiple.Celsius;
                case GameUtil.TemperatureUnit.Kelvin:
                    return TemperatureUnitMultiple.Kelvin;
                case GameUtil.TemperatureUnit.Fahrenheit:
                    return TemperatureUnitMultiple.Fahrenheit;
                default:
                    State.Common.Logger.LogOnce("VanillaToMultiple: invalid unit");
                    return TemperatureUnitMultiple.All;
            }
        }

        private static void Save()
        {
            try
            {
                if (State.Common.ConfigPath == null)
                {
                    return;
                }

                State.Common.SaveConfig(State.ConfigFileName, State.Unit);
            }
            catch (Exception e)
            {
                State.Common.Logger.Log("Failed to save unit to file.", e);
            }
        }
    }
}

using Harmony;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RemoveDailyReports.Patches
{
    [HarmonyPatch(typeof(ReportScreen))]
    [HarmonyPatch(nameof(ReportScreen.ShowReport))]
    public static class ReportScreen_ShowReport_Disable
    {
        public static bool Prefix()
        {
            return false;
        }
    }

    [HarmonyPatch(typeof(ReportScreen))]
    [HarmonyPatch("Refresh")]
    public static class ReportScreen_Refresh_Disable
    {
        public static bool Prefix()
        {
            return false;
        }
    }
}

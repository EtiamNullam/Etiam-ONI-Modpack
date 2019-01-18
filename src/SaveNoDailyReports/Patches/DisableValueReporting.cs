using Harmony;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RemoveDailyReports.Patches
{
    [HarmonyPatch(typeof(ReportManager))]
    [HarmonyPatch(nameof(ReportManager.ReportValue))]
    public static class ReportManager_ReportValue_Disable
    {
        public static bool Prefix()
        {
            return false;
        }
    }
}

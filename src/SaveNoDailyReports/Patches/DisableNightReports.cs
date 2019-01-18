using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Harmony;

namespace RemoveDailyReports.Patches
{
    [HarmonyPatch(typeof(ReportManager))]
    [HarmonyPatch("OnNightTime")]
    public static class ReportManager_OnNightTime_ClearDailyReports
    {
        public static void Postfix(ref List<ReportManager.DailyReport> ___dailyReports)
        {
            ___dailyReports.Clear();
        }
    }
}

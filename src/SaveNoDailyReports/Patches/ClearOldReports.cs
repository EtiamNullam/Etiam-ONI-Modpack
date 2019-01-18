using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Harmony;

namespace RemoveDailyReports.Patches
{
    [HarmonyPatch(typeof(ReportManager))]
    [HarmonyPatch("OnNightTime")]
    public static class ClearOldReports_AtNight
    {
        public static void Postfix(ref List<ReportManager.DailyReport> ___dailyReports)
        {
            try
            {
                int recentReportsToSpare = 5;
                int dailyReportsCount = ___dailyReports.Count();

                if (dailyReportsCount > recentReportsToSpare)
                {
                    var reportsToClear = ___dailyReports.Take(dailyReportsCount - recentReportsToSpare);
                    foreach (var report in reportsToClear)
                    {
                        report.reportEntries.Clear();
                    }
                }
            }
            catch (Exception e)
            {
                Debug.Log("Error while clearing old reports at night: " + e);
            }
        }
    }
}

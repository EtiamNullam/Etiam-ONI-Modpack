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
        private static readonly int ToSpare = 10;

        public static void Postfix(ref List<ReportManager.DailyReport> ___dailyReports)
        {
            try
            {
                int reportsCount = ___dailyReports.Count;

                if (reportsCount > ToSpare)
                {
                    ___dailyReports
                        .Take(reportsCount - ToSpare)
                        .Do(report => report.reportEntries.Clear());
                }
            }
            catch (Exception e)
            {
                Debug.Log("RDR: Error while clearing old reports at night: " + e);
            }
        }
    }
}

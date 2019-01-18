using Harmony;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RemoveDailyReports.Patches
{
    // TODO: extract it to seperate mod?
    public static class FixMissingReports
    {
        [HarmonyPatch(typeof(ReportManager))]
        [HarmonyPatch("OnDeserialized")]
        public static class ReportManager_FixMissingReports
        {
            public static void Postfix(ReportManager __instance, ref List<ReportManager.DailyReport> ___dailyReports)
            {
                try
                {
                    var missingCycles = GameUtil.GetCurrentCycle() - ___dailyReports.Count();

                    if (missingCycles > 0)
                    {
                        var missingDailyReports = new ReportManager.DailyReport[missingCycles];

                        for (int i = 0; i < missingCycles; i++)
                        {
                            var newDailyReport = new ReportManager.DailyReport(__instance)
                            {
                                day = i
                            };
                            newDailyReport.reportEntries.Clear();
                            missingDailyReports[i] = newDailyReport;

                        }
                        ___dailyReports.InsertRange(0, missingDailyReports);
                    }
                }
                catch (Exception e)
                {
                    Debug.Log("RDR: Error while trying to fix missing daily reports: " + e);
                }
            }
        }
    }
}

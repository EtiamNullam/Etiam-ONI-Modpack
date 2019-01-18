using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Harmony;

namespace SaveNoDailyReports
{
    //[HarmonyPatch(typeof(ReportManager.DailyReport))]
    //[HarmonyPatch(MethodType.Constructor)]
    //[HarmonyPatch(nameof(ReportManager.DailyReport.AddData))]

    //[HarmonyPatch(typeof(ReportManager.NoteStorage))]
    //[HarmonyPatch(nameof(ReportManager.NoteStorage.Serialize))]
    //public static class SaveNoDailyReports
    //{
    //    private static int accessCount = 0;
    //    //public static bool Prefix(ReportManager.DailyReport __instance)
    //    public static bool Prefix(ReportManager.NoteStorage __instance)
    //    {
    //        return false;
    //        //__instance.
    //        //Debug.Log("hit");
    //        try
    //        {
    //            //Debug.Log("SaveNoDailyReportsMod: previous count: " + __instance.reportEntries.Count());
    //            //__instance.reportEntries.RemoveRange(0, __instance.reportEntries.Count() - 1);
    //            //Debug.Log("SaveNoDailyReportsMod: previous count: " + __instance.no.Count());
    //            //__instance.reportEntries.RemoveRange(0, __instance.reportEntries.Count() - 1);
    //            //Debug.Log("SaveNoDailyReportsMod: access count: " + ++accessCount);
    //        }
    //        catch (Exception e)
    //        {
    //            Debug.Log("SaveNoDailyReportsMod: " + e);
    //        }
    //        return false;
    //    }
    //}

    //[HarmonyPatch(typeof(ReportManager.NoteStorage))]
    //[HarmonyPatch(nameof(ReportManager.NoteStorage.Deserialize))]
    //public static class SaveNoDailyReports_temp
    //{
    //    public static bool Prefix(ReportManager.NoteStorage __instance)//, ___noteEntries, ReportManager.NoteStorage.st ___stringTable)
    //    {
    //        return false;
    //    }
    //}
    [HarmonyPatch(typeof(ReportManager))]
    [HarmonyPatch("OnNightTime")]
    public static class ReportManager_OnNightTime_ClearDailyReports
    {
        public static bool Prefix(ref List<ReportManager.DailyReport> ___dailyReports)//, ___noteEntries, ReportManager.NoteStorage.st ___stringTable)
        {
            ___dailyReports.RemoveRange(0, ___dailyReports.Count() - 1);

            return false;
        }
    }

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

    // TODO: block all reporting attempts in their root
}

namespace KiotVietTimeSheet.BackgroundTasks.BackgroundProcess.Common
{
    public static class EventTypeStatic
    {
        public const string CreateDraftPaySheetIntegrationSocket = "CreateDraftPaySheetIntegrationSocket";
        public const string AutoLoadingAndUpdatePaySheetIntegrationSocket = "AutoLoadingAndUpdatePaySheetIntegrationSocket";
        public const string PaySheetEmptyPendingIntegrationSocket = "PaySheetEmptyPendingIntegrationSocket";
        public const string PaySheetPendingIntegrationSocket = "PaySheetPendingIntegrationSocket";
        public const string UpdateClockingTimeSocket = "UpdateClockingTimeSocket";

        public const string CreateConfirmClockingIntegrationSocket = "CreateConfirmClockingIntegrationSocket";
        public const int AutoKeepingJob = 1;
    }
}

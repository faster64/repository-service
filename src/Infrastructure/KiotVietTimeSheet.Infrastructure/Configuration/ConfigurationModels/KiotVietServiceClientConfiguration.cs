namespace KiotVietTimeSheet.Infrastructure.Configuration.ConfigurationModels
{
    public class KiotVietServiceClientConfiguration
    {
        #region Booking

        public string BookingEndPoint { get; set; }
        public int[] BookingGroupIds { get; set; }
        public string BookingInternalToken { get; set; }
        public string TimeSheetBookingInternalToken { get; set; }
        public string InternalIpWhitelist { get; set; }

        #endregion Booking

        #region Fnb

        public string FnbEndPoint { get; set; }
        public int[] FnbGroupIds { get; set; }
        public string FnBInternalToken { get; set; }

        #endregion Fnb

        #region Retail

        public string RetailEndPoint { get; set; }
        public int[] RetailGroupIds { get; set; }
        public string RetailerInternalToken { get; set; }

        #endregion Retail

        public string KvVersion { get; set; }
    }
}
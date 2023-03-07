using KiotVietTimeSheet.Application.Dto;
using ServiceStack;

namespace KiotVietTimeSheet.Api.ServiceModel
{
    [Route("/gps-timekeeping/login", "POST", Summary = "GPS timekeeping login API",
    Notes = "")]
    public class GpsTimekeepingLoginReq : IReturn<object>
    {
        public int TenantId { get; set; }
        public int BranchId { get; set; }
        public string IdentityKeyClocking { get; set; }
        public string EmployeeCode { get; set; }
        public string EmployeePhone { get; set; }
        public bool IsPhone { get; set; }
    }


    [Route("/clocking-gps/get-clockings", "GET", Summary = "Lấy danh sách ca chấm công", Notes = "")]
    public class GetClockingsForClockingGpsReq : QueryDb<ClockingGpsDto>, IReturn<ClockingGpsDto>
    {
        public int BranchId { get; set; }
        public long EmployeeId { get; set; }
    }

    [Route("/clocking-gps/get-employee", "POST", Summary = "Lấy danh sách ca chấm công", Notes = "")]
    public class GetEmployeeForClockingGpsReq : QueryDb<EmployeeDto>, IReturn<object>
    {
        public string IdentityKeyClocking { get; set; }
        public string Keyword { get; set; }
        public bool IsPhone { get; set; }
        public string Os { get; set; }
        public string OsVersion { get; set; }
        public string Vendor { get; set; }
        public string Model { get; set; }
        public string Type { get; set; }
    }

    [Route("/clocking-gps/update-device", "PUT", Summary = "Update device id", Notes = "")]
    public class UpdateDeviceForEmployeeClockingGpsReq : IReturn<object>
    {
        public long EmployeeId { get; set; }
        public string VerifyCode { get; set; }
        public string Os { get; set; }
        public string OsVersion { get; set; }
        public string Type { get; set; }
    }

    [Route("/clocking-gps/update-clocking", "PUT", Summary = "Chấm công Gps", Notes = "")]
    public class UpdateClockingForClockingGpsReq : QueryDb<EmployeeDto>, IReturn<EmployeeDto>
    {
        public long Id { get; set; }
        public ClockingDto Clocking { get; set; }
        public ClockingHistoryDto ClockingHistory { get; set; }
        public string GeoCoordinate { get; set; }
        public string IdentityKeyClocking { get; set; }
        public bool AcceptWrongGps { get; set; }
    }

    [Route("/clocking-gps/get-employee-by-identity-key", "GET", Summary = "Lấy thông tin nhân viên bằng mã thiết bị", Notes = "")]
    public class GetEmployeeByIdentityKeyForClockingGpsReq : QueryDb<EmployeeDto>, IReturn<object>
    {
        public string IdentityKeyClocking { get; set; }
    }

    [Route("/clocking-gps/getcurrenttimeserver", "GET")]
    public class GetCurrentTimeServer : IReturn<object>
    {

    }

    [Route("/clocking-gps/get-gps-info", "GET", Summary = "Get Gps Info", Notes = "")]
    public class GetGpsInfoForClockingGpsReq : IReturn<object>
    {
        public int BranchId { get; set; }
    }
}


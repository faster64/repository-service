using System;
using System.Collections.Generic;
using KiotVietTimeSheet.Application.Abstractions;
using KiotVietTimeSheet.Application.Auth;
using KiotVietTimeSheet.Application.Auth.Common;
using KiotVietTimeSheet.Application.Dto;

namespace KiotVietTimeSheet.Application.Queries.GetDraftPaysheet
{
    [RequiredPermission(TimeSheetPermission.Paysheet_Read)]
    public class GetDraftPaysheetQuery : QueryBase<PaysheetDto>
    {
        public byte SalaryPeriod { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public List<long> EmployeeIds { get; set; }
        public int BranchId { get; set; }
        public long PaysheetId { get; set; }
        public int? WorkingDayNumber { get; set; }

        public GetDraftPaysheetQuery(byte salaryPeriod, DateTime startTime, DateTime endTime, List<long> employeeIds, int branchId, long paysheetId, int? workingDayNumber)
        {
            SalaryPeriod = salaryPeriod;
            StartTime = startTime;
            EndTime = endTime;
            EmployeeIds = employeeIds;
            BranchId = branchId;
            PaysheetId = paysheetId;
            WorkingDayNumber = workingDayNumber;
        }
    }
}

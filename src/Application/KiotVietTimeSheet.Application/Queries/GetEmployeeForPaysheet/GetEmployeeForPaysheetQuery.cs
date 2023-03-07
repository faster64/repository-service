using System;
using KiotVietTimeSheet.Application.Abstractions;
using KiotVietTimeSheet.Application.Auth;
using KiotVietTimeSheet.Application.Auth.Common;
using KiotVietTimeSheet.Application.Dto;
using KiotVietTimeSheet.Domain.AggregatesModels.PaysheetAggregate.Enum;
using KiotVietTimeSheet.SharedKernel.Models;

namespace KiotVietTimeSheet.Application.Queries.GetEmployeeForPaysheet
{
    [RequiredPermission(TimeSheetPermission.Employee_Read)]
    public class GetEmployeeForPaysheetQuery : QueryBase<PagingDataSource<EmployeeDto>>
    {
        public PaySheetWorkingPeriodStatuses SalaryPeriod { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public int BranchId { get; set; }
        public string KeyWord { get; set; }
        public GetEmployeeForPaysheetQuery(
            PaySheetWorkingPeriodStatuses salaryPeriod,
            DateTime startTime,
            DateTime endTime,
            int branchId,
            string keyWord)
        {
            SalaryPeriod = salaryPeriod;
            StartTime = startTime;
            EndTime = endTime;
            BranchId = branchId;
            KeyWord = keyWord;
        }
    }
}

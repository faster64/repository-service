using KiotVietTimeSheet.Application.Abstractions;
using KiotVietTimeSheet.Application.Dto;
using KiotVietTimeSheet.SharedKernel.Models;
using ServiceStack.OrmLite;
using System.Collections.Generic;
using KiotVietTimeSheet.Application.Auth;
using KiotVietTimeSheet.Application.Auth.Common;

namespace KiotVietTimeSheet.Application.Queries.GetClockingsForCalendar
{
    [RequiredPermission(TimeSheetPermission.Clocking_Read)]
    public class GetClockingsForCalendarQuery : QueryBase<PagingDataSource<ClockingDto>>
    {
        public ISqlExpression Query { get; set; }
        public int BranchId { get; set; }
        public List<byte> ClockingHistoryStates { get; set; }
        public List<long> DepartmentIds { get; set; }

        public GetClockingsForCalendarQuery(ISqlExpression query, int branchId, List<byte> clockingHistoryStates, List<long> departmentIds)
        {
            Query = query;
            BranchId = branchId;
            ClockingHistoryStates = clockingHistoryStates;
            DepartmentIds = departmentIds;
        }
    }
}

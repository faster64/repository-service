using System;
using System.Collections.Generic;
using KiotVietTimeSheet.Domain.AggregatesModels.PaysheetAggregate.Models;
using KiotVietTimeSheet.SharedKernel.QueryFilter;

namespace KiotVietTimeSheet.Application.AppQueries.QueryFilters
{
    public class PaySheetQueryFilter : QueryFilterBase<Paysheet>
    {
        public string PaysheetKeyword { get; set; }
        public List<long> PaySheetIds { get; set; }
        public List<int?> BranchIds { get; set; }
        public List<byte> PaysheetStatuses { get; set; }
        public byte? SalaryPeriod { get; set; }
        public string EmployeeKeyword { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
    }
}

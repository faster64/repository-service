using System.Collections.Generic;
using KiotVietTimeSheet.Domain.AggregatesModels.PayslipAggregate.Models;
using KiotVietTimeSheet.SharedKernel.QueryFilter;

namespace KiotVietTimeSheet.Application.AppQueries.QueryFilters
{
    public class PayslipByPaysheetIdFilter : QueryFilterBase<Payslip>
    {
        public List<byte> PayslipStatuses { get; set; }
        public long? PaysheetId { get; set; }
        public long? EmployeeId { get; set; }
        public List<long> PayslipIds { get; set; }
    }
}

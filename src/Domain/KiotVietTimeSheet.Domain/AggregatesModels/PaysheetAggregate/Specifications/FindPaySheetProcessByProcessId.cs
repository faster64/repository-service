
using KiotVietTimeSheet.Domain.AggregatesModels.PaysheetAggregate.Models;
using KiotVietTimeSheet.SharedKernel.Specifications;

namespace KiotVietTimeSheet.Domain.AggregatesModels.PaysheetAggregate.Specifications
{
    public class FindPaySheetProcessByProcessId : ExpressionSpecification<PaysheetProcess>
    {
        public FindPaySheetProcessByProcessId(string processId)
            : base(e => e.ProcessId == processId)
        { }
    }
}

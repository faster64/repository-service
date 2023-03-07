using KiotVietTimeSheet.Domain.AggregatesModels.PaysheetAggregate.Models;
using KiotVietTimeSheet.SharedKernel.Specifications;

namespace KiotVietTimeSheet.Domain.AggregatesModels.PaysheetAggregate.Specifications
{
    public class FindPaysheetByNameOrCode : ExpressionSpecification<Paysheet>
    {
        public FindPaysheetByNameOrCode(string paysheetKeyword)
            : base(p => p.Code.Contains(paysheetKeyword) || p.Name.Contains(paysheetKeyword))
        { }
    }
}

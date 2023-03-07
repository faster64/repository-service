using KiotVietTimeSheet.Domain.AggregatesModels.FingerPrintAggregate.Models;
using KiotVietTimeSheet.SharedKernel.Specifications;

namespace KiotVietTimeSheet.Domain.AggregatesModels.FingerPrintAggregate.Specifications
{
    public class FindFingerPrintByFingerCodeSpec : ExpressionSpecification<FingerPrint>
    {
        public FindFingerPrintByFingerCodeSpec(string fingerCode)
            : base(x => x.FingerCode == fingerCode)
        {
        }
    }
}

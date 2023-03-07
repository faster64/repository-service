using System.Collections.Generic;
using KiotVietTimeSheet.Domain.AggregatesModels.FingerPrintAggregate.Models;
using KiotVietTimeSheet.SharedKernel.Specifications;

namespace KiotVietTimeSheet.Domain.AggregatesModels.FingerPrintAggregate.Specifications
{
    public class GetAllFingerPrintByFingerCodesSpec : ExpressionSpecification<FingerPrint>
    {
        public GetAllFingerPrintByFingerCodesSpec(List<string> fingerCodeList)
            : base(x => fingerCodeList.Contains(x.FingerCode))
        {
        }
    }
}

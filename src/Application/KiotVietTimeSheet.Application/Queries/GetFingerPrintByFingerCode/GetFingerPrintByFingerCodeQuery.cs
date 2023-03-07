using KiotVietTimeSheet.Application.Abstractions;
using KiotVietTimeSheet.Application.Dto;

namespace KiotVietTimeSheet.Application.Queries.GetFingerPrintByFingerCode
{
    public class GetFingerPrintByFingerCodeQuery : QueryBase<FingerPrintDto>
    {
        public string FingerCode { get; set; }
        public int BranchId { get; set; }

        public GetFingerPrintByFingerCodeQuery(string fingerCode, int branchId)
        {
            FingerCode = fingerCode;
            BranchId = branchId;
        }
    }
}

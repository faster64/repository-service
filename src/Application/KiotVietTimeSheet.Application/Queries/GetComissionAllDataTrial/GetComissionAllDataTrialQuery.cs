using KiotVietTimeSheet.Application.Abstractions;
using KiotVietTimeSheet.Application.Auth;
using KiotVietTimeSheet.Application.Auth.Common;
using KiotVietTimeSheet.Application.Dto;
using KiotVietTimeSheet.SharedKernel.Models;

namespace KiotVietTimeSheet.Application.Queries.GetComissionAll
{
    public class GetComissionAllDataTrialQuery : QueryBase<PagingDataSource<CommissionDto>>
    {
        public string Keyword { get; set; }
        public bool IsActive { get; set; }
        public GetComissionAllDataTrialQuery(string keyword, bool isActive = false)
        {
            Keyword = keyword;
            IsActive = isActive;
        }
    }
}

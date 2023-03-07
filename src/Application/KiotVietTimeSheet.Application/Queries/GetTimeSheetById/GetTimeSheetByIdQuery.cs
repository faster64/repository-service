using KiotVietTimeSheet.Application.Abstractions;
using KiotVietTimeSheet.Application.Auth;
using KiotVietTimeSheet.Application.Auth.Common;
using KiotVietTimeSheet.Application.Dto;

namespace KiotVietTimeSheet.Application.Queries.GetTimeSheetById
{
    [RequiredPermission(TimeSheetPermission.TimeSheet_Read)]
    public sealed class GetTimeSheetByIdQuery : QueryBase<TimeSheetDto>
    {
        public long Id { get; set; }
        public bool IncludeReferences { get; set; }

        public GetTimeSheetByIdQuery(long id, bool includeReferences)
        {
            Id = id;
            IncludeReferences = includeReferences;
        }
    }
}

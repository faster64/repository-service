using KiotVietTimeSheet.Application.Abstractions;
using KiotVietTimeSheet.Application.Auth;
using KiotVietTimeSheet.Application.Auth.Common;
using KiotVietTimeSheet.Application.Dto;

namespace KiotVietTimeSheet.Application.Queries.GetJobTitleById
{
    [RequiredPermission(TimeSheetPermission.JobTitle_Read)]
    public class GetJobTitleByIdQuery : QueryBase<JobTitleDto>
    {
        public long Id { get; set; }

        public GetJobTitleByIdQuery(long id)
        {
            Id = id;
        }
    }
}

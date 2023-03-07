using KiotVietTimeSheet.Application.Abstractions;
using KiotVietTimeSheet.Application.Auth;
using KiotVietTimeSheet.Application.Auth.Common;
using KiotVietTimeSheet.Application.Dto;

namespace KiotVietTimeSheet.Application.Queries.GetShiftById
{
    public sealed class GetShiftByIdQuery : QueryBase<ShiftDto>
    {
        public long Id { get; set; }

        public GetShiftByIdQuery(long id)
        {
            Id = id;
        }
    }
}

using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using KiotVietTimeSheet.Application.Abstractions;
using KiotVietTimeSheet.Application.Auth;
using KiotVietTimeSheet.Application.Dto;
using KiotVietTimeSheet.Application.Repositories.ReadRepositories;
using KiotVietTimeSheet.Domain.AggregatesModels.ClockingAggregate.Enums;
using KiotVietTimeSheet.Domain.AggregatesModels.ClockingAggregate.Models;
using MediatR;
using ServiceStack;
using ServiceStack.OrmLite;

namespace KiotVietTimeSheet.Application.Queries.GetClockingForPaySlipClockingDetail
{
    public class GetClockingForPaySlipClockingDetailQueryHandler : QueryHandlerBase,
        IRequestHandler<GetClockingForPaySlipClockingDetailQuery, PayslipClockingDetailPagingDataSource>
    {
        private readonly IClockingReadOnlyRepository _clockingReadOnlyRepository;

        public GetClockingForPaySlipClockingDetailQueryHandler(
            IClockingReadOnlyRepository clockingReadOnlyRepository,
            IAuthService authService 
        ) : base(authService)
        {
            _clockingReadOnlyRepository = clockingReadOnlyRepository;
        }

        public async Task<PayslipClockingDetailPagingDataSource> Handle(GetClockingForPaySlipClockingDetailQuery request, CancellationToken cancellationToken)
        {
            var query = request.Query;
            var q = (SqlExpression<Clocking>)query;
            q.And(c => c.ClockingPaymentStatus == (byte)ClockingPaymentStatuses.UnPaid
                       && (c.ClockingStatus == (byte)ClockingStatuses.CheckedOut
                           || (c.AbsenceType == (byte)AbsenceTypes.AuthorisedAbsence && c.ClockingStatus == (byte)ClockingStatuses.WorkOff)
                       ));
            var dataSource = await _clockingReadOnlyRepository.FiltersAsync(query);

            var result = dataSource.ConvertTo<PayslipClockingDetailPagingDataSource>();
            if (result.Data != null && result.Data.Any())
            {
                result.TotalTimeIsLate = result.Data.Sum(c => c.TimeIsLate);
                result.TotalTimeIsLeaveWorkEarly = result.Data.Sum(c => c.TimeIsLeaveWorkEarly);
                result.TotalOverTimeAfterShiftWork = result.Data.Sum(c => c.OverTimeAfterShiftWork);
                result.TotalOverTimeBeforeShiftWork = result.Data.Sum(c => c.OverTimeBeforeShiftWork);

                var sumRow = new ClockingDto
                {
                    Id = -1
                };
                result.Data.Insert(0, sumRow);
            }

            return result;
        }
    }
}

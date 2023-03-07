using KiotVietTimeSheet.Application.Abstractions;
using KiotVietTimeSheet.Application.Auth;
using KiotVietTimeSheet.Application.Dto;
using KiotVietTimeSheet.Application.Repositories.ReadRepositories;
using MediatR;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace KiotVietTimeSheet.Application.Queries.GetClockingPenalizeForPaySheet
{
    public class GetClockingPenalizeForPaySheetQueryHandler : QueryHandlerBase,
        IRequestHandler<GetClockingPenalizeForPaySheetQuery, List<ClockingPenalizeDto>>
    {
        private readonly IClockingReadOnlyRepository _clockingReadOnlyRepository;

        public GetClockingPenalizeForPaySheetQueryHandler(
            IClockingReadOnlyRepository clockingReadOnlyRepository,
            IAuthService authService
        ) : base(authService)
        {
            _clockingReadOnlyRepository = clockingReadOnlyRepository;
        }

        public async Task<List<ClockingPenalizeDto>> Handle(GetClockingPenalizeForPaySheetQuery request, CancellationToken cancellationToken)
        {
            var clockings = await _clockingReadOnlyRepository.GetClockingPenalizeForPaySheet(request.From, request.To, request.EmployeeIds);
            return clockings;
        }
    }
}

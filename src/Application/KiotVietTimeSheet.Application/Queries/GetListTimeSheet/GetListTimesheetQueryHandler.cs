using System.Threading;
using System.Threading.Tasks;
using KiotVietTimeSheet.Application.Abstractions;
using KiotVietTimeSheet.Application.Auth;
using KiotVietTimeSheet.Application.Dto;
using KiotVietTimeSheet.Application.Repositories.ReadRepositories;
using KiotVietTimeSheet.SharedKernel.Models;
using MediatR;

namespace KiotVietTimeSheet.Application.Queries.GetListTimesheet
{
    public class GetListTimesheetQueryHandler : QueryHandlerBase,
        IRequestHandler<GetListTimesheet.GetListTimesheetQuery, PagingDataSource<TimeSheetDto>>
    {
        private readonly ITimeSheetReadOnlyRepository _timeSheetReadOnlyRepository;

        public GetListTimesheetQueryHandler(
            IAuthService authService,
            ITimeSheetReadOnlyRepository timeSheetReadOnlyRepository

        ) : base(authService)
        {
            _timeSheetReadOnlyRepository = timeSheetReadOnlyRepository;
        }

        public async Task<PagingDataSource<TimeSheetDto>> Handle(GetListTimesheetQuery request, CancellationToken cancellationToken)
        {
            var result = await _timeSheetReadOnlyRepository.FiltersAsync(request.Query);
            return result;
        }
    }
}

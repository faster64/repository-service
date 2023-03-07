using System.Threading;
using System.Threading.Tasks;
using KiotVietTimeSheet.Application.Abstractions;
using KiotVietTimeSheet.Application.Auth;
using KiotVietTimeSheet.Application.Dto;
using KiotVietTimeSheet.Application.Repositories.ReadRepositories;
using KiotVietTimeSheet.SharedKernel.Models;
using MediatR;

namespace KiotVietTimeSheet.Application.Queries.GetListShift
{
    public class GetListShiftQueryHandler : QueryHandlerBase,
        IRequestHandler<GetListShiftQuery, PagingDataSource<ShiftDto>>
    {
        private readonly IShiftReadOnlyRepository _shiftReadOnlyRepository;

        public GetListShiftQueryHandler(
            IAuthService authService,
            IShiftReadOnlyRepository shiftReadOnlyRepository

        ) : base(authService)
        {
            _shiftReadOnlyRepository = shiftReadOnlyRepository;
        }

        public async Task<PagingDataSource<ShiftDto>> Handle(GetListShiftQuery request, CancellationToken cancellationToken)
        {
            var result = await _shiftReadOnlyRepository.FiltersAsync(request.Query);
            return result;
        }
    }
}

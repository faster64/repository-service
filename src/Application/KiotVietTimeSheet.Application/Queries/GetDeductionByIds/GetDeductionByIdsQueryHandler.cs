using System.Threading;
using System.Threading.Tasks;
using KiotVietTimeSheet.Application.Abstractions;
using KiotVietTimeSheet.Application.Auth;
using KiotVietTimeSheet.Application.Dto;
using KiotVietTimeSheet.Application.Repositories.ReadRepositories;
using KiotVietTimeSheet.SharedKernel.Models;
using MediatR;

namespace KiotVietTimeSheet.Application.Queries.GetDeductionByIds
{
    public class GetDeductionByIdsQueryHandler : QueryHandlerBase,
        IRequestHandler<GetDeductionByIdsQuery, PagingDataSource<DeductionDto>>
    {
        private readonly IDeductionReadOnlyRepository _deductionReadOnlyRepository;

        public GetDeductionByIdsQueryHandler(
            IDeductionReadOnlyRepository deductionReadOnlyRepository,
            IAuthService authService
        ) : base(authService)
        {
            _deductionReadOnlyRepository = deductionReadOnlyRepository;
        }

        public async Task<PagingDataSource<DeductionDto>> Handle(GetDeductionByIdsQuery request, CancellationToken cancellationToken)
        {
            return await _deductionReadOnlyRepository.FiltersAsync(request.Query, true);
        }
    }
}

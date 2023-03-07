using System.Threading;
using System.Threading.Tasks;
using KiotVietTimeSheet.Application.Abstractions;
using KiotVietTimeSheet.Application.Auth;
using KiotVietTimeSheet.Application.Dto;
using KiotVietTimeSheet.Application.Repositories.ReadRepositories;
using KiotVietTimeSheet.SharedKernel.Models;
using MediatR;

namespace KiotVietTimeSheet.Application.Queries.GetAllDeduction
{
    public class GetAllDeductionQueryHandler : QueryHandlerBase,
        IRequestHandler<GetAllDeductionQuery, PagingDataSource<DeductionDto>>
    {
        private readonly IDeductionReadOnlyRepository _deductionReadOnlyRepository;

        public GetAllDeductionQueryHandler(
            IDeductionReadOnlyRepository deductionReadOnlyRepository,
            IAuthService authService
        ) : base(authService)
        {
            _deductionReadOnlyRepository = deductionReadOnlyRepository;
        }

        public async Task<PagingDataSource<DeductionDto>> Handle(GetAllDeductionQuery request, CancellationToken cancellationToken)
        {
            var result = await _deductionReadOnlyRepository.FiltersAsync(request.Query);
            return result;
        }
    }
}

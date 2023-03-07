using System.Threading;
using System.Threading.Tasks;
using KiotVietTimeSheet.Application.Abstractions;
using KiotVietTimeSheet.Application.Auth;
using KiotVietTimeSheet.Application.Dto;
using KiotVietTimeSheet.Application.Repositories.ReadRepositories;
using KiotVietTimeSheet.SharedKernel.Models;
using MediatR;

namespace KiotVietTimeSheet.Application.Queries.GetCommissionDetailsByProductId
{
    public class GetCommissionDetailsByProductIdQueryHandler : QueryHandlerBase,
        IRequestHandler<GetCommissionDetailsByProductIdQuery, PagingDataSource<CommissionDetailDto>>
    {
        private readonly ICommissionDetailReadOnlyRepository _commissionDetailReadOnlyRepository;

        public GetCommissionDetailsByProductIdQueryHandler(
            IAuthService authService,
            ICommissionDetailReadOnlyRepository commissionDetailReadOnlyRepository
        ) : base(authService)
        {
            _commissionDetailReadOnlyRepository = commissionDetailReadOnlyRepository;
        }

        public async Task<PagingDataSource<CommissionDetailDto>> Handle(GetCommissionDetailsByProductIdQuery request,
            CancellationToken cancellationToken)
        {
            return await _commissionDetailReadOnlyRepository.GetCommissionDetailByProductId(request.Query);
        }
    }
}


using KiotVietTimeSheet.Application.Abstractions;
using KiotVietTimeSheet.Application.Dto;
using MediatR;
using System.Threading;
using System.Threading.Tasks;
using KiotVietTimeSheet.Application.Auth;
using KiotVietTimeSheet.Application.Repositories.ReadRepositories;
using AutoMapper;

namespace KiotVietTimeSheet.Application.Queries.GetCommissionById
{
    public class GetCommissionByIdQueryHandler : QueryHandlerBase,
        IRequestHandler<GetCommissionByIdQuery, CommissionDto>
    {
        private readonly ICommissionReadOnlyRepository _commissionReadOnlyRepository;
        private readonly IMapper _mapper;

        public GetCommissionByIdQueryHandler(
            IAuthService authService,
            ICommissionReadOnlyRepository commissionReadOnlyRepository,
            IMapper mapper
        ) : base(authService)
        {
            _commissionReadOnlyRepository = commissionReadOnlyRepository;
            _mapper = mapper;
        }

        public async Task<CommissionDto> Handle(GetCommissionByIdQuery request, CancellationToken cancellationToken)
        {
            var id = request.Id;
            var result = await _commissionReadOnlyRepository.FindByIdAsync(id, false, true);
            return _mapper.Map<CommissionDto>(result);
        }
    }
}


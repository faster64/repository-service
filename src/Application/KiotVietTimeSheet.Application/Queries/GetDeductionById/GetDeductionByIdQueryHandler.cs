using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using KiotVietTimeSheet.Application.Abstractions;
using KiotVietTimeSheet.Application.Auth;
using KiotVietTimeSheet.Application.Dto;
using KiotVietTimeSheet.Application.Repositories.ReadRepositories;
using MediatR;

namespace KiotVietTimeSheet.Application.Queries.GetDeductionById
{
    public class GetDeductionByIdQueryHandler : QueryHandlerBase,
        IRequestHandler<GetDeductionByIdQuery, DeductionDto>
    {
        private readonly IDeductionReadOnlyRepository _deductionReadOnlyRepository;
        private readonly IMapper _mapper;

        public GetDeductionByIdQueryHandler(
            IDeductionReadOnlyRepository deductionReadOnlyRepository,
            IMapper mapper,
            IAuthService authService
        ) : base(authService)
        {
            _deductionReadOnlyRepository = deductionReadOnlyRepository;
            _mapper = mapper;
        }

        public async Task<DeductionDto> Handle(GetDeductionByIdQuery request, CancellationToken cancellationToken)
        {
            return _mapper.Map<DeductionDto>(await _deductionReadOnlyRepository.FindByIdAsync(request.Id));
        }
    }
}

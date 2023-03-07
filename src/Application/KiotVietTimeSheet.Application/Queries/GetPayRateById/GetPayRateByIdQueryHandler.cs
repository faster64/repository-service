using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using KiotVietTimeSheet.Application.Abstractions;
using KiotVietTimeSheet.Application.Auth;
using KiotVietTimeSheet.Application.Dto;
using KiotVietTimeSheet.Application.Repositories.ReadRepositories;
using MediatR;

namespace KiotVietTimeSheet.Application.Queries.GetPayRateById
{
    public class GetPayRateByIdQueryHandler : QueryHandlerBase,
        IRequestHandler<GetPayRateByIdQuery, PayRateDto>
    {
        private readonly IMapper _mapper;
        private readonly IPayRateReadOnlyRepository _payRateReadOnlyRepository;
        public GetPayRateByIdQueryHandler(
            IAuthService authService,
            IPayRateReadOnlyRepository payRateReadOnlyRepository,
            IMapper mapper
            ) : base(authService)
        {
            _payRateReadOnlyRepository = payRateReadOnlyRepository;
            _mapper = mapper;
        }

        public async Task<PayRateDto> Handle(GetPayRateByIdQuery request, CancellationToken cancellationToken)
        {
            var payRate = await _payRateReadOnlyRepository.FindByIdAsync(request.Id);
            return _mapper.Map<PayRateDto>(payRate);
        }
    }
}

using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using KiotVietTimeSheet.Application.Abstractions;
using KiotVietTimeSheet.Application.Auth;
using KiotVietTimeSheet.Application.Dto;
using KiotVietTimeSheet.Application.Repositories.ReadRepositories;
using KiotVietTimeSheet.Domain.AggregatesModels.EmployeeAggregate.Specifications;
using MediatR;

namespace KiotVietTimeSheet.Application.Queries.GetPayRateByEmployeeId
{
    public class GetPayRateByEmployeeIdQueryHandler : QueryHandlerBase,
        IRequestHandler<GetPayRateByEmployeeIdQuery, PayRateDto>
    {
        private readonly IMapper _mapper;
        private readonly IPayRateReadOnlyRepository _payRateReadOnlyRepository;
        public GetPayRateByEmployeeIdQueryHandler(
            IAuthService authService,
            IPayRateReadOnlyRepository payRateReadOnlyRepository,
            IMapper mapper
            ) : base(authService)
        {
            _payRateReadOnlyRepository = payRateReadOnlyRepository;
            _mapper = mapper;
        }

        public async Task<PayRateDto> Handle(GetPayRateByEmployeeIdQuery request, CancellationToken cancellationToken)
        {
            var payRate = await _payRateReadOnlyRepository.FindBySpecificationAsync(new FindPayRateByEmployeeIdSpec(request.EmployeeId), true);
            return _mapper.Map<PayRateDto>(payRate);
        }
    }
}

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using KiotVietTimeSheet.Application.Abstractions;
using KiotVietTimeSheet.Application.Auth;
using KiotVietTimeSheet.Application.Dto;
using KiotVietTimeSheet.Application.Repositories.ReadRepositories;
using KiotVietTimeSheet.Domain.AggregatesModels.PayslipDetailAggregate.Specifications;
using MediatR;

namespace KiotVietTimeSheet.Application.Queries.GetPayslipDetailByTenantId
{
    public class GetPayslipDetailByTenantIdQueryHandler : QueryHandlerBase,
        IRequestHandler<GetPayslipDetailByTenantIdQuery, List<PayslipDetailDto>>
    {
        private readonly IMapper _mapper;
        private readonly IPayslipDetailReadOnlyRepository _payslipDetailReadOnlyRepository;

        public GetPayslipDetailByTenantIdQueryHandler(
            IAuthService authService,
            IMapper mapper,
            IPayslipDetailReadOnlyRepository payslipDetailReadOnlyRepository
        ) : base(authService)
        {
            _mapper = mapper;
            _payslipDetailReadOnlyRepository = payslipDetailReadOnlyRepository;
        }

        public async Task<List<PayslipDetailDto>> Handle(GetPayslipDetailByTenantIdQuery request, CancellationToken cancellationToken)
        {
            var spec = new FindPayslipDetailByTenantIdSpec(request.TenantId, request.RuleType);
            var payslipDetailList = await _payslipDetailReadOnlyRepository.GetBySpecificationAsync(spec);
            return _mapper.Map<List<PayslipDetailDto>>(payslipDetailList);
        }
    }
}

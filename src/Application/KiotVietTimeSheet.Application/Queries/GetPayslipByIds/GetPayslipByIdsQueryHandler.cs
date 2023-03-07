using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using KiotVietTimeSheet.Application.Abstractions;
using KiotVietTimeSheet.Application.Auth;
using KiotVietTimeSheet.Application.Dto;
using KiotVietTimeSheet.Application.Repositories.ReadRepositories;
using KiotVietTimeSheet.Domain.AggregatesModels.PayslipAggregate.Enums;
using MediatR;

namespace KiotVietTimeSheet.Application.Queries.GetPayslipById
{
    public class GetPayslipByIdsQueryHandler : QueryHandlerBase,
        IRequestHandler<GetPayslipByIdsQuery, List<long>>
    {
        private readonly IPayslipReadOnlyRepository _payslipReadOnlyRepository;
        private readonly IMapper _mapper;

        public GetPayslipByIdsQueryHandler(
            IAuthService authService,
            IPayslipReadOnlyRepository payslipReadOnlyRepository,
            IMapper mapper
        ) : base(authService)
        {
            _payslipReadOnlyRepository = payslipReadOnlyRepository;
            _mapper = mapper;
        }

        public async Task<List<long>> Handle(GetPayslipByIdsQuery request, CancellationToken cancellationToken)
        {
            object[] payslipIds = { request.Ids };
            var paysheetIds = 
                _mapper.Map<List<PayslipDto>>(await _payslipReadOnlyRepository.FindByIdsAsync(payslipIds))
                .Where(p => p.PayslipStatus != (byte)PayslipStatuses.PaidSalary)
                .Where(p => p.PayslipStatus != (byte)PayslipStatuses.Void)
                .Select(p => p.PaysheetId)
                .ToList();
            return paysheetIds;
        }
    }
}

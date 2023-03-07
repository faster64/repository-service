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
using KiotVietTimeSheet.Domain.AggregatesModels.PayslipAggregate.Specifications;
using MediatR;

namespace KiotVietTimeSheet.Application.Queries.GetUnPaidPayslipByEmployeeId
{
    public class GetUnPaidPayslipByEmployeeIdQueryHandler : QueryHandlerBase,
        IRequestHandler<GetUnPaidPayslipByEmployeeIdQuery, object>
    {
        private readonly IPayslipReadOnlyRepository _payslipReadOnlyRepository;
        private readonly IPaysheetReadOnlyRepository _paysheetReadOnlyRepository;
        private readonly IMapper _mapper;

        public GetUnPaidPayslipByEmployeeIdQueryHandler(
            IAuthService authService,
            IPayslipReadOnlyRepository payslipReadOnlyRepository,
            IPaysheetReadOnlyRepository paysheetReadOnlyRepository,
            IMapper mapper

        ) : base(authService)
        {
            _payslipReadOnlyRepository = payslipReadOnlyRepository;
            _mapper = mapper;
            _paysheetReadOnlyRepository = paysheetReadOnlyRepository;
        }

        public async Task<object> Handle(GetUnPaidPayslipByEmployeeIdQuery request, CancellationToken cancellationToken)
        {
            var unpaidPayslips = await GetUnPaidPayslipAsync(request.EmployeeId);
            var paysheetIds = unpaidPayslips.Select(e => e.PaysheetId).Distinct().ToList();
            var paysheets = await GetPaysheetsByIdsAsync(paysheetIds);
            return new Dictionary<string, object>() { { "UnpaidPayslips", unpaidPayslips }, { "Paysheets", paysheets } };
        }

        public async Task<List<UnPaidPayslipDto>> GetUnPaidPayslipAsync(long employeeId)
        {
            var spec = new FindPayslipByEmployeeId(employeeId)
                .And(new FindUnPaidPayslipSpec())
                .And(new FindPayslipByStatusSpec((byte)PayslipStatuses.PaidSalary));
            var result = await _payslipReadOnlyRepository.SelectIntoBySpecificationAsync<UnPaidPayslipDto>(spec);
            return result;
        }

        public async Task<List<PaysheetDto>> GetPaysheetsByIdsAsync(List<long> ids)
        {
            object[] paysheetIds = { ids };
            return _mapper.Map<List<PaysheetDto>>(await _paysheetReadOnlyRepository.FindByIdsAsync(paysheetIds));
        }
    }
}

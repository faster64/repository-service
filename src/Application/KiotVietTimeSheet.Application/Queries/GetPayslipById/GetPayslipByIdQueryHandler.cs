using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using KiotVietTimeSheet.Application.Abstractions;
using KiotVietTimeSheet.Application.Auth;
using KiotVietTimeSheet.Application.Dto;
using KiotVietTimeSheet.Application.Repositories.ReadRepositories;
using KiotVietTimeSheet.Domain.AggregatesModels.PayslipAggregate.Specifications;
using MediatR;

namespace KiotVietTimeSheet.Application.Queries.GetPayslipById
{
    public class GetPayslipByIdQueryHandler : QueryHandlerBase,
        IRequestHandler<GetPayslipByIdQuery, PayslipDto>
    {
        private readonly IPayslipReadOnlyRepository _payslipReadOnlyRepository;
        private readonly IMapper _mapper;
        private readonly IPaysheetReadOnlyRepository _paysheetReadOnlyRepository;
        private readonly IEmployeeReadOnlyRepository _employeeReadOnlyRepository;

        public GetPayslipByIdQueryHandler(
            IAuthService authService,
            IPayslipReadOnlyRepository payslipReadOnlyRepository,
            IMapper mapper,
            IPaysheetReadOnlyRepository paysheetReadOnlyRepository,
            IEmployeeReadOnlyRepository employeeReadOnlyRepository
        ) : base(authService)
        {
            _payslipReadOnlyRepository = payslipReadOnlyRepository;
            _mapper = mapper;
            _paysheetReadOnlyRepository = paysheetReadOnlyRepository;
            _employeeReadOnlyRepository = employeeReadOnlyRepository;
        }

        public async Task<PayslipDto> Handle(GetPayslipByIdQuery request, CancellationToken cancellationToken)
        {
            var spec = new FindPayslipByIdSpec(request.Id);
            var payslip = _mapper.Map<PayslipDto>(await _payslipReadOnlyRepository.FindBySpecificationAsync(spec, true));

            if (payslip == null) return null;

            payslip.Paysheet = _mapper.Map<PaysheetDto>(await _paysheetReadOnlyRepository.FindByIdAsync(payslip.PaysheetId));
            payslip.Employee = _mapper.Map<EmployeeDto>(await _employeeReadOnlyRepository.FindByIdAsync(payslip.EmployeeId));

            return payslip;
        }
    }
}

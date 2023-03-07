using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using KiotVietTimeSheet.Application.Abstractions;
using KiotVietTimeSheet.Application.Auth;
using KiotVietTimeSheet.Application.Dto;
using KiotVietTimeSheet.Application.Repositories.ReadRepositories;
using KiotVietTimeSheet.Application.ServiceClients;
using KiotVietTimeSheet.Application.ServiceClients.RequestModels;
using KiotVietTimeSheet.Domain.AggregatesModels.EmployeeAggregate.Specifications;
using KiotVietTimeSheet.Domain.AggregatesModels.PaysheetAggregate.Enum;
using KiotVietTimeSheet.Domain.AggregatesModels.PaysheetAggregate.Specifications;
using MediatR;

namespace KiotVietTimeSheet.Application.Commands.ExportPayslipData
{
    public class ExportPayslipDataCommandHandler : QueryHandlerBase,
        IRequestHandler<ExportPayslipDataCommand, List<PayslipDto>>
    {
        private readonly IPayslipReadOnlyRepository _payslipReadOnlyRepository;
        private readonly IPaysheetReadOnlyRepository _paysheetReadOnlyRepository;
        private readonly IEmployeeReadOnlyRepository _employeeReadOnlyRepository;
        private readonly IKiotVietServiceClient _kiotVietServiceClient;
        private readonly IMapper _mapper;

        public ExportPayslipDataCommandHandler(
            IAuthService authService,
            IPayslipReadOnlyRepository payslipReadOnlyRepository,
            IPaysheetReadOnlyRepository paysheetReadOnlyRepository,
            IEmployeeReadOnlyRepository employeeReadOnlyRepository,
            IKiotVietServiceClient kiotVietServiceClient,
            IMapper mapper
        ) : base(authService)
        {
            _payslipReadOnlyRepository = payslipReadOnlyRepository;
            _paysheetReadOnlyRepository = paysheetReadOnlyRepository;
            _mapper = mapper;
            _employeeReadOnlyRepository = employeeReadOnlyRepository;
            _kiotVietServiceClient = kiotVietServiceClient;
        }

        public async Task<List<PayslipDto>> Handle(ExportPayslipDataCommand request, CancellationToken cancellationToken)
        {
            var finalResult = new List<PayslipDto>();
            var result = await _payslipReadOnlyRepository.GetByQueryFilter(request.PayslipByPaysheetIdFilter);
            if (result?.Data == null) return finalResult;

            finalResult = _mapper.Map<List<PayslipDto>>(result.Data);
            var paySheetIds = result.Data.Select(x => x.PaysheetId).ToList();
            var paySheets = await _paysheetReadOnlyRepository.GetBySpecificationAsync(new FindPaysheetByIdsSpec(paySheetIds));

            if (!paySheets.Any()) return finalResult;

            var employeeIds = result.Data.Select(x => x.EmployeeId).ToList();
            var employees = await _employeeReadOnlyRepository.GetBySpecificationAsync(new FindEmployeeByIdsSpec(employeeIds), false, true);
            var payslipPaymentAllocations = await _kiotVietServiceClient.GetPayslipPaymentsValueIncludeAllocation(
                new GetPayslipPaymentsValueIncludeAllocationReq
                {
                    EmployeeIds = employeeIds
                });

            var isCheckPayslipPaymentAllocations = payslipPaymentAllocations.Any();

            foreach (var payslip in finalResult)
            {
                if (isCheckPayslipPaymentAllocations && payslip.PayslipStatus == (byte)PaysheetStatuses.TemporarySalary)
                {
                    var employeeAmount = payslipPaymentAllocations.FirstOrDefault(x => x.EmployeeId == payslip.EmployeeId);
                    payslip.TotalPayment += employeeAmount?.Amount ?? 0;
                }

                var employee = employees.FirstOrDefault(x => x.Id == payslip.EmployeeId);
                payslip.Employee = _mapper.Map<EmployeeDto>(employee);
                var paySheet = paySheets.FirstOrDefault(x => x.Id == payslip.PaysheetId);
                if (paySheet == null) continue;

                payslip.StartTime = paySheet.StartTime;
                payslip.EndTime = paySheet.EndTime;
            }

            return finalResult;
        }
    }
}

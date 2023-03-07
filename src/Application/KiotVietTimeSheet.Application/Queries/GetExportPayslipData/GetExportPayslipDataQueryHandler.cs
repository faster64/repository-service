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
using KiotVietTimeSheet.Domain.AggregatesModels.PaysheetAggregate.Models;
using KiotVietTimeSheet.Domain.AggregatesModels.PaysheetAggregate.Specifications;
using MediatR;

namespace KiotVietTimeSheet.Application.Queries.GetExportPayslipData
{
    public class GetExportPayslipDataQueryHandler : QueryHandlerBase,
        IRequestHandler<GetExportPayslipDataQuery, List<PayslipDto>>
    {
        private readonly IPayslipReadOnlyRepository _payslipReadOnlyRepository;
        private readonly IMapper _mapper;
        private readonly IEmployeeReadOnlyRepository _employeeReadOnlyRepository;
        private readonly IPaysheetReadOnlyRepository _paysheetReadOnlyRepository;
        private readonly IKiotVietServiceClient _kiotVietServiceClient;

        public GetExportPayslipDataQueryHandler(
            IAuthService authService,
            IPayslipReadOnlyRepository payslipReadOnlyRepository,
            IMapper mapper,
            IEmployeeReadOnlyRepository employeeReadOnlyRepository,
            IPaysheetReadOnlyRepository paysheetReadOnlyRepository,
            IKiotVietServiceClient kiotVietServiceClient
        ) : base(authService)
        {
            _payslipReadOnlyRepository = payslipReadOnlyRepository;
            _employeeReadOnlyRepository = employeeReadOnlyRepository;
            _mapper = mapper;
            _paysheetReadOnlyRepository = paysheetReadOnlyRepository;
            _kiotVietServiceClient = kiotVietServiceClient;
        }

        public async Task<List<PayslipDto>> Handle(GetExportPayslipDataQuery request, CancellationToken cancellationToken)
        {
            var finalResult = new List<PayslipDto>();
            var result = await _payslipReadOnlyRepository.GetByQueryFilter(request.Filter);
            if (result?.Data != null)
            {
                finalResult = _mapper.Map<List<PayslipDto>>(result.Data);
                var paySheetIds = result.Data.Select(x => x.PaysheetId).ToList();
                var paySheets = await _paysheetReadOnlyRepository.GetBySpecificationAsync(new FindPaysheetByIdsSpec(paySheetIds));


                if (paySheets.Any())
                {
                    var employeeIds = result.Data.Select(x => x.EmployeeId).ToList();
                    finalResult = await GeneratePayslip(finalResult, paySheets, employeeIds);
                }
            }
            return finalResult;
        }

        #region Private method
        private async Task<List<PayslipDto>> GeneratePayslip(List<PayslipDto> payslips, List<Paysheet> paySheets, List<long> employeeIds)
        {
            var employees = await _employeeReadOnlyRepository.GetBySpecificationAsync(new FindEmployeeByIdsSpec(employeeIds), false, true);
            var payslipPaymentAllocations = await _kiotVietServiceClient.GetPayslipPaymentsValueIncludeAllocation(
                new GetPayslipPaymentsValueIncludeAllocationReq
                {
                    EmployeeIds = employeeIds
                });


            foreach (var payslip in payslips)
            {
                if (payslip.PayslipStatus == (byte)PaysheetStatuses.TemporarySalary && payslipPaymentAllocations != null && payslipPaymentAllocations.Count > 0)
                {
                    var employeeAmount =
                        payslipPaymentAllocations.FirstOrDefault(x => x.EmployeeId == payslip.EmployeeId);
                    if (employeeAmount != null)
                    {
                        payslip.TotalPayment = payslip.TotalPayment + employeeAmount.Amount;
                    }
                }


                var employee = employees.FirstOrDefault(x => x.Id == payslip.EmployeeId);
                payslip.Employee = _mapper.Map<EmployeeDto>(employee);
                var paySheet = paySheets.FirstOrDefault(x => x.Id == payslip.PaysheetId);
                if (paySheet != null)
                {
                    payslip.StartTime = paySheet.StartTime;
                    payslip.EndTime = paySheet.EndTime;
                }
            }

            return payslips;
        }
        #endregion
    }
}

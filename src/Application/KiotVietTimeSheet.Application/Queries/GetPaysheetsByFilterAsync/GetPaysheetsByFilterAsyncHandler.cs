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
using KiotVietTimeSheet.Domain.AggregatesModels.PaysheetAggregate.Enum;
using MediatR;

namespace KiotVietTimeSheet.Application.Queries.GetPaysheetsByFilterAsync
{
    public class GetPaysheetsByFilterAsyncHandler : QueryHandlerBase,
        IRequestHandler<GetPaysheetsByFilterAsyncQuery, PaysheetPagingDataSource>
    {
        private readonly IPaysheetReadOnlyRepository _paysheetReadOnlyRepository;
        private readonly IKiotVietServiceClient _kiotVietServiceClient;
        private readonly IMapper _mapper;

        public GetPaysheetsByFilterAsyncHandler(
            IAuthService authService,
            IPaysheetReadOnlyRepository paysheetReadOnlyRepository,
            IKiotVietServiceClient kiotVietServiceClient,
            IMapper mapper

        ) : base(authService)
        {
            _paysheetReadOnlyRepository = paysheetReadOnlyRepository;
            _kiotVietServiceClient = kiotVietServiceClient;
            _mapper = mapper;
        }

        public async Task<PaysheetPagingDataSource> Handle(GetPaysheetsByFilterAsyncQuery request, CancellationToken cancellationToken)
        {
            var paysheetResult = await _paysheetReadOnlyRepository.GetListByQueryFilterAsync(request.PaysheetByFilter, request.IncludePaySlips);
            var dicPaySheetEmployeeIds = paysheetResult.dicPaysheetEmployeeIds;

            // Lấy giá trị phân bổ để tính thêm vào tổng thanh toán cho bảng lương tạm tính
            if (dicPaySheetEmployeeIds.Any())
            {
                var employeeIds = dicPaySheetEmployeeIds.Values.SelectMany(s => s).Distinct().ToList();
                var employeeWithAllocate = await _kiotVietServiceClient.GetPayslipPaymentsValueIncludeAllocation(
                    new GetPayslipPaymentsValueIncludeAllocationReq
                    {
                        EmployeeIds = employeeIds
                    });

                if (employeeWithAllocate != null && employeeWithAllocate.Count > 0)
                {
                    paysheetResult.totalPayment += employeeWithAllocate.Sum(s => s.Amount);
                    foreach (var paySheet in paysheetResult.dataSource.Data)
                    {
                        if (dicPaySheetEmployeeIds.ContainsKey(paySheet.Id) && paySheet.PaysheetStatus != (byte)PaysheetStatuses.PaidSalary)
                        {
                            paySheet.TotalPayment += employeeWithAllocate.Where(p =>
                                    dicPaySheetEmployeeIds[paySheet.Id].Contains(p.EmployeeId))
                                .Sum(p => p.Amount);
                            paySheet.CalculateTotalNeedPay(paySheet.TotalNetSalary, paySheet.TotalPayment);
                        }
                    }
                }
            }

            var pgTotalNeedPay = paysheetResult.totalNetSalary - paysheetResult.totalPayment;
            return new PaysheetPagingDataSource
            {
                Total = paysheetResult.dataSource.Total,
                Data = _mapper.Map<List<PaysheetDto>>(paysheetResult.dataSource.Data),
                TotalNetSalary = paysheetResult.totalNetSalary,
                TotalPayment = paysheetResult.totalPayment,
                TotalNeedPay = pgTotalNeedPay > 0 ? pgTotalNeedPay : 0
            };
        }
    }
}

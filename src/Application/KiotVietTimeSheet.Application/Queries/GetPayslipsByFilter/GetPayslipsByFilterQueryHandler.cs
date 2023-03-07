using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using KiotVietTimeSheet.Application.Abstractions;
using KiotVietTimeSheet.Application.Auth;
using KiotVietTimeSheet.Application.Dto;
using KiotVietTimeSheet.Application.Repositories.ReadRepositories;
using KiotVietTimeSheet.Domain.AggregatesModels.PaysheetAggregate.Specifications;
using KiotVietTimeSheet.SharedKernel.Models;
using MediatR;

namespace KiotVietTimeSheet.Application.Queries.GetPayslipsByFilter
{
    public class GetPayslipsByFilterQueryHandler : QueryHandlerBase,
        IRequestHandler<GetPayslipsByFilterQuery, PagingDataSource<PayslipDto>>
    {
        private readonly IPayslipReadOnlyRepository _payslipReadOnlyRepository;
        private readonly IPaysheetReadOnlyRepository _paysheetReadOnlyRepository;
        public GetPayslipsByFilterQueryHandler(
            IAuthService authService,
            IPayslipReadOnlyRepository payslipReadOnlyRepository,
            IPaysheetReadOnlyRepository paysheetReadOnlyRepository
        ) : base(authService)
        {
            _payslipReadOnlyRepository = payslipReadOnlyRepository;
            _paysheetReadOnlyRepository = paysheetReadOnlyRepository;
        }

        public async Task<PagingDataSource<PayslipDto>> Handle(GetPayslipsByFilterQuery request, CancellationToken cancellationToken)
        {
            var result = await _payslipReadOnlyRepository.FiltersAsync(request.Query);
            if (result?.Data != null)
            {
                var paySheetIds = result.Data.Select(x => x.PaysheetId).ToList();
                var paySheets = await _paysheetReadOnlyRepository.GetBySpecificationAsync(new FindPaysheetByIdsSpec(paySheetIds));
                if (paySheets.Any())
                {
                    foreach (var payslip in result.Data)
                    {
                        var paySheet = paySheets.FirstOrDefault(x => x.Id == payslip.PaysheetId);
                        if (paySheet != null)
                        {
                            payslip.StartTime = paySheet.StartTime;
                            payslip.EndTime = paySheet.EndTime;
                        }
                    }
                }
            }
            return result;
        }
    }
}

using System.Threading;
using System.Threading.Tasks;
using KiotVietTimeSheet.Application.Abstractions;
using KiotVietTimeSheet.Application.Auth;
using KiotVietTimeSheet.Application.Repositories.ReadRepositories;
using KiotVietTimeSheet.Domain.AggregatesModels.PayslipAggregate.Specifications;
using MediatR;

namespace KiotVietTimeSheet.Application.Queries.GetPaysheetExistsPayslips
{
    public class GetPaysheetExistsPayslipsQueryHandler :  QueryHandlerBase,
        IRequestHandler<GetPaysheetExistsPayslipsQuery, bool>
    {
        private readonly IPayslipReadOnlyRepository _payslipReadOnlyRepository; 

        public GetPaysheetExistsPayslipsQueryHandler(
            IAuthService authService,
            IPayslipReadOnlyRepository payslipReadOnlyRepository
        ) : base(authService)
        {
            _payslipReadOnlyRepository = payslipReadOnlyRepository;
        }


        public async Task<bool> Handle(GetPaysheetExistsPayslipsQuery request, CancellationToken cancellationToken)
        {
            var result = await _payslipReadOnlyRepository.AnyBySpecificationAsync(
                    new FindPayslipByPaysheetIdSpec(request.PaysheetId));

            return result;
        }
    }
}

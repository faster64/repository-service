using System.Threading;
using System.Threading.Tasks;
using KiotVietTimeSheet.Application.Abstractions;
using KiotVietTimeSheet.Application.Auth;
using KiotVietTimeSheet.Application.Dto;
using KiotVietTimeSheet.Application.Repositories.ReadRepositories;
using KiotVietTimeSheet.Domain.AggregatesModels.EmployeeAggregate.Models;
using KiotVietTimeSheet.Domain.AggregatesModels.PayRateAggregate.Models;
using KiotVietTimeSheet.SharedKernel.Models;
using MediatR;
using ServiceStack.OrmLite;

namespace KiotVietTimeSheet.Application.Queries.GetPayRate
{
    public class GetPayRateQueryHandler : QueryHandlerBase,
        IRequestHandler<GetPayRateQuery, PagingDataSource<PayRateDto>>
    {
        private readonly IPayRateReadOnlyRepository _payRateReadOnlyRepository;
        public GetPayRateQueryHandler(
            IAuthService authService,
            IPayRateReadOnlyRepository payRateReadOnlyRepository
            ) : base(authService)
        {
            _payRateReadOnlyRepository = payRateReadOnlyRepository;
        }

        public async Task<PagingDataSource<PayRateDto>> Handle(GetPayRateQuery request, CancellationToken cancellationToken)
        {
            var q = (SqlExpression<PayRate>)request.Query;
            q = q.Join<PayRate, Employee>((pr, em) => pr.EmployeeId == em.Id && em.IsActive && !em.IsDeleted);

            var result = await _payRateReadOnlyRepository.FiltersAsync(q);
            return result;
        }
    }
}

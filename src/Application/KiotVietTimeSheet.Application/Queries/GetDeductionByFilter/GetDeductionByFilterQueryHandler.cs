using System.Threading;
using System.Threading.Tasks;
using KiotVietTimeSheet.Application.Abstractions;
using KiotVietTimeSheet.Application.Auth;
using KiotVietTimeSheet.Application.Repositories.ReadRepositories;
using KiotVietTimeSheet.Domain.AggregatesModels.DeductionAggregate.Models;
using MediatR;
using ServiceStack.OrmLite;

namespace KiotVietTimeSheet.Application.Queries.GetDeductionByFilter
{
    public class GetDeductionByFilterQueryHandler : QueryHandlerBase,
        IRequestHandler<GetDeductionByFilterQuery, object>
    {
        private readonly IDeductionReadOnlyRepository _deductionReadOnlyRepository;

        public GetDeductionByFilterQueryHandler(
            IDeductionReadOnlyRepository deductionReadOnlyRepository,
            IAuthService authService
        ) : base(authService)
        {
            _deductionReadOnlyRepository = deductionReadOnlyRepository;
        }

        public async Task<object> Handle(GetDeductionByFilterQuery request, CancellationToken cancellationToken)
        {
            var query = request.Query;
            var deductionIdDeleted = request.DeductionIdDeleted;
            var q = (SqlExpression<Deduction>)query;
            q.Where(x => !x.IsDeleted || x.Id == deductionIdDeleted);

            var result = await _deductionReadOnlyRepository.FiltersAsync(query, true);
            return result;
        }
    }
}

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using KiotVietTimeSheet.Application.Abstractions;
using KiotVietTimeSheet.Application.Auth;
using KiotVietTimeSheet.Application.Repositories.ReadRepositories;
using KiotVietTimeSheet.Domain.AggregatesModels.PaysheetAggregate.Models;
using MediatR;

namespace KiotVietTimeSheet.Application.Queries.GetPaysheetByIds
{
    public class GetPaysheetByIdsQueryHandler : QueryHandlerBase,
        IRequestHandler<GetPaysheetByIdsQuery, List<Paysheet>>
    {
        private readonly IPaysheetReadOnlyRepository _paysheetReadOnlyRepository;

        public GetPaysheetByIdsQueryHandler(
            IAuthService authService,
            IPaysheetReadOnlyRepository paysheetReadOnlyRepository

        ) : base(authService)
        {
            _paysheetReadOnlyRepository = paysheetReadOnlyRepository;
        }

        public async Task<List<Paysheet>> Handle(GetPaysheetByIdsQuery request, CancellationToken cancellationToken)
        {
            object[] paysheetIds = {request.Ids};
            var paysheetList = await _paysheetReadOnlyRepository.FindByIdsAsync(paysheetIds);
            return paysheetList;
        }
    }
}

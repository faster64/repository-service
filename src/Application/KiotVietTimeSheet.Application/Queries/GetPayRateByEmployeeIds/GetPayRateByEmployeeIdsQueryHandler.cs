using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using KiotVietTimeSheet.Application.Abstractions;
using KiotVietTimeSheet.Application.Auth;
using KiotVietTimeSheet.Application.Repositories.ReadRepositories;
using KiotVietTimeSheet.Domain.AggregatesModels.PayRateAggregate.Models;
using KiotVietTimeSheet.Domain.AggregatesModels.PayRateAggregate.Specifications;
using MediatR;
using Microsoft.Extensions.Logging;

namespace KiotVietTimeSheet.Application.Queries.GetPayRateByEmployeeIds
{
    public class GetPayRateByEmployeeIdsQueryHandler : QueryHandlerBase,
        IRequestHandler<GetPayRateByEmployeeIdsQuery, List<PayRate>>
    {
        private readonly IPayRateReadOnlyRepository _payRateReadOnlyRepository;
        private readonly ILogger<GetPayRateByEmployeeIdsQueryHandler> _logger;
        public GetPayRateByEmployeeIdsQueryHandler(
            IAuthService authService,
            IPayRateReadOnlyRepository payRateReadOnlyRepository,
            ILogger<GetPayRateByEmployeeIdsQueryHandler> logger
            ) : base(authService)
        {
            _logger = logger;
            _payRateReadOnlyRepository = payRateReadOnlyRepository;
        }

        public async Task<List<PayRate>> Handle(GetPayRateByEmployeeIdsQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var payRate = await _payRateReadOnlyRepository.GetBySpecificationAsync(new FindPayRateByEmployeeIdsSpec(request.EmployeeIds), true);
                return payRate;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                throw;
            }
        }
    }
}

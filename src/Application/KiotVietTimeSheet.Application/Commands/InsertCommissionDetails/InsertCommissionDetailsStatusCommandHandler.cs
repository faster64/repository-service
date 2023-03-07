using KiotVietTimeSheet.Application.Abstractions;
using KiotVietTimeSheet.Application.Auth;
using KiotVietTimeSheet.Application.Caching;
using KiotVietTimeSheet.Domain.AggregatesModels.CommissionDetailAggregate.Enums;
using KiotVietTimeSheet.SharedKernel.Domain;
using MediatR;
using System.Threading;
using System.Threading.Tasks;
using KiotVietTimeSheet.Application.ServiceClients.Dtos;
namespace KiotVietTimeSheet.Application.Commands.InsertCommissionDetails
{
    public class InsertCommissionDetailsStatusCommandHandler : BaseCommandHandler,
        IRequestHandler<InsertCommissionDetailsStatusCommand, InsertCommissionDetailsStatus>
    {
        private readonly ICacheClient _cacheClient;
        private readonly IAuthService _authService;

        public InsertCommissionDetailsStatusCommandHandler(
            IEventDispatcher eventDispatcher,
            IAuthService authService,
            ICacheClient cacheClient
        )
            : base(eventDispatcher)
        {
            _authService = authService;
            _cacheClient = cacheClient;
        }

        public async Task<InsertCommissionDetailsStatus> Handle(InsertCommissionDetailsStatusCommand request, CancellationToken cancellationToken)
        {
            var context = _authService.Context;
            var keyObject = $@"{CacheKeys.GetEntityCacheKey(
                context.TenantCode,
                nameof(KiotVietTimeSheet),
                nameof(InsertCommissionDetailsStatus)
            )}*";
            var insertCommissionDetailStatus = _cacheClient.GetOrDefault<InsertCommissionDetailsStatus>(keyObject);
            if (insertCommissionDetailStatus != null &&
                (insertCommissionDetailStatus.Status == InsertCommissionDetailStatusEnums.Completed ||
                 insertCommissionDetailStatus.Status == InsertCommissionDetailStatusEnums.Error)) _cacheClient.Remove(keyObject);
            return insertCommissionDetailStatus;
        }
    }
}


using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using KiotVietTimeSheet.DomainEventProcessWorker.AuditTrailProcess.Common;
using KiotVietTimeSheet.DomainEventProcessWorker.AuditTrailProcess.Dto;
using KiotVietTimeSheet.Infrastructure.AuditTrail;
using KiotVietTimeSheet.Infrastructure.KiotVietApiClient;
using KiotVietTimeSheet.Infrastructure.KiotVietApiClient.Dtos;
using KiotVietTimeSheet.Infrastructure.Persistence.Models;
using KiotVietTimeSheet.SharedKernel.EventBus;
using ServiceStack;
using KiotVietTimeSheet.Application.Logging;

namespace KiotVietTimeSheet.DomainEventProcessWorker.AuditTrailProcess
{
    public class BaseAuditProcess
    {
        private ILogger<BaseAuditProcess> _logger;
        private ILogger<BaseAuditProcess> Logger => _logger ?? (_logger = HostContext.Resolve<ILogger<BaseAuditProcess>>());

        protected readonly IKiotVietApiClient KiotVietApiClient;
        protected readonly IAuditProcessFailEventService AuditProcessFailEventService;

        public BaseAuditProcess(IKiotVietApiClient kiotVietApiClient, IAuditProcessFailEventService auditProcessFailEventService)
        {
            KiotVietApiClient = kiotVietApiClient;
            AuditProcessFailEventService = auditProcessFailEventService;
        }

        protected async Task AddLogAsync(AuditTrailLog log, int groupId, string retailerCode)
        {
            var auditLogReq = new WriteAuditLogReq
            {
                Content = log.Content,
                Action = log.Action,
                CreatedDate = log.CreatedDate,
                FunctionId = log.FunctionId,
                RetailerId = log.TenantId,
                BranchId = log.BranchId,
                UserId = log.UserId
            };
            await KiotVietApiClient.WriteAuditLogAsync(auditLogReq, groupId, retailerCode);
        }

        protected AuditTrailLog GenerateLog(
            TimeSheetFunctionTypes functionType,
            TimeSheetAuditTrailAction actionType,
            string content,
            IntegrationEventContext context
        )
        {
            return new AuditTrailLog
            {
                FunctionId = (int)functionType,
                Action = (int)actionType,
                Content = content,
                CreatedDate = DateTime.UtcNow,
                TenantId = context.TenantId,
                UserId = context.UserId,
                BranchId = context.BranchId
            };
        }

        protected async Task Retry(IntegrationEvent @event, Exception ex)
        {
            var errMess = GetMessageBaseAuditProcessTemplateError(ex.Message, @event);
            Logger.LogError(ex, errMess.ToJson());
            await AuditProcessFailEventService.AddAsync(new AuditProcessFailEvent(@event, ex.Message));
        }

        private MessageTemplateWriteLogger GetMessageBaseAuditProcessTemplateError(string description, IntegrationEvent @event)
        {
            var modelTemplate = new MessageTemplateWriteLogger
            {
                UserId = @event.Context.UserId,
                GroupId = @event.Context.GroupId,
                BranchId = @event.Context.BranchId,
                TenantId = @event.Context.TenantId,
                RetailerCode = @event.Context.RetailerCode,
                Description = description
            };

            return modelTemplate;
        }

    }
}

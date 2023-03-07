using System;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using KiotVietTimeSheet.Application.ServiceClients;
using KiotVietTimeSheet.Application.ServiceClients.Dtos;
using KiotVietTimeSheet.SharedKernel.EventBus;
using KiotVietTimeSheet.AuditTrailWorker.AuditTrailProcess.Common;
using KiotVietTimeSheet.AuditTrailWorker.AuditTrailProcess.Dto;
using ExecutionContext = KiotVietTimeSheet.SharedKernel.Auth.ExecutionContext;

namespace KiotVietTimeSheet.AuditTrailWorker.AuditTrailProcess
{
    public class BaseAuditProcess
    {
        protected readonly IKiotVietInternalService KiotVietInternalService;

        public BaseAuditProcess(IKiotVietInternalService kiotVietInternalService)
        {
            KiotVietInternalService = kiotVietInternalService;
        }

        public BaseAuditProcess(ExecutionContext context, IKiotVietInternalService kiotVietInternalService)
        {
            KiotVietInternalService = kiotVietInternalService;

            SetCultureLangue(context);
        }

        protected async Task AddLogAsync(AuditTrailLog log)
        {
            var auditLogReq = new WriteAuditLogRequest
            {
                Content = log.Content,
                Action = log.Action,
                CreatedDate = log.CreatedDate,
                FunctionId = log.FunctionId,
                RetailerId = log.TenantId,
                BranchId = log.BranchId,
                UserId = log.UserId
            };
            await KiotVietInternalService.WriteAuditLogAsync(auditLogReq);
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

        private static void SetCultureLangue(ExecutionContext context)
        {
            var info = new CultureInfo(context?.Language ?? "vi-VN")
            {
                DateTimeFormat = {ShortDatePattern = "dd/MM/yyyy"},
                NumberFormat =
                {
                    CurrencyDecimalSeparator = ".",
                    CurrencyGroupSeparator = ",",
                    CurrencySymbol = string.Empty,
                    NumberDecimalSeparator = ".",
                    NumberGroupSeparator = ",",
                    PercentDecimalSeparator = ".",
                    PercentGroupSeparator = ","
                }
            };
            Thread.CurrentThread.CurrentCulture = info;
            Thread.CurrentThread.CurrentUICulture = info;
        }
    }
}

using System;
using System.Data.SqlClient;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using KiotVietTimeSheet.Application.Abstractions;
using Microsoft.Extensions.Logging;
using KiotVietTimeSheet.Application.Runtime.Exception;
using KiotVietTimeSheet.Infrastructure.Persistence.Ef;
using KiotVietTimeSheet.SharedKernel.Notification;
using MediatR;
using Microsoft.EntityFrameworkCore;
using KiotVietTimeSheet.Application.Auth;
using KiotVietTimeSheet.Application.Logging;
using ServiceStack;

namespace KiotVietTimeSheet.Infrastructure.Behaviors
{
    public class TransactionBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    {
        private readonly EfDbContext _dbContext;
        private readonly ITimeSheetIntegrationEventService _timeSheetIntegrationEventService;
        private readonly ILogger<TransactionBehavior<TRequest, TResponse>> _logger;
        private readonly DomainNotificationHandler _notificationHandler;
        private readonly IAuthService _authService;

        public TransactionBehavior(
            EfDbContext dbContext,
            ITimeSheetIntegrationEventService timeSheetIntegrationEventService,
            ILogger<TransactionBehavior<TRequest, TResponse>> logger,
            INotificationHandler<DomainNotification> notificationHandler,
            IAuthService authService
        )
        {
            _dbContext = dbContext ?? throw new ArgumentException(nameof(EfDbContext));
            _timeSheetIntegrationEventService = timeSheetIntegrationEventService ?? throw new ArgumentException(nameof(ITimeSheetIntegrationEventService));
            _logger = logger;
            _notificationHandler = (DomainNotificationHandler)notificationHandler ?? throw new ArgumentException(nameof(INotificationHandler<DomainNotification>));
            _authService = authService;
        }

        public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
        {
            var response = default(TResponse);
            var isInternalRequest = request is IInternalRequest;
            if (isInternalRequest)
            {
                return await next();
            }
            try
            {
                if (_dbContext.HasActiveTransaction || request is QueryBase<TResponse>)
                {
                    return await next();
                }

                var strategy = _dbContext.Database.CreateExecutionStrategy();
                await strategy.ExecuteAsync(async () =>
                {
                    Guid transactionId;
                    var hasError = false;
                    using (var transaction = await _dbContext.BeginTransactionAsync())
                    {
                        response = await next();
                        hasError = _notificationHandler.Notifications.Select(n => n.ErrorResult).Any();
                        if (!hasError)
                        {
                            await _dbContext.CommitTransactionAsync(transaction);
                            transactionId = transaction.TransactionId;
                        }
                    }
                    if (!hasError)
                    {
                        await _timeSheetIntegrationEventService.PublishEventsToEventBusAsync(transactionId);
                    }
                });

                return response;
            }
            catch (Exception ex)
            {
                if (ex.InnerException?.InnerException is SqlException exception && exception.Number == 1205) // DeadLock
                {
                    throw new KvTimeSheetException("Đang có thay đổi mới hơn từ server. Vui lòng thử lại.");
                }

                var exErr = GetMessageTemplateErrorInTransaction(ex.Message);
                _logger.LogError(ex, exErr.ToJson());
                throw;
            }
        }

        private MessageTemplateWriteLogger GetMessageTemplateErrorInTransaction(string description)
        {
            var user = _authService.Context.User;
            var contextUser = _authService.Context;

            var modelTemplate = new MessageTemplateWriteLogger
            {
                UserId = user.Id,
                UserName = user.UserName,
                KvSessionId = user.KvSessionId,
                IndustryId = user.IndustryId,
                GroupId = user.GroupId,
                BranchId = contextUser.BranchId,
                TenantId = contextUser.TenantId,
                CurrentLang = contextUser.Language,
                RetailerCode = contextUser.TenantCode,
                UserIsAdmin = user.IsAdmin,
                Description = description
            };

            return modelTemplate;
        }
    }
}

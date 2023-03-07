using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KiotVietTimeSheet.Api.ServiceInterface.Attributes;
using KiotVietTimeSheet.Api.ServiceInterface.Common;
using KiotVietTimeSheet.Api.ServiceModel;
using KiotVietTimeSheet.Application.Auth;
using KiotVietTimeSheet.Application.Commands.CreateEmployee;
using KiotVietTimeSheet.Application.Commands.DeleteEmployee;
using KiotVietTimeSheet.Application.Queries.GetEmployee;
using KiotVietTimeSheet.Application.Runtime.Exception;
using KiotVietTimeSheet.Infrastructure.DbMaster;
using KiotVietTimeSheet.SharedKernel.Models;
using KiotVietTimeSheet.SharedKernel.Notification;
using MediatR;
using Microsoft.Extensions.Logging;
using ServiceStack;
using Message = KiotVietTimeSheet.Resources.Message;

namespace KiotVietTimeSheet.Api.ServiceInterface
{
    [InternalAccess]
    public class SyncApi : Service
    {
        private readonly TimeSheetHelper _timeSheetHelper;
        public ILogger Logger { get; }
        private readonly IMediator _mediator;
        private readonly IAuthService _authService;
        private readonly DomainNotificationHandler _notificationHandler;
        private IEnumerable<ErrorResult> Errors => _notificationHandler.Notifications.Select(n => n.ErrorResult);

        public SyncApi(ILogger<SyncApi> logger, IMediator mediator, INotificationHandler<DomainNotification> notificationHandler, IMasterDbService masterDbService, IAuthService authService)
        {
            Logger = logger;
            _mediator = mediator;
            _notificationHandler = (DomainNotificationHandler)notificationHandler;
            _timeSheetHelper = new TimeSheetHelper(masterDbService);
            _authService = authService;
        }
        public string Get(Ping p)
        {
            return "pong";
        }

        public async Task<object> Get(SyncEmployeeList request)
        {
            if (string.IsNullOrEmpty(Request.Headers["RetailerId"]) || !int.TryParse(Request.Headers["RetailerId"], out var retailerId))
            {
                throw new KvTimeSheetException("Retailer Not Found");
            }
            var result = await _mediator.Send(new SyncEmployeeListQuery()
            {
                CurrentPage = request.CurrentPage,
                PageSize = request.PageSize,
                LastModifiedFrom = request.LastModifiedFrom,
                RetailerId = retailerId
            });
            return result;
        }

        public async Task<object> Post(CreateSyncEmployee req)
        {
            if (string.IsNullOrEmpty(Request.Headers["RetailerId"]) || !int.TryParse(Request.Headers["RetailerId"], out var retailerId))
            {
                throw new KvTimeSheetException("Retailer Not Found");
            }
            if (string.IsNullOrEmpty(Request.Headers["UserId"]) || !int.TryParse(Request.Headers["UserId"], out _))
            {
                throw new KvTimeSheetException("UserId Not Found");
            }
            var (blockUnit, retailerCode) = await _timeSheetHelper.GetBlockUnitByRetailerId(retailerId);
            var result = await _mediator.Send(new CreateSyncEmployeeCommand(
            
                req.Employee,
                retailerId,
                retailerCode,
                blockUnit
            ));
            if (Errors.Any())
            {
                throw new KvTimeSheetException(Errors.FirstOrDefault()?.Message ?? Message.error_whenCreateData);
            }
            return new { Success = true, Data = new { result.Id } };
        }

        public async Task<object> Delete(DeleteSyncEmployee req)
        {
            if (string.IsNullOrEmpty(Request.Headers["RetailerId"]) || !int.TryParse(Request.Headers["RetailerId"], out var retailerId))
            {
                throw new KvTimeSheetException("Retailer Not Found");
            }
            if (string.IsNullOrEmpty(Request.Headers["UserId"]) || !int.TryParse(Request.Headers["UserId"], out _))
            {
                throw new KvTimeSheetException("UserId Not Found");
            }
            var retailerCode = _authService.Context?.TenantCode;
            if (retailerCode.IsNullOrEmpty() && !string.IsNullOrEmpty(Request.Headers["Retailer"]))
            {
                retailerCode = Request.Headers["Retailer"];
            }
            if(retailerCode.IsNullOrEmpty())
            {
                retailerCode = await _timeSheetHelper.GetRetailerCodeAsync(retailerId);                
            }
            if(_authService.Context.TenantCode.IsNullOrEmpty())
            {
                _authService.Context.TenantCode = retailerCode;
            }
                
            await _mediator.Send(new DeleteSyncEmployeeCommand(req.Id)
            {
                Retailer = retailerCode
            });
            if (Errors.Any())
            {
                throw new KvTimeSheetException(Errors.FirstOrDefault()?.Message ?? Message.error_whenDeleteData);
            }
            return new {Success = true, Message = "Delete successfully"};
        }

    }
}
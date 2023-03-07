using AutoMapper;
using KiotVietTimeSheet.Application.Abstractions;
using KiotVietTimeSheet.Application.Auth;
using KiotVietTimeSheet.Application.Caching;
using KiotVietTimeSheet.Application.Commands.DeleteCommission;
using KiotVietTimeSheet.Application.Commands.DeleteCommissionDataTrial;
using KiotVietTimeSheet.Application.Commands.DeleteEmployee;
using KiotVietTimeSheet.Application.Commands.DeleteTrialData;
using KiotVietTimeSheet.Application.Dto;
using KiotVietTimeSheet.Application.Queries.GetComissionAll;
using KiotVietTimeSheet.Application.Repositories.ReadRepositories;
using KiotVietTimeSheet.Application.ServiceClients;
using KiotVietTimeSheet.Application.ServiceClients.RequestModels;
using KiotVietTimeSheet.Domain.AggregatesModels.EmployeeAggregate.Models;
using KiotVietTimeSheet.Domain.AggregatesModels.EmployeeAggregate.Specifications;
using KiotVietTimeSheet.Domain.Common;
using KiotVietTimeSheet.Resources;
using KiotVietTimeSheet.SharedKernel.Domain;
using MediatR;
using Microsoft.Extensions.Logging;
using ServiceStack;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace KiotVietTimeSheet.Application.Commands.DeleteTrialData
{
    public class DeleteTrialDataCommandHandler : BaseCommandHandler, IRequestHandler<DeleteTrialDataCommand, object>
    {
        private readonly ITrialDataReadOnlyRepository _trialDataReadOnlyRepository;
        private readonly IEmployeeReadOnlyRepository _employeeReadOnlyRepository;
        private readonly IKiotVietServiceClient _kiotVietServiceClient;
        private readonly ILogger<DeleteEmployeeCommandHandler> _logger;
        private readonly ICacheClient _cacheClient;
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;

        public DeleteTrialDataCommandHandler(
            IEventDispatcher eventDispatcher,
            ITrialDataReadOnlyRepository trialDataReadOnlyRepository,
            IEmployeeReadOnlyRepository employeeReadOnlyRepository,
            IKiotVietServiceClient kiotVietServiceClient,
            ILogger<DeleteEmployeeCommandHandler> logger,
            ICacheClient cacheClient,
            IMediator mediator,
            IMapper mapper
        )
        : base(eventDispatcher)
        {
            _trialDataReadOnlyRepository = trialDataReadOnlyRepository;
            _employeeReadOnlyRepository = employeeReadOnlyRepository;
            _kiotVietServiceClient = kiotVietServiceClient;
            _logger = logger;
            _cacheClient = cacheClient;
            _mediator = mediator;
            _mapper = mapper;
        }

        public async Task<object> Handle(DeleteTrialDataCommand request, CancellationToken cancellationToken)
        {
            if (request.TrialType == 0)
            {
                NotifyValidationErrors(typeof(DeleteTrialDataCommand), new List<string>() { string.Format(Message.not_empty, "TrialType") });
                return null;
            }
            else if (request.BranchId == 0)
            {
                NotifyValidationErrors(typeof(DeleteTrialDataCommand), new List<string>() { string.Format(Message.not_empty, "BranchId") });
                return null;
            }
            else if (request.UserIdAdmin == 0)
            {
                NotifyValidationErrors(typeof(DeleteTrialDataCommand), new List<string>() { string.Format(Message.not_empty, "UserIdAdmin") });
                return null;
            }
            else if (string.IsNullOrEmpty(request.TenantCode))
            {
                NotifyValidationErrors(typeof(DeleteTrialDataCommand), new List<string>() { string.Format(Message.not_empty, "TenantCode") });
                return null;
            }
            else if (request.TenantId == 0)
            {
                NotifyValidationErrors(typeof(DeleteTrialDataCommand), new List<string>() { string.Format(Message.not_empty, "TenantId") });
                return null;
            }
            else if (request.GroupId == 0)
            {
                NotifyValidationErrors(typeof(DeleteTrialDataCommand), new List<string>() { string.Format(Message.not_empty, "GroupId") });
                return null;
            }

            var cacheKeys = CacheKeys.GetListEntityCacheKey(request.TenantCode, nameof(Employee), "*");
            _cacheClient.RemoveByParttern(cacheKeys);

            var isSuccess = false;
            switch (request.TrialType)
            {
                case 3:
                    var listCommission = await _mediator.Send(new GetComissionAllDataTrialQuery("", true));
                    if(listCommission.Data.Count > 0)
                    {
                        foreach (var commission in listCommission.Data)
                        {
                            await _mediator.Send(new DeleteCommissionDataTrialCommand(commission.Id, 0, request.UserIdAdmin, request.TenantCode, request.TenantId, request.GroupId));
                        }
                    }
                    var result = await _employeeReadOnlyRepository.GetBySpecificationAsync(new FindEmployeeByBranchIdSpec(request.BranchId));
                    var employees = _mapper.Map<List<EmployeeDto>>(result);

                    isSuccess = await _trialDataReadOnlyRepository.DeleteBookingTrialDataAsync(request.TenantId, request.UserIdAdmin);
                    if (isSuccess)
                    {
                        foreach (var employee in employees)
                        {
                            if (employee.Code == "NV000001") continue;
                            await HandleNotifyChangeAsync(employee);
                        }
                    }
                    break;
                default:
                    break;
            }
            return new { IsSuccess = isSuccess };
        }

        private async Task HandleNotifyChangeAsync(EmployeeDto employee)
        {
            if (employee == null || employee.Id <= 0)
            {
                _logger.LogError("[HandleNotifyChange] Delete Employee has empty data");
                return;
            }
            try
            {

                using (var tokenSource = new CancellationTokenSource(Constant.MillisecondsDelay))
                {
                    var req = new OnDelEmployeeReq()
                    {
                        RetailerId = employee.TenantId,
                        EmployeeId = employee.Id
                    };
                    var resp = await _kiotVietServiceClient.OnDeleteEmployee(req, tokenSource.Token);
                    var log = new
                    {
                        Req = req,
                        Message = resp?.ResponseStatus?.Message
                    };
                    _logger.LogInformation($"[HandleNotifyChange] Delete Employee : {log.ToSafeJson()}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
            }
        }
    }
}
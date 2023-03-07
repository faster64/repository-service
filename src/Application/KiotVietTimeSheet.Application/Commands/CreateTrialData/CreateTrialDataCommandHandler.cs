using AutoMapper;
using KiotVietTimeSheet.Application.Abstractions;
using KiotVietTimeSheet.Application.Auth;
using KiotVietTimeSheet.Application.Caching;
using KiotVietTimeSheet.Application.Commands.CreateCommission;
using KiotVietTimeSheet.Application.Commands.CreateCommissionDataTrial;
using KiotVietTimeSheet.Application.Commands.CreateCommissionDetailByProductCategory;
using KiotVietTimeSheet.Application.Commands.CreateCommissionDetailByProductCategoryDataTrial;
using KiotVietTimeSheet.Application.Commands.CreateEmployee;
using KiotVietTimeSheet.Application.Dto;
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
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace KiotVietTimeSheet.Application.Commands.CreateTrialData
{
    public class CreateTrialDataCommandHandler : BaseCommandHandler, IRequestHandler<CreateTrialDataCommand, object>
    {
        private readonly ITrialDataReadOnlyRepository _trialDataReadOnlyRepository;
        private readonly IEmployeeReadOnlyRepository _employeeReadOnlyRepository;
        private readonly IKiotVietServiceClient _kiotVietServiceClient;
        private readonly ILogger<CreateEmployeeCommandHandler> _logger;
        private readonly ICacheClient _cacheClient;
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;

        public CreateTrialDataCommandHandler(
            IEventDispatcher eventDispatcher,
            ITrialDataReadOnlyRepository trialDataReadOnlyRepository,
            IEmployeeReadOnlyRepository employeeReadOnlyRepository,
            IKiotVietServiceClient kiotVietServiceClient,
            ILogger<CreateEmployeeCommandHandler> logger,
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

        public async Task<object> Handle(CreateTrialDataCommand request, CancellationToken cancellationToken)
        {
            if (request.TrialType == 0)
            {
                NotifyValidationErrors(typeof(CreateTrialDataCommand), new List<string>() { string.Format(Message.not_empty, "TrialType") });
                return null;
            }
            else if (request.BranchId == 0)
            {
                NotifyValidationErrors(typeof(CreateTrialDataCommand), new List<string>() { string.Format(Message.not_empty, "BranchId") });
                return null;
            }
            else if (request.UserId1 == 0)
            {
                NotifyValidationErrors(typeof(CreateTrialDataCommand), new List<string>() { string.Format(Message.not_empty, "UserId1") });
                return null;
            }
            else if (request.UserId2 == 0)
            {
                NotifyValidationErrors(typeof(CreateTrialDataCommand), new List<string>() { string.Format(Message.not_empty, "UserId2") });
                return null;
            }
            else if (request.UserIdAdmin == 0)
            {
                NotifyValidationErrors(typeof(CreateTrialDataCommand), new List<string>() { string.Format(Message.not_empty, "UserIdAdmin") });
                return null;
            }
            else if (string.IsNullOrEmpty(request.TenantCode))
            {
                NotifyValidationErrors(typeof(CreateTrialDataCommand), new List<string>() { string.Format(Message.not_empty, "TenantCode") });
                return null;
            }
            else if (request.TenantId == 0)
            {
                NotifyValidationErrors(typeof(CreateTrialDataCommand), new List<string>() { string.Format(Message.not_empty, "TenantId") });
                return null;
            }
            else if (request.GroupId == 0)
            {
                NotifyValidationErrors(typeof(CreateTrialDataCommand), new List<string>() { string.Format(Message.not_empty, "GroupId") });
                return null;
            }


            var cacheKeys = CacheKeys.GetListEntityCacheKey(request.TenantCode, nameof(Employee), "*");
            _cacheClient.RemoveByParttern(cacheKeys);

            var isSuccess = false;
            switch (request.TrialType)
            {
                case 3:
                    // tạo bảng hoa hồng
                    var commission = new CommissionDto() { Name = "Hoa hồng dịch vụ - tư vấn", IsActive = true, IsAllBranch = true };
                    var returnObj = await _mediator.Send(new CreateCommissionDataTrialCommand(commission));
                    if (returnObj == null)
                    {
                        return null;
                    }

                    //tạo bảng hoa hồng chi tiết
                    await _mediator.Send(new CreateCommissionDetailByProductCategoryDataTrialCommand(new List<long> { returnObj.Id }, new ProductCategoryReqDto() { Id = 0, Name = null }, request.UserIdAdmin, request.TenantId, request.GroupId));
                    isSuccess = await _trialDataReadOnlyRepository.CreateBookingTrialDataAsync(request.TenantId, request.BranchId, request.UserIdAdmin, request.UserId1, request.UserId2, returnObj.Id);

                    if (isSuccess)
                    {
                        var result = await _employeeReadOnlyRepository.GetBySpecificationAsync(new FindEmployeeByBranchIdSpec(request.BranchId));
                        var employees = _mapper.Map<List<EmployeeDto>>(result);
                        foreach (var employee in employees)
                        {
                            if (employee.Code == "NV000001") continue;
                            employee.WorkBranchIds = new List<int>() { request.BranchId };
                            await HandleNotifyChangeAsync(employee);
                        }
                    }
                    break;
                default:
                    break;
            }

            return new { IsSuccess = isSuccess };
        }
        private async Task HandleNotifyChangeAsync(EmployeeDto employeeDto)
        {
            if (employeeDto == null || employeeDto.Id <= 0)
            {
                _logger.LogError("[HandleNotifyChange] Create Employee has empty data");
                return;
            }
            try
            {

                using (var tokenSource = new CancellationTokenSource(Constant.MillisecondsDelay))
                {
                    var employeeBranches = new List<ChangeEmployeeBranchReq>();

                    if (employeeDto.WorkBranchIds != null)
                    {
                        foreach (var item in employeeDto.WorkBranchIds)
                        {
                            employeeBranches.Add(new ChangeEmployeeBranchReq()
                            {
                                BranchId = item,
                                RetailerId = employeeDto.TenantId,
                                EmployeeId = employeeDto.Id
                            });
                        }
                    }

                    var req = new OnChangeEmployeeReq()
                    {
                        RetailerId = employeeDto.TenantId,
                        Employee = new ChangeEmployeeReq(employeeDto, employeeBranches)
                    };
                    var resp = await _kiotVietServiceClient.OnChangeEmployee(req, tokenSource.Token);
                    var log = new
                    {
                        ReqEmployee = req.Employee,
                        Message = resp?.ResponseStatus?.Message
                    };
                    _logger.LogInformation($"[HandleNotifyChange] Create Employee Success: { log.ToSafeJson()}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
            }
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using FluentValidation.Results;
using KiotVietTimeSheet.Application.Abstractions;
using KiotVietTimeSheet.Application.Auth;
using KiotVietTimeSheet.Application.Configuration;
using KiotVietTimeSheet.Application.Dto;
using KiotVietTimeSheet.Application.EventBus.Events.EmployeeEvents;
using KiotVietTimeSheet.Application.Repositories.ReadRepositories;
using KiotVietTimeSheet.Application.Repositories.WriteRepositories;
using KiotVietTimeSheet.Application.Runtime.Exception;
using KiotVietTimeSheet.Application.Service.Interfaces;
using KiotVietTimeSheet.Application.ServiceClients;
using KiotVietTimeSheet.Application.ServiceClients.RequestModels;
using KiotVietTimeSheet.Application.Validators.EmployeeValidators;
using KiotVietTimeSheet.Application.Validators.PayRateValidators;
using KiotVietTimeSheet.Domain.AggregatesModels.EmployeeAggregate.Enums;
using KiotVietTimeSheet.Domain.AggregatesModels.EmployeeAggregate.Models;
using KiotVietTimeSheet.Domain.AggregatesModels.PayRateAggregate.Models;
using KiotVietTimeSheet.Domain.AggregatesModels.ShiftAggregate.Models;
using KiotVietTimeSheet.Domain.AggregatesModels.ShiftAggregate.Specifications;
using KiotVietTimeSheet.Domain.Common;
using KiotVietTimeSheet.Domain.Engines.SalaryEngine.Helpers;
using KiotVietTimeSheet.Domain.Engines.SalaryEngine.Rules.Deduction;
using KiotVietTimeSheet.SharedKernel.Domain;
using KiotVietTimeSheet.SharedKernel.Models;
using KiotVietTimeSheet.SharedKernel.Notification;
using KiotVietTimeSheet.Utilities;
using MediatR;
using Microsoft.Extensions.Logging;
using ServiceStack;

namespace KiotVietTimeSheet.Application.Commands.CreateEmployee
{
    public class CreateEmployeeCommandHandler : BaseCommandHandler,
        IRequestHandler<CreateEmployeeCommand, EmployeeDto>
    {
        private readonly IMapper _mapper;
        private readonly IAuthService _authService;
        private readonly IEmployeeWriteOnlyRepository _employeeWriteOnlyRepository;
        private readonly IAllowanceReadOnlyRepository _allowanceReadOnlyRepository;
        private readonly IDeductionReadOnlyRepository _deductionReadOnlyRepository;
        private readonly ICommissionReadOnlyRepository _commissionReadOnlyRepository;
        private readonly ICommissionBranchReadOnlyRepository _commissionBranchReadOnlyRepository;
        private readonly IPayRateTemplateReadOnlyRepository _payRateTemplateReadOnlyRepository;
        private readonly IShiftReadOnlyRepository _shiftReadOnlyRepository;
        private readonly IPayRateWriteOnlyRepository _payRateWriteOnlyRepository;
        private readonly ITimeSheetIntegrationEventService _timeSheetIntegrationEventService;
        private readonly IApplicationConfiguration _applicationConfiguration;
        private readonly IKiotVietServiceClient _kiotVietServiceClient;
        private readonly ILogger<CreateEmployeeCommandHandler> _logger;
        private readonly IEmployeeBranchReadOnlyRepository _employeeBranchReadOnlyRepository;
        private readonly IEmployeeReadOnlyRepository _employeeReadOnlyRepository;
        public CreateEmployeeCommandHandler(
            IEventDispatcher eventDispatcher,
            IMapper mapper,
            IAuthService authService,
            IEmployeeWriteOnlyRepository employeeWriteOnlyRepository,
            IPayRateTemplateReadOnlyRepository payRateTemplateReadOnlyRepository,
            IAllowanceReadOnlyRepository allowanceReadOnlyRepository,
            IDeductionReadOnlyRepository deductionReadOnlyRepository,
            ICommissionReadOnlyRepository commissionReadOnlyRepository,
            ICommissionBranchReadOnlyRepository commissionBranchReadOnlyRepository,
            IShiftReadOnlyRepository shiftReadOnlyRepository,
            IPayRateWriteOnlyRepository payRateWriteOnlyRepository,
            ITimeSheetIntegrationEventService timeSheetIntegrationEventService,
            IApplicationConfiguration applicationConfiguration,
            IKiotVietServiceClient kiotVietServiceClient,
            ILogger<CreateEmployeeCommandHandler> logger,
            IEmployeeBranchReadOnlyRepository employeeBranchReadOnlyRepository,
            IEmployeeReadOnlyRepository employeeReadOnlyRepository)
            : base(eventDispatcher)
        {
            _mapper = mapper;
            _authService = authService;
            _employeeWriteOnlyRepository = employeeWriteOnlyRepository;
            _payRateTemplateReadOnlyRepository = payRateTemplateReadOnlyRepository;
            _allowanceReadOnlyRepository = allowanceReadOnlyRepository;
            _deductionReadOnlyRepository = deductionReadOnlyRepository;
            _commissionReadOnlyRepository = commissionReadOnlyRepository;
            _commissionBranchReadOnlyRepository = commissionBranchReadOnlyRepository;
            _shiftReadOnlyRepository = shiftReadOnlyRepository;
            _payRateWriteOnlyRepository = payRateWriteOnlyRepository;
            _timeSheetIntegrationEventService = timeSheetIntegrationEventService;
            _applicationConfiguration = applicationConfiguration;
            _kiotVietServiceClient = kiotVietServiceClient;
            _logger = logger;
            _employeeBranchReadOnlyRepository = employeeBranchReadOnlyRepository;
            _employeeReadOnlyRepository = employeeReadOnlyRepository;
        }

        public bool CheckRequestCondition(CreateEmployeeCommand request, List<int> employeeDtoWorkBranchIds)
        {
            return request.TypeInsert != null && request.TypeInsert == (int)EmployeeTypeGet.Attendance &&
                   employeeDtoWorkBranchIds != null && employeeDtoWorkBranchIds.Any();
        }

        public async Task<List<Shift>> GetShiftFromMainSalaries(CreateEmployeeCommand request)
        {
            var shiftFromMainSalaries = new List<Shift>();

            if (request.PayRateDto.MainSalaryRuleValue?.MainSalaryValueDetails != null &&
                request.PayRateDto.MainSalaryRuleValue.MainSalaryValueDetails.Any())
            {
                var shiftIdInMainSalary = request.PayRateDto.MainSalaryRuleValue.MainSalaryValueDetails
                    .Where(x => x.ShiftId != 0).Select(x => x.ShiftId).ToList();

                shiftFromMainSalaries = await _shiftReadOnlyRepository.GetBySpecificationAsync(
                    new FindShiftByShiftIdsSpec(shiftIdInMainSalary), false, true);
            }

            return shiftFromMainSalaries;
        }

        public async Task<EmployeeDto> Handle(CreateEmployeeCommand request, CancellationToken cancellationToken)
        {
            var employeeDto = request.EmployeeDto;
            var isNotUpdateUserId = employeeDto.IsNotUpdateUserId;
            var viewModel = _mapper.Map<Employee>(employeeDto);
            long? viewModelUserId = viewModel.UserId < 1 ? null : viewModel.UserId;
            var employee = new Employee(
               viewModel.Code,
               viewModel.Name,
               viewModel.DOB,
               viewModel.Gender,
               viewModel.IdentityNumber,
               viewModel.MobilePhone,
               viewModel.Email,
               viewModel.Facebook,
               viewModel.Address,
               viewModel.LocationName,
               viewModel.WardName,
               viewModel.Note,
               viewModel.DepartmentId,
               viewModel.JobTitleId,
               viewModel.ProfilePictures,
               isNotUpdateUserId ? null : viewModelUserId,
               employeeDto.WorkBranchIds
            )
            {
                NickName = viewModel.NickName
            };
            var validator = await (new CreateOrUpdateEmployeeValidator(
                _employeeWriteOnlyRepository,
                _payRateTemplateReadOnlyRepository,
                _authService,
                _employeeReadOnlyRepository,
                new ValidatorParamsObj(request.BlockUnit,
                _applicationConfiguration.EmployeesPerBlock, isChangeMobileNumber: true, sourcePayRateTemplateId:request.PayRateDto?.PayRateTemplateId ?? 0)
                ).ValidateAsync(employee, cancellationToken));
            if (!validator.IsValid)
            {
                NotifyValidationErrors(typeof(Employee), validator.Errors);
                return null;
            }

            //check permission for other branch in attendance
            if (!CheckRequestCondition(request, employeeDto.WorkBranchIds))
            {
                if (_authService.Context.BranchId == employeeDto.WorkBranchIds[0] && !await _authService.HasAnyPermissionMapWithBranchId(new[] { TimeSheetPermission.Employee_Create }, employeeDto.WorkBranchIds[0]))
                {
                    throw new KvTimeSheetUnAuthorizedException($"Người dùng {_authService.Context.User.UserName} không có quyền thực hiện");
                }
                employee.BranchId = employeeDto.BranchId;
            }
            else
            {
                //check create new employee with old version of app cham cong
                if (employeeDto.WorkBranchIds == null || employeeDto.WorkBranchIds.Count == 0)
                {
                    employeeDto.WorkBranchIds = new List<int>
                    {
                        _authService.Context.BranchId
                    };
                    employee.EmployeeBranches = employeeDto.WorkBranchIds.Select(branchId => new EmployeeBranch(branchId, 0)).ToList();
                }
            }

            _employeeWriteOnlyRepository.Add(employee);

            PayRate payRate = null;
            if (request.PayRateDto != null && await _authService.HasPermissions(new[] { TimeSheetPermission.PayRate_Update }))
            {
                var rules = SalaryRuleHelpers.GetRulesFromObjectByRuleValue(request.PayRateDto);
                payRate = new PayRate(
                    employee.Id,
                    request.PayRateDto.PayRateTemplateId,
                    request.PayRateDto.SalaryPeriod,
                    rules
                );
                var shiftFromMainSalaries = await GetShiftFromMainSalaries(request);

                var payRateValidator =
                    await new PayRateCreateOrUpdateValidator(
                            rules,
                            _payRateTemplateReadOnlyRepository,
                            _allowanceReadOnlyRepository,
                            _deductionReadOnlyRepository,
                            _commissionReadOnlyRepository,
                            _commissionBranchReadOnlyRepository,
                            employee,
                            employeeDto.WorkBranchIds,
                            shiftFromMainSalaries
                        )
                        .ValidateAsync(payRate);
                if (!payRateValidator.IsValid)
                {
                    NotifyValidationErrors(typeof(PayRate), payRateValidator.Errors);
                    return null;
                }

                _payRateWriteOnlyRepository.Add(payRate);
            }

            await _timeSheetIntegrationEventService.AddEventAsync(new CreatedEmployeeIntegrationEvent(employee, payRate, employeeDto.WorkBranchIds));
            await _employeeWriteOnlyRepository.UnitOfWork.CommitAsync();
            var result = _mapper.Map<EmployeeDto>(employee);
            result.WorkBranchIds = employeeDto.WorkBranchIds;
            await HandleNotifyChangeAsync(result);

            return result;
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

    public class CreateSyncEmployeeCommandHandler : IRequestHandler<CreateSyncEmployeeCommand, SyncEmployeeDto>
    {
        private readonly IEmployeeWriteOnlyRepository _employeeWriteOnlyRepository;
        private readonly IMapper _mapper;
        private readonly IApplicationConfiguration _applicationConfiguration;
        private readonly IEventDispatcher _eventDispatcher;
        private readonly IAuthService _authService;
        private readonly IPayRateWriteOnlyRepository _payRateWriteOnlyRepository;
        private readonly ILogger<CreateSyncEmployeeCommandHandler> _logger;
        private readonly IEmployeeReadOnlyRepository _employeeReadOnlyRepository;

        public CreateSyncEmployeeCommandHandler(IEmployeeWriteOnlyRepository employeeWriteOnlyRepository,
            IPayRateWriteOnlyRepository payRateWriteOnlyRepository,
            IAuthService authService,
            ILogger<CreateSyncEmployeeCommandHandler> logger,
            IMapper mapper, IApplicationConfiguration applicationConfiguration, IEventDispatcher eventDispatcher,
            IEmployeeReadOnlyRepository employeeReadOnlyRepository)
        {
            _employeeWriteOnlyRepository = employeeWriteOnlyRepository;
            _mapper = mapper;
            _applicationConfiguration = applicationConfiguration;
            _eventDispatcher = eventDispatcher;
            _authService = authService;
            _logger = logger;
            _payRateWriteOnlyRepository = payRateWriteOnlyRepository;
            _employeeReadOnlyRepository = employeeReadOnlyRepository;
        }

        void NotifyValidationErrors(string name, IList<ValidationFailure> errors)
        {
            foreach (var error in errors)
            {
                _eventDispatcher.FireEvent(new DomainNotification(name, new ErrorResult()
                {
                    Code = error.ErrorCode,
                    Message = error.ErrorMessage,
                }));
            }
        }
        public async Task<SyncEmployeeDto> Handle(CreateSyncEmployeeCommand request, CancellationToken cancellationToken)
        {
            var employeeDto = request.SyncEmployeeDto;
            var viewModel = _mapper.Map<Employee>(employeeDto);
            long? viewModelUserId = viewModel.UserId < 1 ? null : viewModel.UserId;
            var employee = new Employee(
                viewModel.Code,
                viewModel.Name,
                viewModel.DOB,
                viewModel.Gender,
                viewModel.IdentityNumber,
                viewModel.MobilePhone,
                viewModel.Email,
                viewModel.Facebook,
                viewModel.Address,
                viewModel.LocationName,
                viewModel.WardName,
                viewModel.Note,
                viewModel.DepartmentId,
                viewModel.JobTitleId,
                viewModel.ProfilePictures,
                viewModelUserId,
                viewModel.EmployeeBranches != null && viewModel.EmployeeBranches.Any() ? viewModel.EmployeeBranches.Select(p => p.BranchId).ToList() : new List<int>()
            )
            {
                BranchId = request.SyncEmployeeDto.BranchId
            };
            // standardize mobile phone
            employee.StandardizedMobilePhone = PhoneNumberHelper.StandardizePhoneNumber(employee.MobilePhone, true);
            var validator = await (new CreateOrUpdateEmployeeValidator(
                _employeeWriteOnlyRepository,
                _authService,
                _employeeReadOnlyRepository,
                new ValidatorParamsObj(request.BlockUnit,
                _applicationConfiguration.EmployeesPerBlock, isChangeMobileNumber: true, isSync: true)
                ).ValidateAsync(employee, cancellationToken));
            if (!validator.IsValid)
            {
                NotifyValidationErrors(nameof(Employee), validator.Errors);
                return null;
            }
            //add payrate default
            

            _authService.Context.TenantCode = request.RetailerCode;
            _employeeWriteOnlyRepository.AddEntityNotGuard(employee);
            try
            {
                var payRate = GetPayRateDefaultForEmployee(employee);
                _payRateWriteOnlyRepository.AddEntityNotGuard(payRate);
            }
            catch(Exception epx) { }

            await _employeeWriteOnlyRepository.UnitOfWork.CommitAsync();
             
            var result = _mapper.Map<SyncEmployeeDto>(employee);
            return result;
        }

        private PayRate GetPayRateDefaultForEmployee(Employee employee)
        {
            var payRateDto = new PayRateDto()
            {
                SalaryPeriod = 1,
                DeductionRuleValue =new DeductionRuleValue()
                {
                    DeductionRuleValueDetails = new List<DeductionRuleValueDetail>()
                }
            };

            var rules = SalaryRuleHelpers.GetRulesFromObjectByRuleValue(payRateDto);

            var payRate = new PayRate(employee.Id, payRateDto.PayRateTemplateId, payRateDto.SalaryPeriod, rules)
            {
                TenantId = employee.TenantId,
                CreatedDate = DateTime.Now,
                CreatedBy = employee.CreatedBy
            };
            return payRate;
        }
    }
}

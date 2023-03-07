using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using KiotVietTimeSheet.Application.Abstractions;
using KiotVietTimeSheet.Application.Auth;
using KiotVietTimeSheet.Application.Configuration;
using KiotVietTimeSheet.Application.Dto;
using KiotVietTimeSheet.Application.EventBus.Events.EmployeeEvents;
using KiotVietTimeSheet.Application.Repositories.ReadRepositories;
using KiotVietTimeSheet.Application.Repositories.WriteRepositories;
using KiotVietTimeSheet.Application.Service.Interfaces;
using KiotVietTimeSheet.Application.ServiceClients;
using KiotVietTimeSheet.Application.ServiceClients.RequestModels;
using KiotVietTimeSheet.Application.Validators.EmployeeValidators;
using KiotVietTimeSheet.Application.Validators.PayRateValidators;
using KiotVietTimeSheet.Domain.AggregatesModels.EmployeeAggregate.Models;
using KiotVietTimeSheet.Domain.AggregatesModels.EmployeeAggregate.Specifications;
using KiotVietTimeSheet.Domain.AggregatesModels.PayRateAggregate.Models;
using KiotVietTimeSheet.Domain.AggregatesModels.ShiftAggregate.Models;
using KiotVietTimeSheet.Domain.Common;
using KiotVietTimeSheet.Domain.Engines.SalaryEngine.Helpers;
using KiotVietTimeSheet.Resources;
using KiotVietTimeSheet.SharedKernel.Domain;
using KiotVietTimeSheet.SharedKernel.Notification;
using KiotVietTimeSheet.Utilities;
using MediatR;
using Microsoft.Extensions.Logging;
using ServiceStack;

namespace KiotVietTimeSheet.Application.Commands.UpdateEmployee
{
    public class UpdateEmployeeCommandHandler : BaseCommandHandler,
        IRequestHandler<UpdateEmployeeCommand, EmployeeDto>
    {
        private readonly IMapper _mapper;
        private readonly IAuthService _authService;
        private readonly IEventDispatcher _eventDispatcher;
        private readonly IEmployeeProfilePictureWriteOnlyRepository _employeeProfilePictureWriteOnlyRepository;
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
        private readonly IEmployeeBranchWriteOnlyRepository _employeeBranchWriteOnlyRepository;
        private readonly IEmployeeBranchReadOnlyRepository _employeeBranchReadOnlyRepository;
        private readonly IPosParamService _posParamService;
        private readonly IKiotVietServiceClient _kiotVietServiceClient;
        private readonly ILogger<UpdateEmployeeCommandHandler> _logger;
        private readonly IEmployeeReadOnlyRepository _employeeReadOnlyRepository;
        public UpdateEmployeeCommandHandler(
            IAuthService authService,
            IEventDispatcher eventDispatcher,
            IEmployeeProfilePictureWriteOnlyRepository employeeProfilePictureWriteOnlyRepository,
            IMapper mapper,
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
            IEmployeeBranchWriteOnlyRepository employeeBranchWriteOnlyRepository,
            IEmployeeBranchReadOnlyRepository employeeBranchReadOnlyRepository,
            IPosParamService posParamService,
            IKiotVietServiceClient kiotVietServiceClient,
            ILogger<UpdateEmployeeCommandHandler> logger,
            IEmployeeReadOnlyRepository employeeReadOnlyRepository
            )
            : base(eventDispatcher)
        {
            _authService = authService;
            _eventDispatcher = eventDispatcher;
            _employeeProfilePictureWriteOnlyRepository = employeeProfilePictureWriteOnlyRepository;
            _mapper = mapper;
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
            _employeeBranchWriteOnlyRepository = employeeBranchWriteOnlyRepository;
            _employeeBranchReadOnlyRepository = employeeBranchReadOnlyRepository;
            _posParamService = posParamService;
            _kiotVietServiceClient = kiotVietServiceClient;
            _logger = logger;
            _employeeReadOnlyRepository = employeeReadOnlyRepository;
        }

        public long? GetUserId(bool isNotUpdateUserId, long? employeeUserId, long? viewModelUserId)
        {
            if (isNotUpdateUserId)
                return employeeUserId;
            if (viewModelUserId < 1)
                return null;
            return viewModelUserId;
        }

        public async Task<List<Shift>> GetShiftFromMainSalaries(PayRateDto payRateDto, List<int> employeeDtoWorkBranchIds)
        {
            var shiftFromMainSalaries = new List<Shift>();

            if (payRateDto.MainSalaryRuleValue?.MainSalaryValueDetails != null &&
                payRateDto.MainSalaryRuleValue.MainSalaryValueDetails.Any())
            {
                var shiftIdInMainSalary = payRateDto.MainSalaryRuleValue.MainSalaryValueDetails
                    .Where(x => x.ShiftId != 0).Select(x => x.ShiftId).ToList();

                shiftFromMainSalaries = await _shiftReadOnlyRepository.GetShiftMultipleBranchOrderByFromAndTo(employeeDtoWorkBranchIds, null, shiftIdInMainSalary);
            }

            return shiftFromMainSalaries;
        }

        public async Task UpdateEmployeeBranch(Employee employee, EmployeeDto employeeDto)
        {
            var deleteEmployeeBranchList =
               (await _employeeBranchReadOnlyRepository.GetAllAsync()).Where(x => x.EmployeeId == employee.Id && !employeeDto.WorkBranchIds.Contains(x.BranchId)).ToList();
            var existedEmployeeBranchList =
                (await _employeeBranchReadOnlyRepository.GetAllAsync()).Where(x => x.EmployeeId == employee.Id && employeeDto.WorkBranchIds.Contains(x.BranchId)).ToList();
            if (deleteEmployeeBranchList.Any())
            {
                _employeeBranchWriteOnlyRepository.BatchDelete(deleteEmployeeBranchList);
            }
            var newBranches = employeeDto.WorkBranchIds != null && employeeDto.WorkBranchIds.Count > 0
                ? employeeDto.WorkBranchIds.Where(id => existedEmployeeBranchList.All(y => y.BranchId != id)).Select(branchId => new EmployeeBranch(branchId, employee.Id)).ToList()
                : null;
            if (newBranches != null && newBranches.Count > 0)
                _employeeBranchWriteOnlyRepository.BatchAdd(newBranches);
        }

        public async Task DeleteProfilePicture(long employeeId)
        {
            var employeeProfilePicture = await _employeeProfilePictureWriteOnlyRepository.GetBySpecificationAsync(new GetProfilePicturesByEmployeeIdSpec(employeeId));
            if (employeeProfilePicture != null)
            {
                _employeeProfilePictureWriteOnlyRepository.BatchDelete(employeeProfilePicture);
            }
        }

        public async Task<EmployeeDto> Handle(UpdateEmployeeCommand request, CancellationToken cancellationToken)
        {
            var employeeDto = request.Employee;
            var payRateDto = request.PayRate;
            var employee = await _employeeWriteOnlyRepository.FindByIdAsync(employeeDto.Id);
            var oldEmployee = employee.CreateCopy();

            if (employee == null)
            {
                await _eventDispatcher.FireEvent(new DomainNotification(typeof(Employee).Name, string.Format(Message.not_exists, Label.employee)));
                return null;
            }
            var isChangeCode = employee.Code != employeeDto.Code;
            var isNotUpdateUserId = employeeDto.IsNotUpdateUserId;
            var viewModel = _mapper.Map<Employee>(employeeDto);

            employee.Update(
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
                GetUserId(isNotUpdateUserId, employee.UserId, viewModel.UserId),
                viewModel.IdentityKeyClocking
            );
            employee.NickName = viewModel.NickName;
            // standardize mobile phone
            employee.StandardizedMobilePhone = PhoneNumberHelper.StandardizePhoneNumber(employee.MobilePhone, true);
            var validator = await (new CreateOrUpdateEmployeeValidator(
                _employeeWriteOnlyRepository,
                _payRateTemplateReadOnlyRepository,
                _authService,
                _employeeReadOnlyRepository,
                new ValidatorParamsObj(request.BlockUnit,
                _applicationConfiguration.EmployeesPerBlock,
                isChangeCode, true, sourcePayRateTemplateId:payRateDto?.PayRateTemplateId ?? 0)
                ).ValidateAsync(employee, cancellationToken));
            if (!validator.IsValid)
            {
                NotifyValidationErrors(typeof(Employee), validator.Errors.Select(e => e.ErrorMessage).ToList());
                return null;
            }

            _employeeWriteOnlyRepository.Update(employee);
            //Update employee branch
            await UpdateEmployeeBranch(employee, employeeDto);
            //End update employee branch

            _employeeProfilePictureWriteOnlyRepository.BatchAdd(employee.ProfilePictures);

            await DeleteProfilePicture(employee.Id);

            PayRate payRate = null;
            if (await _authService.HasPermissions(new[] { TimeSheetPermission.PayRate_Update }) && payRateDto != null)
            {
                var rules = SalaryRuleHelpers.GetRulesFromObjectByRuleValue(payRateDto);
                var existPayRate = await _payRateWriteOnlyRepository.FindBySpecificationAsync(new FindPayRateByEmployeeIdSpec(employee.Id), "PayRateDetails");
                var shiftFromMainSalaries = await GetShiftFromMainSalaries(payRateDto, employeeDto.WorkBranchIds);

                if (existPayRate != null)
                {
                    existPayRate.Update(payRateDto.PayRateTemplateId, payRateDto.SalaryPeriod, rules);
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
                            .ValidateAsync(existPayRate, cancellationToken);

                    if (!payRateValidator.IsValid)
                    {
                        NotifyValidationErrors(typeof(PayRate), payRateValidator.Errors);
                        return null;
                    }

                    payRate = existPayRate;
                    await _payRateWriteOnlyRepository.UpdatePayRateAsync(existPayRate);
                }
                else
                {
                    payRate = new PayRate(
                        employee.Id,
                        payRateDto.PayRateTemplateId,
                        payRateDto.SalaryPeriod,
                        rules
                    );
                    _payRateWriteOnlyRepository.Add(payRate);
                }
            }

            await _timeSheetIntegrationEventService.AddEventAsync(new UpdatedEmployeeIntegrationEvent(
                oldEmployee,
                employee,
                payRate,
                employeeDto.WorkBranchIds)
            );

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
                _logger.LogError("[HandleNotifyChange] Update Employee has empty data");
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
                    _logger.LogInformation($"[HandleNotifyChange] Update Employee Success: {log.ToSafeJson()}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
            }
        }
    }
}

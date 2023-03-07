using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using KiotVietTimeSheet.Application.Abstractions;
using KiotVietTimeSheet.Application.Auth;
using KiotVietTimeSheet.Application.Dto;
using KiotVietTimeSheet.Application.EventBus.Events.SettingEvents;
using KiotVietTimeSheet.Application.Queries.GetSetting;
using KiotVietTimeSheet.Application.Repositories.ReadRepositories;
using KiotVietTimeSheet.Application.Repositories.WriteRepositories;
using KiotVietTimeSheet.Application.Validators.SettingValidators;
using KiotVietTimeSheet.Domain.AggregatesModels.PaysheetAggregate.Enum;
using KiotVietTimeSheet.Domain.AggregatesModels.SettingsAggregate.Models;
using KiotVietTimeSheet.Domain.AggregatesModels.SettingsAggregate.Specifications;
using KiotVietTimeSheet.SharedKernel.Domain;
using MediatR;
using Microsoft.Extensions.Logging;
using ServiceStack;

namespace KiotVietTimeSheet.Application.Commands.CreateOrUpdateSetting
{
    public class CreateOrUpdateSettingCommandHandler : BaseCommandHandler,
        IRequestHandler<CreateOrUpdateSettingCommand, SettingsDto>
    {
        private readonly IAuthService _authService;
        private readonly ISettingsWriteOnlyRepository _settingsWriteOnlyRepository;
        private readonly CreateOrUpdateSettingValidator _createOrUpdateSettingValidator;
        private readonly ITimeSheetIntegrationEventService _timeSheetIntegrationEventService;
        private readonly IMediator _mediator;
        private readonly ILogger<CreateOrUpdateSettingCommandHandler> _logger;
        private readonly IPayRateTemplateReadOnlyRepository _payRateTemplateReadOnlyRepository;

        public CreateOrUpdateSettingCommandHandler(
            IEventDispatcher eventDispatcher,
            IAuthService authService,
            ISettingsWriteOnlyRepository settingsWriteOnlyRepository,
            CreateOrUpdateSettingValidator createOrUpdateSettingValidator,
            ITimeSheetIntegrationEventService timeSheetIntegrationEventService,
            IMediator mediator,
            IPayRateTemplateReadOnlyRepository payRateTemplateReadOnlyRepository,
            ILogger<CreateOrUpdateSettingCommandHandler> logger
        )
            : base(eventDispatcher)
        {
            _authService = authService;
            _settingsWriteOnlyRepository = settingsWriteOnlyRepository;
            _createOrUpdateSettingValidator = createOrUpdateSettingValidator;
            _timeSheetIntegrationEventService = timeSheetIntegrationEventService;
            _mediator = mediator;
            _logger = logger;
            _payRateTemplateReadOnlyRepository = payRateTemplateReadOnlyRepository;
        }

        public async Task<SettingsDto> Handle(CreateOrUpdateSettingCommand request, CancellationToken cancellationToken)
        {
            var settingDto = request.Setting;
            var resultValidate = await _createOrUpdateSettingValidator.ValidateAsync(settingDto, cancellationToken);

            if (!resultValidate.IsValid)
            {
                NotifyValidationErrors(typeof(Settings), resultValidate.Errors.Select(e => e.ErrorMessage).ToList());
                return null;
            }
            var settingItemList = settingDto.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public)
                .ToDictionary(prop => prop.Name, prop => prop.GetValue(settingDto, null).ToString());
            var lsSetting = await _settingsWriteOnlyRepository.GetBySpecificationAsync(new FindSettingByTenantIdSpec(_authService.Context.TenantId));
            var addSettingList = new List<Settings>();
            var isUpdateCronSchedule = false;
            Settings allowAutoKeepingSetting = null;
            foreach (var item in settingItemList)
            {
                // Không cập nhật UseClockingGps ở đây. Có nơi update riêng
                
                if (string.IsNullOrEmpty(item.Key) ||
                    item.Key == nameof(SettingsToObject.UseClockingGps)) continue;
                var setting = lsSetting.FirstOrDefault(x => x.Name == item.Key);
                if (setting != null)
                {
                    if (item.Key.Equals("AllowAutoKeeping", System.StringComparison.OrdinalIgnoreCase))
                    {
                        if (!item.Value.Equals(setting.Value))
                        {
                            isUpdateCronSchedule = true;
                        }    
                        allowAutoKeepingSetting = setting.CreateCopy();
                        allowAutoKeepingSetting.Value = item.Value;
                    }    
                    setting.Update(item.Value);
                }
                else
                {
                    var newSetting = Settings.CreateInstance(_authService.Context.TenantId, item.Key, item.Value);
                    addSettingList.Add(newSetting);

                    if (item.Key.Equals("AllowAutoKeeping", System.StringComparison.OrdinalIgnoreCase)){
                        allowAutoKeepingSetting = newSetting.CreateCopy();
                        allowAutoKeepingSetting.Value = newSetting.Value;
                    }                    
                }
            }

            if (addSettingList.Any())
            {
                _settingsWriteOnlyRepository.BatchAdd(addSettingList);
            }

            if (lsSetting.Any())
            {
                _settingsWriteOnlyRepository.BatchUpdate(lsSetting);
            }

            var settingsObjectDto = await _mediator.Send(new GetSettingQuery(_authService.Context.TenantId), cancellationToken);

            if (await CheckPayrateTemplateImpactBySalaryPeriod(settingsObjectDto, settingDto))
            {
                settingDto.IsImpactPayrateTemplate = true;
            }

            await _timeSheetIntegrationEventService.AddEventAsync(new UpdatedSettingIntegrationEvent(
                settingsObjectDto,
                settingDto,
                request.SettingType)
            );

            await _settingsWriteOnlyRepository.UnitOfWork.CommitAsync();

            var isInsert = false;
            try
            {
                isInsert = await _settingsWriteOnlyRepository.FindAndInsertAutoKeepingCronScheduleAsync(allowAutoKeepingSetting);
            }
            catch (Exception epx)
            {
                _logger.LogError(epx.Message, epx);
            }

            if (isUpdateCronSchedule && !isInsert)
            {
                try
                {
                    await _settingsWriteOnlyRepository.UpdateAutoKeepingCronScheduleAsync(allowAutoKeepingSetting);
                }
                catch (Exception epx)
                {
                    _logger.LogError(epx.Message, epx);
                }
            }

            return settingDto;
        }

        private async Task<bool> CheckPayrateTemplateImpactBySalaryPeriod(SettingObjectDto oldSetting, SettingsDto newSetting)
        {
            var salaryPeriodLst = new List<byte>();
            if (oldSetting.IsDateOfEveryMonth != newSetting.IsDateOfEveryMonth && !newSetting.IsDateOfEveryMonth)
            {
                salaryPeriodLst.Add((byte)PaySheetWorkingPeriodStatuses.EveryMonth);
            }
            if (oldSetting.IsDayOfWeekEveryWeek != newSetting.IsDayOfWeekEveryWeek && !newSetting.IsDayOfWeekEveryWeek)
            {
                salaryPeriodLst.Add((byte)PaySheetWorkingPeriodStatuses.EveryWeek);
            }
            if (oldSetting.IsDateOfTwiceAMonth != newSetting.IsDateOfTwiceAMonth && !newSetting.IsDateOfTwiceAMonth)
            {
                salaryPeriodLst.Add((byte)PaySheetWorkingPeriodStatuses.TwiceAMonth);
            }
            if (oldSetting.IsDayOfWeekTwiceWeekly != newSetting.IsDayOfWeekTwiceWeekly && !newSetting.IsDayOfWeekTwiceWeekly)
            {
                salaryPeriodLst.Add((byte)PaySheetWorkingPeriodStatuses.TwiceWeekly);
            }
            if (salaryPeriodLst.Count > 0)
            {
                return await _payRateTemplateReadOnlyRepository.ExistPayrateTemplateBySalaryPeriod(salaryPeriodLst, _authService.Context.TenantId);
            }
            return false;
        }

    }
}

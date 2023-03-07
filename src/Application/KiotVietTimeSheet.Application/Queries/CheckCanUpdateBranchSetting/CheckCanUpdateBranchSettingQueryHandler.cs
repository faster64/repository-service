using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using KiotVietTimeSheet.Application.Abstractions;
using KiotVietTimeSheet.Application.Auth;
using KiotVietTimeSheet.Application.DomainService;
using KiotVietTimeSheet.Application.DomainService.Dto;
using KiotVietTimeSheet.Application.Dto;
using KiotVietTimeSheet.Application.Repositories.WriteRepositories;
using KiotVietTimeSheet.Domain.AggregatesModels.BranchSettingAggregate.Specifications;
using KiotVietTimeSheet.Domain.AggregatesModels.TimeSheetAggregate.Models;
using KiotVietTimeSheet.Domain.Common;
using KiotVietTimeSheet.Resources;
using KiotVietTimeSheet.SharedKernel.Domain;
using KiotVietTimeSheet.SharedKernel.Notification;
using MediatR;

namespace KiotVietTimeSheet.Application.Queries.CheckCanUpdateBranchSetting
{
    public class CheckCanUpdateBranchSettingQueryHandler : QueryHandlerBase,
        IRequestHandler<CheckCanUpdateBranchSettingQuery, bool>
    {
        private readonly IBranchSettingWriteOnlyRepository _branchSettingWriteOnlyRepository;
        private readonly IGetTimeSheetByBranchWorkingDaysDomainService _getTimeSheetByBranchWorkingDaysDomainService;
        private readonly IEventDispatcher _eventDispatcher;

        public CheckCanUpdateBranchSettingQueryHandler(
            IBranchSettingWriteOnlyRepository branchSettingWriteOnlyRepository,
            IGetTimeSheetByBranchWorkingDaysDomainService getTimeSheetByBranchWorkingDaysDomainService,
            IEventDispatcher eventDispatcher,
            IAuthService authService
        ) : base(authService)
        {
            _branchSettingWriteOnlyRepository = branchSettingWriteOnlyRepository;
            _getTimeSheetByBranchWorkingDaysDomainService = getTimeSheetByBranchWorkingDaysDomainService;
            _eventDispatcher = eventDispatcher;
        }

        public async Task<bool> Handle(CheckCanUpdateBranchSettingQuery request, CancellationToken cancellationToken)
        {
            BranchSettingDto branchSettingDto = request.BranchSettingDto;
            bool isAddMore = request.IsAddMore;
            bool isRemove = request.IsRemove;
            DateTime applyFrom = request.ApplyFrom;
            var branchSettingItem = await _branchSettingWriteOnlyRepository.FindBySpecificationAsync(new FindBranchSettingByBranchIdSpec(branchSettingDto.BranchId));
            if (branchSettingItem == null)
            {
                return true;
            }

            if (isAddMore || isRemove)
            {
                applyFrom = applyFrom.Date;
                var dayNeedAdd = branchSettingDto.WorkingDays.Except(branchSettingItem.WorkingDaysInArray).ToList();
                var dayNeedRemove = branchSettingItem.WorkingDaysInArray.Except(branchSettingDto.WorkingDays).ToList();
                var workingDaysChanged = new List<byte>();
                if (isAddMore)
                {
                    workingDaysChanged.AddRange(dayNeedAdd);
                }

                if (isRemove)
                {
                    workingDaysChanged.AddRange(dayNeedRemove);
                }

                var getTimeSheetByBranchWorkingDaysDto = new GetTimeSheetByBranchWorkingDaysDto()
                {
                    ApplyFrom = applyFrom,
                    BranchId = branchSettingItem.BranchId,
                    WorkingDays = workingDaysChanged
                };
                var timeSheets =
                    _getTimeSheetByBranchWorkingDaysDomainService.GetTimeSheetByBranchWorkingDay(getTimeSheetByBranchWorkingDaysDto);

                if (timeSheets != null && timeSheets.Count > Constant.MaximumTimeSheetCanUpdate)
                {
                    NotifyExceedMaximumTimeSheet();
                    return false;
                }
            }
            return true;
        }

        private void NotifyExceedMaximumTimeSheet()
        {
            _eventDispatcher.FireEvent(new DomainNotification(typeof(TimeSheet).Name, string.Format(Message.timeSheet_maximumSelectedTimeSheet, Constant.MaximumTimeSheetCanUpdate)));
        }
    }
}

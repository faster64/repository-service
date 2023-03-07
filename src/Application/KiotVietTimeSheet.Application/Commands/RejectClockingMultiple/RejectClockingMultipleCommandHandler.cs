using System;
using AutoMapper;
using KiotVietTimeSheet.Application.Abstractions;
using KiotVietTimeSheet.Application.Auth;
using KiotVietTimeSheet.Application.DomainService;
using KiotVietTimeSheet.Application.DomainService.Interfaces;
using KiotVietTimeSheet.Application.Dto;
using KiotVietTimeSheet.Application.EventBus.Events.ClockingEvents;
using KiotVietTimeSheet.Application.Repositories.WriteRepositories;
using KiotVietTimeSheet.Application.ServiceClients;
using KiotVietTimeSheet.Domain.AggregatesModels.ClockingAggregate.Models;
using KiotVietTimeSheet.Domain.AggregatesModels.ClockingAggregate.Specifications;
using KiotVietTimeSheet.Resources;
using KiotVietTimeSheet.SharedKernel.Domain;
using MediatR;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using KiotVietTimeSheet.Domain.AggregatesModels.ClockingAggregate.Enums;
using KiotVietTimeSheet.Domain.AggregatesModels.TimeSheetAggregate.Models;

namespace KiotVietTimeSheet.Application.Commands.RejectClockingMultiple
{
    public class RejectClockingMultipleCommandHandler : BaseCommandHandler,
        IRequestHandler<RejectClockingMultipleCommand, List<ClockingDto>>
    {
        private readonly IMapper _mapper;
        private readonly IClockingWriteOnlyRepository _clockingWriteOnlyRepository;
        private readonly IRejectClockingsDomainService _rejectClockingsService;
        private readonly IPaySheetOutOfDateDomainService _paySheetOutOfDateDomainService;
        private readonly ITimeSheetIntegrationEventService _timeSheetIntegrationEventService;
        private readonly IKiotVietServiceClient _kiotVietServiceClient;
        private readonly IAuthService _authService;

        public RejectClockingMultipleCommandHandler(
            IEventDispatcher eventDispatcher,
            IMapper mapper,
            IClockingWriteOnlyRepository clockingWriteOnlyRepository,
            IAuthService authService,
            IRejectClockingsDomainService rejectClockingsService,
            IPaySheetOutOfDateDomainService paySheetOutOfDateDomainService,
            ITimeSheetIntegrationEventService timeSheetIntegrationEventService,
            IKiotVietServiceClient kiotVietServiceClient
        )
            : base(eventDispatcher)
        {
            _mapper = mapper;
            _clockingWriteOnlyRepository = clockingWriteOnlyRepository;
            _rejectClockingsService = rejectClockingsService;
            _paySheetOutOfDateDomainService = paySheetOutOfDateDomainService;
            _timeSheetIntegrationEventService = timeSheetIntegrationEventService;
            _kiotVietServiceClient = kiotVietServiceClient;
            _authService = authService;
        }

        public async Task<List<ClockingDto>> Handle(RejectClockingMultipleCommand request, CancellationToken cancellationToken)
        {
            var listClockingId = request.ListClockingId;

            var clockings = await _clockingWriteOnlyRepository.GetBySpecificationAsync(new FindClockingByIdsSpec(listClockingId));
            bool isHavePermission = true;
            string branchName = string.Empty;

            foreach (var clocking in clockings)
            {
                isHavePermission = await IsHavePermissionOnBranch(clocking.BranchId, TimeSheetPermission.Clocking_Delete, _authService.Context.User.IsAdmin);
                if (!isHavePermission)
                {
                    var branch = await _kiotVietServiceClient.GetBranchById(clocking.BranchId);
                    if (branch != null)
                        branchName = branch.Name;
                    break;
                }
            }
            var errors = new List<string>();
            if (!isHavePermission)
            {
                errors.Add(string.Format(Message.clocking_rejectError, branchName));
                NotifyValidationErrors(typeof(Clocking), errors);
                return null;
            }

            if (!clockings.Any() || clockings.All(c => c.ClockingStatus == (byte)ClockingStatuses.Void))
            {
                if (listClockingId.Count > 1)
                {
                    errors.Add(Message.clocking_allHasBeenCancelled);
                    NotifyValidationErrors(typeof(Clocking), errors);
                }
                else
                {
                    errors.Add(Message.clocking_haveBeenCancelled);
                    NotifyValidationErrors(typeof(Clocking), errors);
                }

                return null;
            }

            var returnObj = await _rejectClockingsService.RejectClockingsAsync(clockings);
            if (!returnObj)
            {
                return null;
            }

            var timeSheet = new TimeSheet();
            timeSheet.SetRepeatWithDate(false, DateTime.Now, DateTime.Now);
            await _timeSheetIntegrationEventService.AddEventAsync(new RejectMultipleClockingIntegrationEvent(timeSheet, clockings));

            await _paySheetOutOfDateDomainService.WithClockingDataChangeAsync(clockings
                .Where(x => (x.CheckOutDate != null || x.AbsenceType == (byte)AbsenceTypes.AuthorisedAbsence) &&
                            x.ClockingPaymentStatus != (byte)ClockingPaymentStatuses.Paid).ToList());

            await _clockingWriteOnlyRepository.UnitOfWork.CommitAsync();

            var result = _mapper.Map<List<ClockingDto>>(clockings);
            return result;
        }
        async Task<bool> IsHavePermissionOnBranch(int branchId, string permissionName, bool isAdmin)
        {
            if (isAdmin)
                return true;
            var permission = await _kiotVietServiceClient.GetPermissionByBranchId(_authService.Context.User.Id, branchId);
            if (!permission.Data.ContainsKey(permissionName) || !permission.Data[permissionName])
            {
                return false;
            }
            return true;
        }
    }
}


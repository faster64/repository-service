using KiotVietTimeSheet.Api.ServiceModel;
using Microsoft.Extensions.Logging;
using MediatR;
using ServiceStack;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Message = KiotVietTimeSheet.Resources.Message;
using ServiceStack.Host;
using KiotVietTimeSheet.Api.ServiceModel.Types;
using KiotVietTimeSheet.Application.Commands.BatchAddTimeSheetWhenCreateMultipleTimeSheet;
using KiotVietTimeSheet.Application.Commands.CancelTimeSheet;
using KiotVietTimeSheet.Application.Commands.CopyListTimeSheet;
using KiotVietTimeSheet.Application.Commands.CreateTimesheet;
using KiotVietTimeSheet.Application.Commands.SendEmailActiveTrial;
using KiotVietTimeSheet.Application.Commands.UpdateTimesheet;
using KiotVietTimeSheet.Application.Dto;
using KiotVietTimeSheet.Application.Queries.GetAllPenalizeByTenantId;
using KiotVietTimeSheet.Application.Queries.GetBranchSetting;
using KiotVietTimeSheet.Application.Queries.GetHolidayInDateRange;
using KiotVietTimeSheet.Application.Queries.GetListTimesheet;
using KiotVietTimeSheet.Application.Queries.GetTimeSheetById;
using KiotVietTimeSheet.Application.Runtime.Exception;
using KiotVietTimeSheet.Domain.AggregatesModels.ClockingAggregate.Enums;
using KiotVietTimeSheet.SharedKernel.Notification;
using KiotVietTimeSheet.SharedKernel.Models;
using ServiceStack.Auth;
using ServiceStack.Configuration;

namespace KiotVietTimeSheet.Api.ServiceInterface
{
    public class TimeSheetApi : BaseApi
    {
        #region Properties
        private readonly IMediator _mediator;

        public IAutoQueryDb AutoQuery { get; set; }
        #endregion

        #region Constructors
        public TimeSheetApi(
            ILogger<TimeSheetApi> logger,
            INotificationHandler<DomainNotification> notificationHandler,
            IMediator mediator
        ) : base(logger, notificationHandler)
        {
            _mediator = mediator;
        }
        #endregion

        #region GET methods
        public async Task<object> Get(GetListTimeSheetReq req)
        {
            var query = AutoQuery.CreateQuery(req, Request.GetRequestParams());
            var result = await _mediator.Send(new GetListTimesheetQuery(query));

            return Ok(result);

        }

        public async Task<object> Get(GetVersionTimeSheetApp req)
        {
            var appSettings = HostContext.TryResolve<IAppSettings>();
            if (appSettings == null)
            {
                throw new KvTimeSheetException(Message.error_whenGetData);
            }

            var consulPrivateKey = appSettings.Get<string>("ConsulPrivateKey");
            var consulPublicKey = appSettings.Get<string>("ConsulPublicKey");
            var consulAuthKey = appSettings.Get<string>("ConsulAuthKeyBase64");
            var authProvider = new JwtAuthProvider()
            {
                AuthKeyBase64 = consulAuthKey,
                PrivateKeyXml = consulPrivateKey,
                PublicKeyXml = consulPublicKey
            };
            var jwt = authProvider.CreateJwtBearerToken(new AuthUserSession());
            var client = new JsonHttpClient(appSettings.GetString("ConsulGateWay"));
            client.AddHeader("Authorization", $"Bearer {jwt}");
            var response = await client.GetAsync<FingerMachineAppVersion>("/kv/kv-download-urls");
            if (response?.Item.GroupIds != null)
            {
                response.Item.GroupIds = new List<int>();
                for (int i = 1; i <= 9; i++)
                {
                    response.Item.GroupIds.Add(i);
                }
            }
            return response;
        }

        public async Task<object> Get(GetTimeSheetByIdReq req)
        {
            return Ok(await _mediator.Send(new GetTimeSheetByIdQuery(req.Id, req.IncludeReferences)));

        }

        public async Task<object> Get(GetExportTimeSheetDataReq req)
        {
            var request = HostContext.TryGetCurrentRequest() ?? new BasicRequest();
            using (var employeeApiExport = HostContext.ResolveService<EmployeeApi>(request))
            using (var shiftApiExport = HostContext.ResolveService<ShiftApi>(request))
            using (var clockingApiExport = HostContext.ResolveService<ClockingApi>(request))
            {
                var holidaysGetExport = await _mediator.Send(new GetHolidayInDateRangeQuery(req.Filters.StartTime, req.Filters.EndTime));
                var branchSettingsGetExport = await _mediator.Send(new GetBranchSettingByIdQuery(req.Filters.BranchId));

                var employeesResultExport = (await employeeApiExport.Get(new GetListEmployeeReq
                {
                    WithDeleted = true,
                    IdIn = req.EmployeeForCalendarIds
                })) as Response<PagingDataSource<EmployeeDto>>;

                var shiftsResultExport = (await shiftApiExport.Get(new GetListShiftReq
                {
                    IdIn = req.ShiftForCalendarIds
                })) as Response<PagingDataSource<ShiftDto>>;

                var listClockingResult = (await clockingApiExport.Get(req.Filters)) as Response<PagingDataSource<ClockingDto>>;
                if (listClockingResult != null && employeesResultExport != null)
                {
                    var employeeIds = listClockingResult.Result.Data.Select(x => x.EmployeeId).Distinct().ToList();
                    employeesResultExport.Result.Data = employeesResultExport.Result.Data.Where(x => employeeIds.Contains(x.Id)).ToList();
                }

                return Ok(new
                {
                    Holidays = holidaysGetExport ?? new List<HolidayDto>(),
                    BranchWorkingDays = branchSettingsGetExport?.WorkingDays ?? new List<byte>(),
                    Employees = employeesResultExport != null ? employeesResultExport.Result.Data : new List<EmployeeDto>(),
                    Shifts = shiftsResultExport != null ? shiftsResultExport.Result.Data : new List<ShiftDto>(),
                    Clockings = listClockingResult != null ? listClockingResult.Result.Data.Where(c => c.ClockingStatus != (int)ClockingStatuses.Void).ToList() : new List<ClockingDto>()
                });
            }
        }

        public async Task<object> Get(GetExportTimeSheetMultiBranchDataReq req)
        {
            var request = HostContext.TryGetCurrentRequest() ?? new BasicRequest();
            using (var employeeApi = HostContext.ResolveService<EmployeeApi>(request))
            using (var shiftApi = HostContext.ResolveService<ShiftApi>(request))
            using (var clockingApi = HostContext.ResolveService<ClockingApi>(request))
            {
                var holidays = await _mediator.Send(new GetHolidayInDateRangeQuery(req.Filters.StartTime, req.Filters.EndTime));
                var branchSettings = await _mediator.Send(new GetBranchSettingByIdQuery(req.Filters.BranchId));

                var employeesResult = (await employeeApi.Get(new GetListEmployeeMultipleBranchReq()
                {
                    WithDeleted = true,
                    ShiftIds = req.Filters.ShiftIds,
                    BranchIds = req.Filters.BranchIds,
                    EmployeeIds = req.EmployeeForCalendarIds
                })) as Response<PagingDataSource<EmployeeDto>>;

                var shiftsResult = (await shiftApi.Get(new GetShiftMultipleBranchOrderByFromAndToReq()
                {
                    BranchIds = req.Filters.BranchIds,
                    ShiftIds = req.ShiftForCalendarIds
                })) as Response<List<ShiftDto>>;

                var listClockingResultExport = (await clockingApi.Get(req.Filters)) as Response<PagingDataSource<ClockingDto>>;
                if (listClockingResultExport != null && employeesResult != null)
                {
                    var employeeIds = listClockingResultExport.Result.Data.Select(x => x.EmployeeId).Distinct().ToList();
                    employeesResult.Result.Data = employeesResult.Result.Data.Where(x => employeeIds.Contains(x.Id)).ToList();
                }

                return Ok(new
                {
                    Holidays = holidays ?? new List<HolidayDto>(),
                    BranchWorkingDays = branchSettings?.WorkingDays ?? new List<byte>(),
                    Employees = employeesResult != null ? employeesResult.Result.Data : new List<EmployeeDto>(),
                    Shifts = shiftsResult != null ? shiftsResult.Result : new List<ShiftDto>(),
                    Clockings = listClockingResultExport != null ? listClockingResultExport.Result.Data.Where(c => c.ClockingStatus != (int)ClockingStatuses.Void).ToList() : new List<ClockingDto>()
                });
            }
        }

        public object Get(CheckActiveTimesheetReq req)
        {
            return "Timesheet is actived";
        }
        #endregion

        #region POST methods
        public async Task<object> Post(SendEmailActiveTrialTimeSheetReq req)
        {
            await _mediator.Send(new SendEmailActiveTrialCommand(req.SendTo, req.BccEmail, req.Subject, req.Body));

            if (Errors.Any())
            {
                return BadRequest(Message.error_whenDeleteData, Errors);
            }

            return Ok(Message.timeSheet_cancelSuccessfully);
        }
        public async Task<object> Post(CreateTimeSheetReq req)
        {
            var returnObj = await _mediator.Send(new CreateTimesheetCommand(req.TimeSheet));

            if (Errors.Any())
            {
                return BadRequest(Message.error_whenCreateData, Errors);
            }

            return Ok(returnObj);
        }

        public async Task<object> Post(BatchAddTimeSheetWhenCreateMultipleTimeSheetReq req)
        {
            var returnObj = await _mediator.Send(new BatchAddTimeSheetWhenCreateMultipleTimeSheetCommand(req.TimeSheet, req.EmployeeIds, req.IsAuto));

            if (Errors.Any())
            {
                return BadRequest(Message.error_whenCreateData, Errors);
            }

            return Ok(returnObj);
        }

        /// <summary>
        /// Sao chép lịch làm việc trong 1 khoảng thời gian được chọn
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        public async Task<object> Post(CopyListTimeSheetReq req)
        {
            var returnObj = await _mediator.Send(new CopyListTimeSheetCommand(req.BranchId, req.CopyFrom, req.CopyTo, req.PasteFrom, req.PasteTo));

            if (Errors.Any())
            {
                return BadRequest(Message.error_whenCopyData, Errors);
            }

            return Ok(returnObj);
        }

        public async Task<object> Post(PostExportTimeSheetMultiBranchDataReq req)
        {
            var request = HostContext.TryGetCurrentRequest() ?? new BasicRequest();
            using (var employeeApiItem = HostContext.ResolveService<EmployeeApi>(request))
            using (var shiftApiItem = HostContext.ResolveService<ShiftApi>(request))
            using (var clockingApiItem = HostContext.ResolveService<ClockingApi>(request))
            {
                var holidays = await _mediator.Send(new GetHolidayInDateRangeQuery(req.Filters.StartTime, req.Filters.EndTime));
                var branchSettings = await _mediator.Send(new GetBranchSettingByIdQuery(req.Filters.BranchId));

                Response<PagingDataSource<ShiftDto>> shiftsResult = null;
                var listClockingResult = (await clockingApiItem.Get(req.Filters)) as Response<PagingDataSource<ClockingDto>>;
                var listEmployeeIdsFromClocking = new List<long>();
                if (listClockingResult != null)
                {
                    listEmployeeIdsFromClocking = listClockingResult.Result.Data.Select(x => x.EmployeeId).Distinct().ToList();
                }

                var employeesResult = (await employeeApiItem.Get(new GetListEmployeeReq
                {
                    WithDeleted = true,
                    IdIn = req.Filters.EmployeeIds.Any() ? req.Filters.EmployeeIds : listEmployeeIdsFromClocking
                })) as Response<PagingDataSource<EmployeeDto>>;
                
                if (listClockingResult != null && employeesResult != null)
                {
                    var shiftIds = listClockingResult.Result.Data.Select(x => x.ShiftId).Distinct().ToList();
                    shiftsResult = (await shiftApiItem.Get(new GetListShiftReq
                    {
                        IdIn = shiftIds
                    })) as Response<PagingDataSource<ShiftDto>>;
                    employeesResult.Result.Data = employeesResult.Result.Data.Where(x => listEmployeeIdsFromClocking.Contains(x.Id)).ToList();
                }

                var penalizes = await _mediator.Send(new GetAllPenalizeByTenantIdQuery());

                return Ok(new
                {
                    Holidays = holidays ?? new List<HolidayDto>(),
                    BranchWorkingDays = branchSettings?.WorkingDays ?? new List<byte>(),
                    Employees = employeesResult != null ? employeesResult.Result.Data : new List<EmployeeDto>(),
                    Shifts = shiftsResult != null ? shiftsResult.Result.Data : new List<ShiftDto>(),
                    Clockings = listClockingResult != null ? listClockingResult.Result.Data.Where(c => c.ClockingStatus != (int)ClockingStatuses.Void).ToList() : new List<ClockingDto>(),
                    Penalizes = penalizes ?? new List<PenalizeDto>()
                });
            }
        }

        public async Task<object> Post(PostExportTimeSheetDataReq req)
        {
            var request = HostContext.TryGetCurrentRequest() ?? new BasicRequest();
            using (var employeeApi = HostContext.ResolveService<EmployeeApi>(request))
            using (var shiftApi = HostContext.ResolveService<ShiftApi>(request))
            using (var clockingApi = HostContext.ResolveService<ClockingApi>(request))
            {
                var holidays = await _mediator.Send(new GetHolidayInDateRangeQuery(req.Filters.StartTime, req.Filters.EndTime));
                var branchSettings = await _mediator.Send(new GetBranchSettingByIdQuery(req.Filters.BranchId));

                var employeesResult = (await employeeApi.Get(new GetListEmployeeReq
                {
                    WithDeleted = true,
                    IdIn = req.EmployeeForCalendarIds,
                    BranchIds = req.Filters.BranchId > 0 ? new int?[] { req.Filters.BranchId } : new int?[] { }
                })) as Response<PagingDataSource<EmployeeDto>>;
                var shiftsResult = (await shiftApi.Get(new GetListShiftReq
                {
                    IdIn = req.ShiftForCalendarIds,
                    BranchId = req.Filters.BranchId
                })) as Response<PagingDataSource<ShiftDto>>;
                var clockingsResult = (await clockingApi.Get(req.Filters)) as Response<PagingDataSource<ClockingDto>>;
                if (clockingsResult != null && employeesResult != null)
                {
                    var employeeIds = clockingsResult.Result.Data.Select(x => x.EmployeeId).Distinct().ToList();
                    employeesResult.Result.Data = employeesResult.Result.Data.Where(x => employeeIds.Contains(x.Id)).ToList();
                }

                return Ok(new
                {
                    Holidays = holidays ?? new List<HolidayDto>(),
                    BranchWorkingDays = branchSettings?.WorkingDays ?? new List<byte>(),
                    Employees = employeesResult != null ? employeesResult.Result.Data : new List<EmployeeDto>(),
                    Shifts = shiftsResult != null ? shiftsResult.Result.Data : new List<ShiftDto>(),
                    Clockings = clockingsResult != null ? clockingsResult.Result.Data.Where(c => c.ClockingStatus != (int)ClockingStatuses.Void).ToList() : new List<ClockingDto>()
                });
            }
        }
        #endregion

        #region PUT methods
        public async Task<object> Put(UpdateTimeSheetReq req)
        {
            var returnObj = await _mediator.Send(new UpdateTimesheetCommand(req.TimeSheet, req.ForAllClockings));

            if (Errors.Any())
            {
                return BadRequest(Message.error_whenUpdateData, Errors);
            }

            return Ok(returnObj);
        }

        public async Task<object> Put(CancelTimeSheetReq req)
        {
            await _mediator.Send(new CancelTimeSheetCommand(req.Id));

            if (Errors.Any())
            {
                return BadRequest(Message.error_whenDeleteData, Errors);
            }

            return Ok(Message.timeSheet_cancelSuccessfully);
        }

        #endregion
    }
}

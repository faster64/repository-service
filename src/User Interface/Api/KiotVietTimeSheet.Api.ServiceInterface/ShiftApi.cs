using KiotVietTimeSheet.Api.ServiceModel;
using System.Threading.Tasks;
using ServiceStack;
using Microsoft.Extensions.Logging;
using MediatR;
using System.Linq;
using KiotVietTimeSheet.Application.Commands.CreateShift;
using KiotVietTimeSheet.Application.Commands.DeleteShift;
using KiotVietTimeSheet.Application.Commands.UpdateClocking;
using KiotVietTimeSheet.Application.Commands.UpdateShift;
using KiotVietTimeSheet.Application.Queries.GetListShift;
using KiotVietTimeSheet.Application.Queries.GetShiftById;
using KiotVietTimeSheet.Application.Queries.GetShiftByWorkingDayEmployee;
using KiotVietTimeSheet.Application.Queries.GetShiftMultipleBranchOrderByFromAndTo;
using KiotVietTimeSheet.Application.Queries.GetShiftOrderByFromAndTo;
using Message = KiotVietTimeSheet.Resources.Message;
using Label = KiotVietTimeSheet.Resources.Label;
using KiotVietTimeSheet.SharedKernel.Notification;
using KiotVietTimeSheet.Application.Runtime.Exception;
using System.Collections.Generic;
using KiotVietTimeSheet.SharedKernel.Models;

namespace KiotVietTimeSheet.Api.ServiceInterface
{
    public class ShiftApi : BaseApi
    {
        #region Properties
        private readonly IMediator _mediator;
        public IAutoQueryDb AutoQuery { get; set; }
        #endregion

        #region Constructors
        public ShiftApi(
            ILogger<ShiftApi> logger,
            INotificationHandler<DomainNotification> notificationHandler,
            IMediator mediator
        ) : base(logger, notificationHandler)
        {
            _mediator = mediator;
        }
        #endregion

        #region GET methods
        /// <summary>
        /// Lấy danh sách ca làm việc
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        public async Task<object> Get(GetListShiftReq req)
        {
            var query = AutoQuery.CreateQuery(req, Request.GetRequestParams());
            return Ok(await _mediator.Send(new GetListShiftQuery(query)));
        }

        /// <summary>
        /// Lấy thông tin chi tiết ca làm việc theo Id
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        public async Task<object> Get(GetShiftByIdReq req)
        {
            return Ok(await _mediator.Send(new GetShiftByIdQuery(req.Id)));
        }

        /// <summary>
        /// Lấy danh sách ca làm việc theo ngày làm việc của nhân viên
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        public async Task<object> Get(GetShiftByWorkingDayEmployeeReq req)
        {
            var returnObj = await _mediator.Send(new GetShiftByWorkingDayEmployeeQuery(req.EmployeeId, req.StartTime));
            return Ok(returnObj);
        }

        public async Task<object> Get(GetShiftOrderByFromAndToReq req)
        {
            return Ok(await _mediator.Send(new GetShiftOrderByFromAndToQuery(req.BranchId, req.ShiftIds, req.Keyword)));

        }
        public async Task<object> Get(GetShiftMultipleBranchOrderByFromAndToReq req)
        {
            return Ok(await _mediator.Send(new GetShiftMultipleBranchOrderByFromAndToQuery(req.BranchIds, req.ShiftIds, req.IncludeShiftIds, req.IncludeDeleted)));

        }
        #endregion

        #region POST methods
        /// <summary>
        /// Tạo mới ca làm việc
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        public async Task<object> Post(CreateShiftReq req)
        {
            await _mediator.Send(new CreateShiftCommand(req.Shift, req.IsGeneralSetting));

            if (Errors.Any())
            {
                return BadRequest(Message.error_whenCreateData, Errors);
            }

            return Ok(string.Format(Message.create_successed, Label.shift.ToLower()));
        }
        #endregion

        #region PUT methods
        /// <summary>
        /// Cập nhật ca làm việc
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        public async Task<object> Put(UpdateShiftReq req)
        {
            var shift = await _mediator.Send(new UpdateShiftCommand(req.Id, req.Shift, req.IsGeneralSetting));
            if (shift == null)
            {
                var errors = new List<ErrorResult>() { new ErrorResult() { Code = "400", Message = string.Format(Message.not_exists, Label.shift) } };
                return BadRequest(string.Format(Message.not_exists, Label.shift), errors);
            }
            if (shift.OldFrom != shift.From || shift.OldTo != shift.To)
            {
                await _mediator.Send(new UpdateClockingTimeCommand(shift.TenantId, shift.BranchId, shift.Id, shift.Name, shift.From, shift.To));
            }

            if (Errors.Any())
            {
                return BadRequest(Message.error_whenUpdateData, Errors);
            }

            return Ok(string.Format(Message.update_successed, Label.shift.ToLower()));
        }

        public async Task<object> Put(ChangeActiveShiftReq req)
        {
            await _mediator.Send(new UpdateShiftCommand(req.Id, req.Shift, req.IsGeneralSetting));
            if (Errors.Any())
                return BadRequest(Message.error_whenUpdateData, Errors);
            return Ok(string.Format(Message.update_successed, Label.shift.ToLower()));
        }

        #endregion

        #region DELETE methods
        /// <summary>
        /// Xóa ca làm việc
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        public async Task<object> Delete(DeleteShiftReq req)
        {
            var returnObj = await _mediator.Send(new DeleteShiftCommand(req.Id));

            if (Errors.Any())
            {
                return BadRequest(Message.error_whenDeleteData, Errors);
            }

            return Ok(returnObj);
        }
        #endregion
    }
}

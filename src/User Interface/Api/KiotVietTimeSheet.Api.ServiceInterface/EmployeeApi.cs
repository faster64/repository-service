using KiotViet.FileUpload;
using KiotVietTimeSheet.Api.ServiceModel;
using KiotVietTimeSheet.Application.Dto;
using Microsoft.Extensions.Logging;
using KiotVietTimeSheet.Domain.AggregatesModels.EmployeeAggregate.Models;
using KiotVietTimeSheet.SharedKernel.Notification;
using MediatR;
using ServiceStack;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using KiotVietTimeSheet.Api.ServiceInterface.Common;
using KiotVietTimeSheet.Application.Commands.AssignUserIdToEmployee;
using KiotVietTimeSheet.Application.Commands.CreateEmployee;
using KiotVietTimeSheet.Application.Commands.DeleteEmployee;
using KiotVietTimeSheet.Application.Commands.DeleteMultipleEmployee;
using KiotVietTimeSheet.Application.Commands.RemoveEmployeePictureId;
using KiotVietTimeSheet.Application.Commands.UnAssignUserEmployee;
using KiotVietTimeSheet.Application.Commands.UpdateEmployee;
using KiotVietTimeSheet.Application.Queries.CheckEmployeeAssignUserId;
using KiotVietTimeSheet.Application.Queries.CheckEmployeeTotalWithBlock;
using KiotVietTimeSheet.Application.Queries.GetEmployee;
using KiotVietTimeSheet.Application.Queries.GetEmployeeAvailable;
using KiotVietTimeSheet.Application.Queries.GetEmployeeByBranchId;
using KiotVietTimeSheet.Application.Queries.GetEmployeeByCurrentUserId;
using KiotVietTimeSheet.Application.Queries.GetEmployeeById;
using KiotVietTimeSheet.Application.Queries.GetEmployeeByUserId;
using KiotVietTimeSheet.Application.Queries.GetEmployeeForPaysheet;
using KiotVietTimeSheet.Application.Queries.GetEmployeeMultipleBranch;
using Message = KiotVietTimeSheet.Resources.Message;
using Label = KiotVietTimeSheet.Resources.Label;
using KiotVietTimeSheet.Application.Runtime.Exception;
using KiotVietTimeSheet.SharedKernel.Models;
using KiotVietTimeSheet.Application.Queries.GetAndCheckTwoFaPin;
using KiotVietTimeSheet.Infrastructure.DbMaster;
using KiotVietTimeSheet.Utilities;
using Newtonsoft.Json;
using ServiceStack.Configuration;
using KiotVietTimeSheet.Domain.Engines.SalaryEngine.Rules.MainSalary;

namespace KiotVietTimeSheet.Api.ServiceInterface
{
    public class EmployeeApi : BaseApi
    {
        #region Properties
        private readonly IKiotVietFileUpload _kiotVietFileUpload;
        public IAutoQueryDb AutoQuery { get; set; }
        private readonly IMediator _mediator;
        private readonly TimeSheetHelper _timeSheetHelper;
        #endregion

        #region Constructors
        public EmployeeApi(
            IKiotVietFileUpload kiotVietFileUpload,
            IMasterDbService masterDbService,
            ILogger<EmployeeApi> logger,
            INotificationHandler<DomainNotification> notificationHandler,
            IMediator mediator
        ) : base(logger, notificationHandler)
        {
            _kiotVietFileUpload = kiotVietFileUpload;
            _mediator = mediator;
            _timeSheetHelper = new TimeSheetHelper(masterDbService);
        }
        #endregion

        #region GET methods
        public async Task<object> Get(GetListEmployeeReq req)
        {
            try
            {
                var query = AutoQuery.CreateQuery(req, Request.GetRequestParams());
                var result = await _mediator.Send(new GetEmployeeQuery(query, req.WithDeleted, req.IncludeFingerPrint));
                return Ok(result);
            }
            catch (KvTimeSheetUnAuthorizedException ex)
            {
                if (req.FromManagementScreen == true)
                {
                    var rs = new PagingDataSource<EmployeeDto>() { Data = new List<EmployeeDto>(), HasNoPermisstion = true };
                    return Ok(rs);
                }
                throw;
            }
        }

        public async Task<object> Get(GetListEmployeeMultipleBranchReq req)
        {
            var result = await _mediator.Send(new GetEmployeeMultipleBranchQuery(req.BranchIds, req.ShiftIds, req.DepartmentIds, req.IsActive, req.WithDeleted, req.Keyword, req.EmployeeIds));
            return Ok(result);
        }

        public async Task<object> Get(GetEmployeeByIdReq req)
        {
            return Ok(await _mediator.Send(new GetEmployeeByIdQuery(req.Id)));
        }

        public async Task<object> Get(GetEmployeeByBranchId req)
        {
            return Ok(await _mediator.Send(new GetEmployeeByBranchIdQuery(req.BranchId, req.TypeSearch)));
        }

        public async Task<object> Get(GetEmployeeForPaysheetReq req)
        {

            return Ok(await _mediator.Send(new GetEmployeeForPaysheetQuery(req.SalaryPeriod, req.StartTime, req.EndTime, req.BranchId, req.Keyword)));
        }

        public async Task<object> Get(GetEmployeeByUserIdReq req)
        {
            var query = AutoQuery.CreateQuery(req, Request.GetRequestParams());
            return Ok(await _mediator.Send(new GetEmployeeByUserIdQuery(query)));
        }

        public async Task<object> Get(GetEmployeeByCurrentUser req)
        {
            return Ok(await _mediator.Send(new GetEmployeeByCurrentUserIdQuery(true, true)));
        }

        /// <summary>
        /// Lấy danh sách nhân viên làm thay ca
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        public async Task<object> Get(GetAvailableEmployeesReq req)
        {
            var result = await _mediator.Send(new GetEmployeeAvailableQuery(req.BranchId, req.WithoutId, req.StartTime, req.EndTime, req.Skip, req.Take, req.Keyword));
            return Ok(result);
        }

        public async Task<object> Get(CheckAssignUserIdToEmployeeReq req)
        {
            var result = await _mediator.Send(new CheckEmployeeAssignUserIdQuery(req.EmployeeId, req.UserId, req.IsCreateUser));

            if (Errors.Any())
            {
                return Ok(result, Errors.FirstOrDefault()?.Message);
            }

            return Ok(result, string.Format(Message.update_successed, Label.employee.ToLower()));
        }

        public async Task<object> Get(CheckTotalEmployeeWithBlockEmployee req)
        {
            var (blockUnit, _) = await _timeSheetHelper.GetBlockUnitByRetailerId(CurrentRetailerId);
            var contractType = await _timeSheetHelper.GetContractTypeByRetailerId(CurrentRetailerId);
            await _mediator.Send(new CheckEmployeeTotalWithBlockQuery(blockUnit, contractType));
            if (Errors.Any())
            {
                return BadRequest(Message.error_whenUpdateData, Errors);
            }

            return Ok(string.Format(Message.can_addMore, Label.employee.ToLower()));
        }

        public async Task<object> Get(GetTwoFaPinReq req)
        {
            var result = await _mediator.Send(new GetTwoFaPinQuery(req.EmployeeId, req.UserId));

            if (Errors.Any())
            {
                return Ok(result, Errors.FirstOrDefault()?.Message);
            }

            return Ok(new { Pin = result });
        }
        #endregion

        #region POST methods
        public async Task<object> Post(CreateEmployeeReq req)
        {
            var formData = Request.FormData;
            if (formData != null)
            {
                req.Employee = formData["employee"].FromJson<EmployeeDto>() ?? new EmployeeDto();
                req.PayRate = formData["payRate"].FromJson<PayRateDto>();
                PreProcessPayRateDto(req.PayRate);
                req.TypeInsert = formData["typeInsert"].FromJson<int>();
                req.Employee.ProfilePictures = req.Employee.ProfilePictures != null ? req.Employee.ProfilePictures.Where(p => p.Id > 0).ToList() : new List<EmployeeProfilePicture>();
                req.Employee.ProfilePictures.AddRange(await ProcessUploadEmployeeProfilePicturesAsync(req.Employee));
            }
            var (blockUnit, _) = await _timeSheetHelper.GetBlockUnitByRetailerId(CurrentRetailerId);
            var result = await _mediator.Send(new CreateEmployeeCommand(req.Employee, req.PayRate, req.TypeInsert, blockUnit));

            if (Errors.Any())
            {
                return BadRequest(Message.error_whenCreateData, Errors);
            }
            return Ok(string.Format(Message.create_successed, Label.employee.ToLower()), result);
        }

        public async Task<object> Post(DeleteMultipleEmployeeReq req)
        {
            await _mediator.Send(new DeleteMultipleEmployeeCommand(req.EmployeeIds));

            if (Errors.Any())
            {
                return BadRequest(Message.error_whenDeleteData, Errors);
            }

            return Ok(string.Format(Message.delete_successed, Label.employee.ToLower()));
        }

        public async Task<object> Post(UnAssignUserWhenDeleteUserReq req)
        {
            var (blockUnit, _) = await _timeSheetHelper.GetBlockUnitByRetailerId(CurrentRetailerId);

            await _mediator.Send(new UnAssignUserEmployeeCommand(req.UserId, blockUnit));

            if (Errors.Any())
            {
                return BadRequest(Message.error_whenUpdateData, Errors);
            }

            return Ok(string.Format(Message.update_successed, Label.employee.ToLower()));
        }

        public async Task<object> Post(DeleteMultipleEmployeeByBranchIdReq req)
        {
            await _mediator.Send(new DeleteMultipleEmployeeByBranchIdCommand(req.BranchId));

            if (Errors.Any())
            {
                return BadRequest(Message.error_whenDeleteData, Errors);
            }

            return Ok(string.Format(Message.delete_successed, Label.employee.ToLower()));
        }

        public async Task<object> Any(VerifyTwoFaPinReq req)
        {
            var result = await _mediator.Send(new CheckTwoFaPinQuery(req.EmployeeId, req.Pin));

            if (Errors.Any())
            {
                return Ok(result, Errors.FirstOrDefault()?.Message);
            }

            return Ok(new { IsValid = result });
        }
        #endregion

        #region PUT methods
        public async Task<object> Put(UpdateEmployeeReq req)
        {
            var formData = Request.FormData;
            if (formData != null)
            {
                req.Employee = formData["employee"].FromJson<EmployeeDto>();
                req.PayRate = formData["payRate"].FromJson<PayRateDto>();
                PreProcessPayRateDto(req.PayRate);
                req.Employee.ProfilePictures = req.Employee.ProfilePictures != null ? req.Employee.ProfilePictures.Where(p => p.Id > 0).ToList() : new List<EmployeeProfilePicture>();
                req.Employee.ProfilePictures.AddRange(await ProcessUploadEmployeeProfilePicturesAsync(req.Employee));
            }
            var (blockUnit, _) = await _timeSheetHelper.GetBlockUnitByRetailerId(CurrentRetailerId);
            var result = await _mediator.Send(new UpdateEmployeeCommand(req.Employee, req.PayRate, blockUnit));
            if (Errors.Any())
            {
                return BadRequest(Message.error_whenUpdateData, Errors);
            }

            return Ok(result);
        }

        public async Task<object> Put(AssignUserIdToEmployeeReq req)
        {
            var (blockUnit, _) = await _timeSheetHelper.GetBlockUnitByRetailerId(CurrentRetailerId);

            await _mediator.Send(new AssignUserIdToEmployeeCommand(req.Employee, blockUnit));

            if (Errors.Any())
            {
                return BadRequest(Message.error_whenUpdateData, Errors);
            }

            return Ok(string.Format(Message.update_successed, Label.employee.ToLower()));
        }
        #endregion

        #region DELETE methods
        public async Task<object> Delete(DeleteEmployeeReq req)
        {
            await _mediator.Send(new DeleteEmployeeCommand(req.Id));

            if (Errors.Any())
            {
                return BadRequest(Message.error_whenDeleteData, Errors);
            }

            return Ok(string.Format(Message.delete_successed, Label.employee.ToLower()));
        }

        public async Task<object> Delete(DeleteEmployeeProfilePictureReq req)
        {
            await _mediator.Send(new RemoveEmployeePictureIdCommand(req.Id));


            if (Errors.Any())
            {
                return BadRequest(Message.error_whenUpdateData, Errors);
            }

            return Ok(string.Format(Message.delete_successed, Label.employee_picture.ToLower()));
        }
        #endregion

        #region PRIVATE methodsP
        private void PreProcessPayRateDto(PayRateDto payRateDto)
        {
            if (payRateDto != null && payRateDto.MainSalaryRuleValue != null && payRateDto.MainSalaryRuleValue.MainSalaryValueDetails != null)
            {
                foreach (var item in payRateDto.MainSalaryRuleValue.MainSalaryValueDetails)
                {
                    if (item.MainSalaryHolidays == null)
                        item.MainSalaryHolidays = new List<MainSalaryHolidays>();
                }
            }
        }
        private async Task<List<EmployeeProfilePicture>> ProcessUploadEmployeeProfilePicturesAsync(EmployeeDto employee)
        {
            var results = new List<EmployeeProfilePicture>();

            if (Request.Files != null)
            {
                foreach (var uploadedFile in Request.Files.Where(uploadedFile => uploadedFile.ContentLength > 0))
                {
                    using (var ms = new MemoryStream())
                    {
                        uploadedFile.WriteTo(ms);
                        var key = $"{CurrentRetailerId}/{Guid.NewGuid().ToString("N").ToLower()}";
                        var uploadResult = await _kiotVietFileUpload.UploadAvatarAsync(key, ms, uploadedFile.ContentType);
                        if (uploadResult.Status == UploadResultStatuses.Success)
                        {
                            results.Add(new EmployeeProfilePicture(employee.Id, uploadResult.Url, employee.TenantId));
                        }
                    }
                }
            }

            return results;
        }
        #endregion
    }
}

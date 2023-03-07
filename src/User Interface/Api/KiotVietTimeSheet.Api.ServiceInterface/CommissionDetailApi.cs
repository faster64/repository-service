using System.Collections.Generic;
using System.Threading.Tasks;
using KiotVietTimeSheet.Api.ServiceModel;
using KiotVietTimeSheet.Application.Commands.CreateCommissionDetail;
using KiotVietTimeSheet.Application.Commands.CreateCommissionDetailByProduct;
using KiotVietTimeSheet.Application.Commands.CreateCommissionDetailByProductCategory;
using KiotVietTimeSheet.Application.Commands.CreateCommissionDetailByProductCategoryAsync;
using KiotVietTimeSheet.Application.Commands.CreateMultipleCommissionDetail;
using KiotVietTimeSheet.Application.Commands.DeleteCommissionDetail;
using KiotVietTimeSheet.Application.Commands.InsertCommissionDetails;
using KiotVietTimeSheet.Application.Commands.UpdateCommissionDetail;
using KiotVietTimeSheet.Application.Commands.UpdateValueOfCommissionDetail;
using Microsoft.Extensions.Logging;
using KiotVietTimeSheet.Application.Queries.GetAllCommissionDetailsByCommissionIds;
using KiotVietTimeSheet.Application.Queries.GetCommissionDetailsByCommissionIds;
using KiotVietTimeSheet.Application.Queries.GetCommissionDetailsByProductId;
using KiotVietTimeSheet.Resources;
using KiotVietTimeSheet.SharedKernel.Notification;
using MediatR;
using Microsoft.EntityFrameworkCore.Internal;
using ServiceStack;
using KiotVietTimeSheet.Application.Commands.CreateCommissionDetailCategory;

namespace KiotVietTimeSheet.Api.ServiceInterface
{
    public class CommissionDetailApi : BaseApi
    {
        private readonly IMediator _mediator;
        public IAutoQueryDb AutoQuery { get; set; }

        public CommissionDetailApi(
            ILogger<CommissionDetailApi> logger,
            INotificationHandler<DomainNotification> notificationHandler,
            IMediator mediator
            ) : base(logger, notificationHandler)
        {
            _mediator = mediator;
        }

        public async Task<object> Get(GetCommissionDetailsProductIdReq req)
        {
            var query = AutoQuery.CreateQuery(req, Request.GetRequestParams());
            var result = await _mediator.Send(new GetCommissionDetailsByProductIdQuery(query));
            return result;
        }

        public async Task<object> Get(GetAllCommissionDetailsByCommissionIdsReq req)
        {
            var result = await _mediator.Send(new GetAllCommissionDetailsByCommissionIdsQuery(req.CommissionIds));
            return result;
        }
        public async Task<object> Get(GetInsertCommissionDetailStatus req)
        {
            var result = await _mediator.Send(new InsertCommissionDetailsStatusCommand());
            return result;
        }

        public async Task<object> Post(GetCommissionDetailsByCommissionIdsReq req)
        {
            var result = await _mediator.Send(new GetCommissionDetailsByCommissionIdsQuery(req.CommissionIds, req.ProductIds));
            return result;
        }
        public async Task<object> Post(CreateCommissionDetailReq req)
        {
            var returnObj = await _mediator.Send(new CreateCommissionDetailCommand(req.CommissionDetails));

            if (Errors.Any())
            {
                return BadRequest(Message.error_whenCreateData, Errors);
            }

            return Ok(returnObj);
        }


        public async Task<object> Post(CreateCommissionDetailByProductCategoryReq req)
        {
            var productNumber =
                    await _mediator.Send(new CreateCommissionDetailByProductCategoryCommand(req.CommissionIds, req.ProductCategory));
            if (Errors.Any())
            {
                return BadRequest(Message.error_whenCreateData, Errors);
            }

            return Ok(new Dictionary<string, object>
                {
                    {"ProductNumber", productNumber }
                });
        }

        public async Task<object> Post(CreateCommissionDetailByProductCategoryAsyncReq req)
        {
            await _mediator.Send(
                    new CreateCommissionDetailByProductCategoryAsyncCommand(req.CommissionIds, req.ProductCategory));
            if (Errors.Any())
            {
                return BadRequest(Message.error_whenCreateData, Errors);
            }

            return Ok(Message.commission_InProgress);
        }

        public async Task<object> Post(CreateCommissionDetailByProductReq req)
        {
            var returnObj =
                    await _mediator.Send(new CreateCommissionDetailByProductCommand(req.CommissionIds, req.Product));
            if (Errors.Any())
            {
                return BadRequest(Message.error_whenCreateData, Errors);
            }

            return Ok(returnObj);
        }

        public async Task<object> Post(CreateMultipleCommissionDetailsReq req)
        {
            var returnObj = await _mediator.Send(new CreateMultipleCommissionDetailCommand(req.Product,
                    req.TotalCommissionIds, req.Value, req.ValueRatio, req.IsUpdateForAllCommission, req.CategoryId,
                    req.ProductCodeKeyword, req.ProductNameKeyword, req.Category));
            if (Errors.Any())
            {
                return BadRequest(Message.error_whenCreateData, Errors);
            }

            return Ok(returnObj);
        }

        public async Task<object> Post(CreateCommissionDetailCategoryIdsReq req)
        {
            var returnObj = await _mediator.Send(new CreateCommissionDetailCategoryCommand(req.CommissionIds, req.CategoryIds));
            if (Errors.Any())
            {
                return BadRequest(Message.error_whenCreateData, Errors);
            }

            return Ok(returnObj);
        }

        public async Task<object> Put(DeleteCommissionDetailReq req)
        {
            await _mediator.Send(new DeleteCommissionDetailCommand(req.CommissionIds, req.Products, req.CategoryIds));
            if (Errors.Any())
            {
                return BadRequest(Message.error_whenDeleteData, Errors);
            }

            return Ok(string.Format(Message.delete_successed, Label.commission_detail.ToLower()));
        }

        public async Task<object> Put(UpdateValueOfCommisionDetailReq req)
        {
            await _mediator.Send(new UpdateValueOfCommissionDetailCommand(req.Product, req.TotalCommissionIds,
                    req.Value, req.ValueRatio, req.IsUpdateForAllCommission, req.CategoryId, req.ProductCodeKeyword,
                    req.ProductNameKeyword, req.Category));
            if (Errors.Any())
            {
                return BadRequest(Message.error_whenDeleteData, Errors);
            }

            return Ok(string.Format(Message.update_successed, Label.commission.ToLower()));
        }

        public async Task<object> Put(UpdateCommissionDetailReq req)
        {
            var returnObj = await _mediator.Send(new UpdateCommissionDetailCommand(req.Commissiondetails));

            if (Errors.Any())
            {
                return BadRequest(Message.error_whenCreateData, Errors);
            }

            return Ok(returnObj);
        }
    }
}

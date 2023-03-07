﻿using AutoMapper;
using KiotVietTimeSheet.Application.Abstractions;
using KiotVietTimeSheet.Application.Auth;
using KiotVietTimeSheet.Application.Commands.CreateCommissionDetailByProductCategoryDataTrial;
using KiotVietTimeSheet.Application.Dto;
using KiotVietTimeSheet.Application.EventBus.Events.CommissionDetailEvents;
using KiotVietTimeSheet.Application.Repositories.WriteRepositories;
using KiotVietTimeSheet.Application.ServiceClients;
using KiotVietTimeSheet.Application.ServiceClients.RequestModels;
using KiotVietTimeSheet.Domain.AggregatesModels.CommissionAggregate.Specifications;
using KiotVietTimeSheet.Domain.AggregatesModels.CommissionDetailAggregate.Enums;
using KiotVietTimeSheet.Domain.AggregatesModels.CommissionDetailAggregate.Models;
using KiotVietTimeSheet.Domain.AggregatesModels.CommissionDetailAggregate.Specifications;
using KiotVietTimeSheet.Resources;
using KiotVietTimeSheet.SharedKernel.Domain;
using KiotVietTimeSheet.SharedKernel.Notification;
using MediatR;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace KiotVietTimeSheet.Application.Commands.CreateCommissionDetailByProductCategory
{
    public class CreateCommissionDetailByProductCategoryDataTrialCommandHandler : BaseCommandHandler,
        IRequestHandler<CreateCommissionDetailByProductCategoryDataTrialCommand, List<CommissionDetailDto>>
    {
        private readonly IMapper _mapper;
        private readonly IEventDispatcher _eventDispatcher;
        private readonly ICommissionDetailWriteOnlyRepository _commissionDetailWriteOnlyRepository;
        private readonly IKiotVietServiceClient _kiotVietServiceClient;
        private readonly ICommissionWriteOnlyRepository _commissionWriteOnlyRepository;

        public CreateCommissionDetailByProductCategoryDataTrialCommandHandler(
            IEventDispatcher eventDispatcher,
            IMapper mapper,
            ICommissionWriteOnlyRepository commissionWriteOnlyRepository,
            ICommissionDetailWriteOnlyRepository commissionDetailWriteOnlyRepository,
            IKiotVietServiceClient kiotVietServiceClient
        )
            : base(eventDispatcher)
        {
            _eventDispatcher = eventDispatcher;
            _mapper = mapper;
            _commissionDetailWriteOnlyRepository = commissionDetailWriteOnlyRepository;
            _commissionWriteOnlyRepository = commissionWriteOnlyRepository;
            _kiotVietServiceClient = kiotVietServiceClient;
        }

        public async Task<List<CommissionDetailDto>> Handle(CreateCommissionDetailByProductCategoryDataTrialCommand request, CancellationToken cancellationToken)
        {
            var commissionIdList = request.CommissionIds;
            var productCategory = request.ProductCategory;
            if (!commissionIdList.Any())
            {
                NotifyCommissionHaveNotSelected();
            }

            var newCommissionDetails = new List<CommissionDetail>();
            var listProductWithCategory = await _kiotVietServiceClient.GetProductByCategoryIdDataTrial(
                new GetProductByCategoryIdReq
                {
                    CategoryId = productCategory.Id,
                    RetailerId = request.TenantId,
                    GroupId = request.GroupId
                });

            var commissionDetailSpecification = new FindCommissionDetailByCommissionIdsSpec(commissionIdList).And(new FindCommissionDetailByTypeSpec((byte)CommissionDetailType.Product));
            var commissionDetails =
                await _commissionDetailWriteOnlyRepository.GetBySpecificationAsync(commissionDetailSpecification);

            foreach (var commissionId in commissionIdList)
            {
                foreach (var product in listProductWithCategory)
                {

                    var isExistedCommissionDetail = commissionDetails.Exists(x => x.CommissionId == commissionId
                                                                                && x.ObjectId == product.Id);
                    if (!isExistedCommissionDetail)
                    {
                        var newCommissionDetail = new CommissionDetail(commissionId, product.Id, null, 10);
                            newCommissionDetails.Add(newCommissionDetail);
                    }
                }
            }

            if (!newCommissionDetails.Any() && listProductWithCategory.Any())
            {
                var commissions =
                    await _commissionWriteOnlyRepository.GetBySpecificationAsync(
                        new FindCommissionByIdsSpec(commissionIdList));

                NotifyProductGroupExists(commissions.Select(x => x.Name).ToList());
            }
            var result = _mapper.Map<List<CommissionDetailDto>>(newCommissionDetails);
            if (!newCommissionDetails.Any() || newCommissionDetails.Count <= 0)
                return _mapper.Map<List<CommissionDetailDto>>(result);
            _commissionDetailWriteOnlyRepository.BatchAdd(newCommissionDetails);
            await _kiotVietServiceClient.CreateCommissionDetailSync(new GetProductByCategoryIdReq
            {
                CommissionDetails = _mapper.Map<List<CommissionDetailDto>>(newCommissionDetails),
                RetailerId = request.TenantId,
                GroupId = request.GroupId,
                UserId = request.UserIdAdmin
            });

            await _commissionDetailWriteOnlyRepository.UnitOfWork.CommitAsync(false);

            return _mapper.Map<List<CommissionDetailDto>>(result);
        }
        private void NotifyCommissionHaveNotSelected()
        {
            _eventDispatcher.FireEvent(new DomainNotification(nameof(CommissionDetail), Message.commission_haveNotSelected));
        }
        private void NotifyProductGroupExists(IEnumerable<string> listCommission)
        {
            _eventDispatcher.FireEvent(new DomainNotification(nameof(CommissionDetail), string.Format(Message.commission_ProductGroupExist, string.Join(", ", listCommission))));
        }
    }
}


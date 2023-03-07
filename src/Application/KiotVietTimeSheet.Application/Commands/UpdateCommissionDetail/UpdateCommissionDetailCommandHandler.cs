using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using KiotVietTimeSheet.Application.Abstractions;
using KiotVietTimeSheet.Application.Dto;
using KiotVietTimeSheet.Application.Repositories.WriteRepositories;
using KiotVietTimeSheet.Domain.AggregatesModels.CommissionDetailAggregate.Models;
using KiotVietTimeSheet.Domain.AggregatesModels.CommissionDetailAggregate.Specifications;
using KiotVietTimeSheet.SharedKernel.Domain;
using KiotVietTimeSheet.SharedKernel.Notification;
using MediatR;

namespace KiotVietTimeSheet.Application.Commands.UpdateCommissionDetail
{
    public class UpdateCommissionDetailCommandHandler : BaseCommandHandler,
        IRequestHandler<UpdateCommissionDetailCommand, List<CommissionDetailDto>>
    {
        private readonly IMapper _mapper;
        private readonly IEventDispatcher _eventDispatcher;
        private readonly ICommissionDetailWriteOnlyRepository _commissionDetailWriteOnlyRepository;

        public UpdateCommissionDetailCommandHandler(
            IEventDispatcher eventDispatcher,
            IMapper mapper,
            ICommissionDetailWriteOnlyRepository commissionDetailWriteOnlyRepository
        )
            : base(eventDispatcher)
        {
            _eventDispatcher = eventDispatcher;
            _mapper = mapper;
            _commissionDetailWriteOnlyRepository = commissionDetailWriteOnlyRepository;
        }

        public async Task<List<CommissionDetailDto>> Handle(UpdateCommissionDetailCommand request, CancellationToken cancellationToken)
        {
            var commissionDetailDtoList = request.CommissionDetailDtoList;
            var oldCommissionDetails =
                await _commissionDetailWriteOnlyRepository.GetBySpecificationAsync(
                    new FindCommissionDetailByIdsSpec(commissionDetailDtoList.Select(c => c.Id).ToList()));

            if (!oldCommissionDetails.Any())
            {
                await _eventDispatcher.FireEvent(new DomainNotification(nameof(CommissionDetail), "Không có bản ghi nào"));
                return null;
            }

            foreach (var commissionDetail in oldCommissionDetails)
            {
                var changedCommissionDetail = commissionDetailDtoList.FirstOrDefault(c => c.Id == commissionDetail.Id);
                if(changedCommissionDetail == null) continue;

                // trường hợp update cả giá trị VND và % về null sẽ xóa hàng hóa khỏi bảng hoa hồng
                if (!changedCommissionDetail.Value.HasValue && !changedCommissionDetail.ValueRatio.HasValue) commissionDetail.Delete();

                if (changedCommissionDetail.Value.HasValue)
                {
                    commissionDetail.Value = changedCommissionDetail.Value;
                    commissionDetail.ValueRatio = null;
                }

                if (changedCommissionDetail.ValueRatio.HasValue)
                {
                    commissionDetail.ValueRatio = changedCommissionDetail.ValueRatio;
                    commissionDetail.Value = null;
                }
            }

            _commissionDetailWriteOnlyRepository.BatchUpdate(oldCommissionDetails);
            await _commissionDetailWriteOnlyRepository.UnitOfWork.CommitAsync();
            var result = _mapper.Map<List<CommissionDetailDto>>(oldCommissionDetails);
            return result;
        }
    }
}


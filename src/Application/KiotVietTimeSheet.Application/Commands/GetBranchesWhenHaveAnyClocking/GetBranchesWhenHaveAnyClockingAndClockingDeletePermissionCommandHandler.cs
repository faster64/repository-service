using KiotVietTimeSheet.Application.Abstractions;
using KiotVietTimeSheet.Application.Auth;
using KiotVietTimeSheet.Application.Repositories.ReadRepositories;
using KiotVietTimeSheet.Application.ServiceClients;
using KiotVietTimeSheet.Domain.AggregatesModels.ClockingAggregate.Specifications;
using KiotVietTimeSheet.SharedKernel.Domain;
using MediatR;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using KiotVietTimeSheet.Domain.AggregatesModels.ClockingAggregate.Enums;
using KiotVietTimeSheet.SharedKernel.Models;
using KiotVietTimeSheet.Application.ServiceClients.Dtos;

namespace KiotVietTimeSheet.Application.Commands.GetBranchesWhenHaveAnyClocking
{
    public class GetBranchesWhenHaveAnyClockingAndClockingDeletePermissionCommandHandler : BaseCommandHandler,
        IRequestHandler<GetBranchesWhenHaveAnyClockingAndClockingDeletePermissionCommand, PagingDataSource<BranchDto>>
    {
        private readonly IClockingReadOnlyRepository _clockingReadOnlyRepository;
        private readonly IKiotVietServiceClient _kiotVietServiceClient;

        public GetBranchesWhenHaveAnyClockingAndClockingDeletePermissionCommandHandler(
            IEventDispatcher eventDispatcher,
            IClockingReadOnlyRepository clockingReadOnlyRepository,
            IKiotVietServiceClient kiotVietServiceClient
        )
            : base(eventDispatcher)
        {
            _clockingReadOnlyRepository = clockingReadOnlyRepository;
            _kiotVietServiceClient = kiotVietServiceClient;
        }

        public async Task<PagingDataSource<BranchDto>> Handle(GetBranchesWhenHaveAnyClockingAndClockingDeletePermissionCommand request, CancellationToken cancellationToken)
        {
            var branchCancelIds = request.BranchCancelIds;
            var employeeId = request.EmployeeId;

            if (branchCancelIds == null || branchCancelIds.Count == 0) return new PagingDataSource<BranchDto>();
            var branchHasPermissionDelete =
                (await _kiotVietServiceClient.GetBranchByPermission(TimeSheetPermission.Clocking_Delete)).Data;
            var branchDeleteIds = branchCancelIds
                .Where(cb => branchHasPermissionDelete.Any(bDelete => bDelete.Id == cb)).ToList();
            if (!branchDeleteIds.Any())
                return new PagingDataSource<BranchDto>();

            //Kiểm tra nhân viên đã có chi tiết ca làm việc khác hủy trên chi nhánh cần hủy
            var clockingSpecification = new FindClockingByEmployeeIdSpec(employeeId)
                .And(new FindClockingByBranchIdsSpec(branchDeleteIds))
                .And(new FindClockingByStatusSpec((byte)ClockingStatuses.Created));

            var checkExistClocking = await _clockingReadOnlyRepository.GetBySpecificationAsync(clockingSpecification);
            var deleteBranches = branchDeleteIds.Select(x => new BranchDto() { Id = x }).ToList();
            return checkExistClocking != null && checkExistClocking.Count > 0 ? new PagingDataSource<BranchDto>() { Data = deleteBranches } : new PagingDataSource<BranchDto>();
        }

    }
}


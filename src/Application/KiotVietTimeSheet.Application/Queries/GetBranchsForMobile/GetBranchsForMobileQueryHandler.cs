using AutoMapper;
using KiotVietTimeSheet.Application.Abstractions;
using KiotVietTimeSheet.Application.Auth;
using KiotVietTimeSheet.Application.Repositories.ReadRepositories;
using KiotVietTimeSheet.Application.ServiceClients;
using KiotVietTimeSheet.Application.ServiceClients.Dtos;
using MediatR;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace KiotVietTimeSheet.Application.Queries.GetBranchsForMobile
{
    public class GetBranchsForMobileQueryHandler : QueryHandlerBase,
        IRequestHandler<GetBranchsForMobileQuery, List<BranchMobileDto>>
    {
        private readonly IMapper _mapper;
        private readonly IKiotVietServiceClient _kiotVietServiceClient;

        public GetBranchsForMobileQueryHandler(
            IBranchSettingReadOnlyRepository branchSettingReadOnlyRepository,
            IMapper mapper,
            IAuthService authService,
            IKiotVietServiceClient kiotVietServiceClient
        ) : base(authService)
        {
            _mapper = mapper;
            _kiotVietServiceClient = kiotVietServiceClient;
        }

        public async Task<List<BranchMobileDto>> Handle(GetBranchsForMobileQuery request, CancellationToken cancellationToken)
        {
            var branchHasPermissionRead =
                (await _kiotVietServiceClient.GetBranchByPermissionForMobile(TimeSheetPermission.Clocking_Read)).Data;
            return _mapper.Map<List<BranchMobileDto>>(branchHasPermissionRead?.Where(x => x.IsActive && !x.LimitAccess));
        }
    }
}
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using KiotVietTimeSheet.Application.Abstractions;
using KiotVietTimeSheet.Application.Auth;
using KiotVietTimeSheet.Application.Dto;
using KiotVietTimeSheet.Application.Repositories.ReadRepositories;
using KiotVietTimeSheet.Domain.AggregatesModels.BranchSettingAggregate.Specifications;
using MediatR;

namespace KiotVietTimeSheet.Application.Queries.GetBranchSettingByIds
{
    public class GetBranchSettingByIdsQueryHandler : QueryHandlerBase,
        IRequestHandler<GetBranchSettingByIdsQuery, List<BranchSettingDto>>
    {
        private readonly IBranchSettingReadOnlyRepository _branchSettingReadOnlyRepository;
        private readonly IMapper _mapper;

        public GetBranchSettingByIdsQueryHandler(
            IBranchSettingReadOnlyRepository branchSettingReadOnlyRepository,
            IMapper mapper,
            IAuthService authService
        ) : base(authService)
        {
            _branchSettingReadOnlyRepository = branchSettingReadOnlyRepository;
            _mapper = mapper;
        }

        public async Task<List<BranchSettingDto>> Handle(GetBranchSettingByIdsQuery request, CancellationToken cancellationToken)
        {
            var branchSettings = await _branchSettingReadOnlyRepository.GetBySpecificationAsync(new FindBranchSettingByBranchIdsSpec(request.Ids));
            return _mapper.Map<List<BranchSettingDto>>(branchSettings);
        }
    }
}

using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using KiotVietTimeSheet.Application.Abstractions;
using KiotVietTimeSheet.Application.Auth;
using KiotVietTimeSheet.Application.Dto;
using KiotVietTimeSheet.Application.Repositories.ReadRepositories;
using KiotVietTimeSheet.Domain.AggregatesModels.BranchSettingAggregate.Specifications;
using MediatR;

namespace KiotVietTimeSheet.Application.Queries.GetBranchSetting
{
    public class GetBranchSettingByIdQueryHandler : QueryHandlerBase,
        IRequestHandler<GetBranchSettingByIdQuery, BranchSettingDto>
    {
        private readonly IBranchSettingReadOnlyRepository _branchSettingReadOnlyRepository;
        private readonly IMapper _mapper;

        public GetBranchSettingByIdQueryHandler(
            IBranchSettingReadOnlyRepository branchSettingReadOnlyRepository,
            IMapper mapper,
            IAuthService authService
        ) : base(authService)
        {
            _branchSettingReadOnlyRepository = branchSettingReadOnlyRepository;
            _mapper = mapper;
        }

        public async Task<BranchSettingDto> Handle(GetBranchSettingByIdQuery request, CancellationToken cancellationToken)
        {
            var branchSetting = await _branchSettingReadOnlyRepository.FindBranchSettingWithDefault(new FindBranchSettingByBranchIdSpec(request.Id));
            return _mapper.Map<BranchSettingDto>(branchSetting);
        }
    }
}

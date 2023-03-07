using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using KiotVietTimeSheet.Application.Abstractions;
using KiotVietTimeSheet.Application.Auth;
using KiotVietTimeSheet.Application.Dto;
using KiotVietTimeSheet.Application.Repositories.ReadRepositories;
using MediatR;

namespace KiotVietTimeSheet.Application.Queries.GetConfirmClockingsByBranchId
{
    public class GetConfirmClockingsByBranchIdQueryHandler : QueryHandlerBase,
        IRequestHandler<GetConfirmClockingsByBranchIdQuery, List<ConfirmClockingDto>>
    {
        private readonly IConfirmClockingReadOnlyRepository _confirmClockingReadOnlyRepository;
        private readonly IMapper _mapper;

        public GetConfirmClockingsByBranchIdQueryHandler(
            IConfirmClockingReadOnlyRepository confirmClockingReadOnlyRepository,
            IAuthService authService,
            IMapper mapper
        ) : base(authService)
        {
            _confirmClockingReadOnlyRepository = confirmClockingReadOnlyRepository;
            _mapper = mapper;
        }

        public async Task<List<ConfirmClockingDto>> Handle(GetConfirmClockingsByBranchIdQuery request, CancellationToken cancellationToken)
        {
            var confirmClockings = await _confirmClockingReadOnlyRepository.GetConfirmClockingsByBranchId(request.BranchId);
            var result = _mapper.Map<List<ConfirmClockingDto>>(confirmClockings);
            return result;
        }
    }
}

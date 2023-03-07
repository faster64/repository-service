using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using KiotVietTimeSheet.Application.Abstractions;
using KiotVietTimeSheet.Application.Auth;
using KiotVietTimeSheet.Application.Dto;
using KiotVietTimeSheet.Application.Repositories.ReadRepositories;
using MediatR;

namespace KiotVietTimeSheet.Application.Queries.GetTimeSheetById
{
    public class GetTimeSheetByIdQueryHandler : QueryHandlerBase,
        IRequestHandler<GetTimeSheetByIdQuery, TimeSheetDto>
    {
        private readonly ITimeSheetReadOnlyRepository _timeSheetReadOnlyRepository;
        private readonly IMapper _mapper;

        public GetTimeSheetByIdQueryHandler(
            IAuthService authService,
            ITimeSheetReadOnlyRepository timeSheetReadOnlyRepository,
            IMapper mapper

        ) : base(authService)
        {
            _timeSheetReadOnlyRepository = timeSheetReadOnlyRepository;
            _mapper = mapper;
        }

        public async Task<TimeSheetDto> Handle(GetTimeSheetByIdQuery request, CancellationToken cancellationToken)
        {
            var result = await _timeSheetReadOnlyRepository.FindByIdAsync(request.Id, request.IncludeReferences);
            return _mapper.Map<TimeSheetDto>(result);
        }
    }
}

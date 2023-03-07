using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using KiotVietTimeSheet.Application.Abstractions;
using KiotVietTimeSheet.Application.Auth;
using KiotVietTimeSheet.Application.Dto;
using KiotVietTimeSheet.Application.Repositories.ReadRepositories;
using MediatR;

namespace KiotVietTimeSheet.Application.Queries.GetJobTitleById
{
    public class GetJobTitleByIdQueryHandler : QueryHandlerBase,
        IRequestHandler<GetJobTitleByIdQuery, JobTitleDto>
    {
        private readonly IJobTitleReadOnlyRepository _jobTitleReadOnlyRepository;
        private readonly IMapper _mapper;
        public GetJobTitleByIdQueryHandler(
            IAuthService authService,
            IJobTitleReadOnlyRepository jobTitleReadOnlyRepository,
            IMapper mapper
            ) : base(authService)
        {
            _jobTitleReadOnlyRepository = jobTitleReadOnlyRepository;
            _mapper = mapper;
        }

        public async Task<JobTitleDto> Handle(GetJobTitleByIdQuery request, CancellationToken cancellationToken)
        {
            var result = await _jobTitleReadOnlyRepository.FindByIdAsync(request.Id);
            return _mapper.Map<JobTitleDto>(result);
        }
    }
}

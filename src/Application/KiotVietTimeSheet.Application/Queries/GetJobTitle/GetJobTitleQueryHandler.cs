using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using KiotVietTimeSheet.Application.Abstractions;
using KiotVietTimeSheet.Application.Auth;
using KiotVietTimeSheet.Application.Dto;
using KiotVietTimeSheet.Application.Repositories.ReadRepositories;
using KiotVietTimeSheet.SharedKernel.Models;
using MediatR;

namespace KiotVietTimeSheet.Application.Queries.GetJobTitle
{
    public class GetJobTitleQueryHandler : QueryHandlerBase,
        IRequestHandler<GetJobTitleQuery, PagingDataSource<JobTitleDto>>
    {
        private readonly IJobTitleReadOnlyRepository _jobTitleReadOnlyRepository;
        public GetJobTitleQueryHandler(
            IAuthService authService,
            IJobTitleReadOnlyRepository jobTitleReadOnlyRepository
            ) : base(authService)
        {
            _jobTitleReadOnlyRepository = jobTitleReadOnlyRepository;
        }

        public async Task<PagingDataSource<JobTitleDto>> Handle(GetJobTitleQuery request, CancellationToken cancellationToken)
        {
            var result = await _jobTitleReadOnlyRepository.FiltersAsync(request.Query);
            result.Data = result.Data?.OrderBy(x => x.Name).ToList();
            return result;
        }
    }
}

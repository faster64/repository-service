using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using KiotVietTimeSheet.Application.Abstractions;
using KiotVietTimeSheet.Application.Auth;
using KiotVietTimeSheet.Application.Dto;
using KiotVietTimeSheet.Application.Repositories.ReadRepositories;
using KiotVietTimeSheet.SharedKernel.Models;
using MediatR;

namespace KiotVietTimeSheet.Application.Queries.GetDepartment
{
    public class GetDepartmentQueryHandler : QueryHandlerBase,
        IRequestHandler<GetDepartmentQuery, PagingDataSource<DepartmentDto>>
    {
        private readonly IDepartmentReadOnlyRepository _departmentReadOnlyRepository;

        public GetDepartmentQueryHandler(
            IDepartmentReadOnlyRepository departmentReadOnlyRepository,
            IAuthService authService
        ) : base(authService)
        {
            _departmentReadOnlyRepository = departmentReadOnlyRepository;
        }

        public async Task<PagingDataSource<DepartmentDto>> Handle(GetDepartmentQuery request, CancellationToken cancellationToken)
        {
            var result = await _departmentReadOnlyRepository.FiltersAsync(request.Query);
            result.Data = result.Data?.OrderBy(x => x.Name).ToList();
            return result;
        }
    }
}

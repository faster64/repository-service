using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using KiotVietTimeSheet.Application.Abstractions;
using KiotVietTimeSheet.Application.Auth;
using KiotVietTimeSheet.Application.Dto;
using KiotVietTimeSheet.Application.Repositories.ReadRepositories;
using MediatR;

namespace KiotVietTimeSheet.Application.Queries.GetDepartmentById
{
    public class GetDepartmentByIdQueryHandler : QueryHandlerBase,
        IRequestHandler<GetDepartmentByIdQuery, DepartmentDto>
    {
        private readonly IDepartmentReadOnlyRepository _departmentReadOnlyRepository;
        private readonly IMapper _mapper;

        public GetDepartmentByIdQueryHandler(
            IDepartmentReadOnlyRepository departmentReadOnlyRepository,
            IMapper mapper,
            IAuthService authService
        ) : base(authService)
        {
            _departmentReadOnlyRepository = departmentReadOnlyRepository;
            _mapper = mapper;
        }

        public async Task<DepartmentDto> Handle(GetDepartmentByIdQuery request, CancellationToken cancellationToken)
        {
            var result = await _departmentReadOnlyRepository.FindByIdAsync(request.Id);
            return _mapper.Map<DepartmentDto>(result);
        }
    }
}

using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using KiotVietTimeSheet.Application.Abstractions;
using KiotVietTimeSheet.Application.Auth;
using KiotVietTimeSheet.Application.Dto;
using KiotVietTimeSheet.Application.Repositories.ReadRepositories;
using KiotVietTimeSheet.SharedKernel.Models;
using MediatR;

namespace KiotVietTimeSheet.Application.Queries.GetEmployeeMultipleBranch
{
    public class GetEmployeeMultipleBranchQueryHandler : QueryHandlerBase,
        IRequestHandler<GetEmployeeMultipleBranchQuery, PagingDataSource<EmployeeDto>>
    {
        private readonly IMapper _mapper;
        private readonly IEmployeeReadOnlyRepository _employeeReadOnlyRepository;

        public GetEmployeeMultipleBranchQueryHandler(
            IAuthService authService,
            IMapper mapper,
            IEmployeeReadOnlyRepository employeeReadOnlyRepository
            ) : base(authService)
        {
            _employeeReadOnlyRepository = employeeReadOnlyRepository;
            _mapper = mapper;
        }

        public async Task<PagingDataSource<EmployeeDto>> Handle(GetEmployeeMultipleBranchQuery request, CancellationToken cancellationToken)
        {
            var employees = await _employeeReadOnlyRepository.GetEmployeesMultipleBranch(request.BranchIds, request.ShiftIds, request.DepartmentIds, request.IsActive, request.IsIncludeDelete, request.Keyword, request.EmployeeIds);
            return _mapper.Map<PagingDataSource<EmployeeDto>>(employees);
        }
    }
}

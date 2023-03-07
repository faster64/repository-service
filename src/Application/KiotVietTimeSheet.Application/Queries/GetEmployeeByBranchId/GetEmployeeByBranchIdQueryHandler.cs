using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using KiotVietTimeSheet.Application.Abstractions;
using KiotVietTimeSheet.Application.Auth;
using KiotVietTimeSheet.Application.Dto;
using KiotVietTimeSheet.Application.Repositories.ReadRepositories;
using KiotVietTimeSheet.Application.Runtime.Exception;
using KiotVietTimeSheet.Domain.AggregatesModels.EmployeeAggregate.Enums;
using KiotVietTimeSheet.Domain.AggregatesModels.EmployeeAggregate.Specifications;
using MediatR;

namespace KiotVietTimeSheet.Application.Queries.GetEmployeeByBranchId
{
    public class GetEmployeeByBranchIdQueryHandler : QueryHandlerBase,
        IRequestHandler<GetEmployeeByBranchIdQuery, List<EmployeeDto>>
    {
        private readonly IMapper _mapper;
        private readonly IEmployeeReadOnlyRepository _employeeReadOnlyRepository;
        private readonly IAuthService _authService;

        public GetEmployeeByBranchIdQueryHandler(
            IAuthService authService,
            IMapper mapper,
            IEmployeeReadOnlyRepository employeeReadOnlyRepository
            ) : base(authService)
        {
            _employeeReadOnlyRepository = employeeReadOnlyRepository;
            _mapper = mapper;
            _authService = authService;
        }

        public async Task<List<EmployeeDto>> Handle(GetEmployeeByBranchIdQuery request, CancellationToken cancellationToken)
        {
            if (request.TypeSearch != null && request.TypeSearch == (int) EmployeeTypeGet.Attendance &&
                _authService.Context.BranchId != request.BranchId &&
                !await _authService.HasAnyPermissionMapWithBranchId(new[] {TimeSheetPermission.Employee_Read},
                    request.BranchId))
            {
                throw new KvTimeSheetUnAuthorizedException(
                    $"Người dùng {_authService.Context.User.UserName} không có quyền thực hiện");
            }
            var result = await _employeeReadOnlyRepository.GetBySpecificationAsync(new FindEmployeeByBranchIdSpec(request.BranchId));
            return _mapper.Map<List<EmployeeDto>>(result);
        }
    }
}


using AutoMapper;
using System.Threading;
using System.Threading.Tasks;
using KiotVietTimeSheet.Application.Repositories.ReadRepositories;
using KiotVietTimeSheet.Application.Auth;
using KiotVietTimeSheet.Application.Dto;
using KiotVietTimeSheet.Application.Abstractions;
using MediatR;
using System.Collections.Generic;
using KiotVietTimeSheet.Domain.AggregatesModels.EmployeeAggregate.Specifications;

namespace KiotVietTimeSheet.Application.Queries.GetEmployeeBranchByEmployeeIds
{
    public class GetEmployeeBranchByEmployeeIdsQueryHandler : QueryHandlerBase,
        IRequestHandler<GetEmployeeBranchByEmployeeIdsQuery, List<EmployeeBranchDto>>
    {
        private readonly IEmployeeBranchReadOnlyRepository _employeeBranchReadOnlyRepository;
        private readonly IMapper _mapper;

        public GetEmployeeBranchByEmployeeIdsQueryHandler(
            IEmployeeBranchReadOnlyRepository employeeBranchReadOnlyRepository,
            IMapper mapper,
            IAuthService authService
        ) : base(authService)
        {
            _employeeBranchReadOnlyRepository = employeeBranchReadOnlyRepository;
            _mapper = mapper;
        }

        public async Task<List<EmployeeBranchDto>> Handle(GetEmployeeBranchByEmployeeIdsQuery request, CancellationToken cancellationToken)
        {
            var result = await _employeeBranchReadOnlyRepository.GetBySpecificationAsync( new FindBranchByEmployeeIdsSpec(request.ListEmployeeId));
            return _mapper.Map<List<EmployeeBranchDto>>(result);
        }
    }
}

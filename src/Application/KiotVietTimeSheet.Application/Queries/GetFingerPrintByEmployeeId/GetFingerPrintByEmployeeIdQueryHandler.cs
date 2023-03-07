using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using KiotVietTimeSheet.Application.Abstractions;
using KiotVietTimeSheet.Application.Auth;
using KiotVietTimeSheet.Application.Dto;
using KiotVietTimeSheet.Application.Repositories.ReadRepositories;
using KiotVietTimeSheet.Domain.AggregatesModels.FingerPrintAggregate.Models;
using KiotVietTimeSheet.Domain.AggregatesModels.FingerPrintAggregate.Specifications;
using MediatR;

namespace KiotVietTimeSheet.Application.Queries.GetFingerPrintByEmployeeId
{
    public class GetFingerPrintByEmployeeIdHandler : QueryHandlerBase,
        IRequestHandler<GetFingerPrintByEmployeeIdQuery, FingerPrintDto>
    {
        private readonly IMapper _mapper;
        private readonly IAuthService _authService;
        private readonly IFingerPrintReadOnlyRepository _fingerPrintReadOnlyRepository;

        public GetFingerPrintByEmployeeIdHandler(
            IAuthService authService,
            IFingerPrintReadOnlyRepository fingerPrintReadOnlyRepository,
            IMapper mapper) : base(authService)
        {
            _mapper = mapper;
            _authService = authService;
            _fingerPrintReadOnlyRepository = fingerPrintReadOnlyRepository;
        }

        public async Task<FingerPrintDto> Handle(GetFingerPrintByEmployeeIdQuery request, CancellationToken cancellationToken)
        {
            var branchId = request.BranchId;
            if (branchId == null || branchId == 0)
            {
                branchId = _authService.Context.BranchId;
            }
            var fingerPrintSpecification = new FindFingerPrintByEmployeeIdSpec(request.EmployeeId).And(new FindFingerPrintByBranchIdSpec(branchId.Value));

            var result = await _fingerPrintReadOnlyRepository.FindBySpecificationAsync(fingerPrintSpecification);

            var ret = _mapper.Map<FingerPrint, FingerPrintDto>(result);
            return ret;
        }
    }
}

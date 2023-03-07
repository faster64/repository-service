using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using KiotVietTimeSheet.Application.Abstractions;
using KiotVietTimeSheet.Application.Auth;
using KiotVietTimeSheet.Application.Dto;
using KiotVietTimeSheet.Application.Repositories.ReadRepositories;
using KiotVietTimeSheet.Domain.AggregatesModels.FingerPrintAggregate.Models;
using KiotVietTimeSheet.SharedKernel.Models;
using MediatR;

namespace KiotVietTimeSheet.Application.Queries.GetFingerMachine
{
    public class GetFingerMachineQueryHandler : QueryHandlerBase,
        IRequestHandler<GetFingerMachineQuery, PagingDataSource<FingerMachineDto>>
    {
        private readonly IFingerMachineReadOnlyRepository _fingerMachineReadOnlyRepository;
        private readonly IMapper _mapper;
        public GetFingerMachineQueryHandler(
            IAuthService authService,
            IFingerMachineReadOnlyRepository fingerMachineReadOnlyRepository,
            IMapper mapper) : base(authService)
        {
            _fingerMachineReadOnlyRepository = fingerMachineReadOnlyRepository;
            _mapper = mapper;
        }

        public async Task<PagingDataSource<FingerMachineDto>> Handle(GetFingerMachineQuery request, CancellationToken cancellationToken)
        {
            var result = await _fingerMachineReadOnlyRepository.FiltersAsync(request.Query, false);
            var ret = _mapper.Map<PagingDataSource<FingerMachine>, PagingDataSource<FingerMachineDto>>(result);
            return ret;
        }
    }
}

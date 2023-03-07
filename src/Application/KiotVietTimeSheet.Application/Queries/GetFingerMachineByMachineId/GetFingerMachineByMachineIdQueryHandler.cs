using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using KiotVietTimeSheet.Application.Abstractions;
using KiotVietTimeSheet.Application.Auth;
using KiotVietTimeSheet.Application.Dto;
using KiotVietTimeSheet.Application.Repositories.ReadRepositories;
using MediatR;

namespace KiotVietTimeSheet.Application.Queries.GetFingerMachineByMachineId
{
    public class GetFingerMachineByMachineIdQueryHandler : QueryHandlerBase,
        IRequestHandler<GetFingerMachineByMachineIdQuery, FingerMachineDto>
    {
        private readonly IFingerMachineReadOnlyRepository _fingerMachineReadOnlyRepository;
        private readonly IMapper _mapper;
        public GetFingerMachineByMachineIdQueryHandler(
            IAuthService authService,
            IFingerMachineReadOnlyRepository fingerMachineReadOnlyRepository,
            IMapper mapper) : base(authService)
        {
            _fingerMachineReadOnlyRepository = fingerMachineReadOnlyRepository;
            _mapper = mapper;
        }

        public async Task<FingerMachineDto> Handle(GetFingerMachineByMachineIdQuery request, CancellationToken cancellationToken)
        {
            return _mapper.Map<FingerMachineDto>(await _fingerMachineReadOnlyRepository.GetFingerMachineByMachineId(request.MachineId));
        }
    }
}

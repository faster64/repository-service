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

namespace KiotVietTimeSheet.Application.Queries.GetFingerPrintByFingerCode
{
    public class GetFingerPrintByFingerCodeHandler : QueryHandlerBase,
        IRequestHandler<GetFingerPrintByFingerCodeQuery, FingerPrintDto>
    {
        private readonly IMapper _mapper;
        private readonly IFingerPrintReadOnlyRepository _fingerPrintReadOnlyRepository;

        public GetFingerPrintByFingerCodeHandler(
            IAuthService authService,
            IFingerPrintReadOnlyRepository fingerPrintReadOnlyRepository,
            IMapper mapper) : base(authService)
        {
            _mapper = mapper;
            _fingerPrintReadOnlyRepository = fingerPrintReadOnlyRepository;
        }

        public async Task<FingerPrintDto> Handle(GetFingerPrintByFingerCodeQuery request, CancellationToken cancellationToken)
        {
            var result = await _fingerPrintReadOnlyRepository.FindBySpecificationAsync(
                new FindFingerPrintByFingerCodeSpec(request.FingerCode)
                    .And(new FindFingerPrintByBranchIdSpec(request.BranchId)));

            var ret = _mapper.Map<FingerPrint, FingerPrintDto>(result);
            return ret;
        }
    }
}

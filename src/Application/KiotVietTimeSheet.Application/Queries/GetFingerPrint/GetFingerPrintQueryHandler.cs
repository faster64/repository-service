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

namespace KiotVietTimeSheet.Application.Queries.GetFingerPrint
{
    public class GetFingerPrintQueryHandler : QueryHandlerBase,
        IRequestHandler<GetFingerPrintQuery, PagingDataSource<FingerPrintDto>>
    {
        private readonly IFingerPrintReadOnlyRepository _fingerPrintReadOnlyRepository;
        private readonly IMapper _mapper;
        public GetFingerPrintQueryHandler(
            IAuthService authService,
            IFingerPrintReadOnlyRepository fingerPrintReadOnlyRepository,
            IMapper mapper) : base(authService)
        {
            _fingerPrintReadOnlyRepository = fingerPrintReadOnlyRepository;
            _mapper = mapper;
        }

        public async Task<PagingDataSource<FingerPrintDto>> Handle(GetFingerPrintQuery request, CancellationToken cancellationToken)
        {
            var result = await _fingerPrintReadOnlyRepository.FiltersAsync(request.Query, false);
            var ret = _mapper.Map<PagingDataSource<FingerPrint>, PagingDataSource<FingerPrintDto>>(result);
            return ret;
        }
    }
}

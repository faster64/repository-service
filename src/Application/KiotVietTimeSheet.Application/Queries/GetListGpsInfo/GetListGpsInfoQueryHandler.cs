using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using KiotVietTimeSheet.Application.Abstractions;
using KiotVietTimeSheet.Application.Auth;
using KiotVietTimeSheet.Application.Dto;
using KiotVietTimeSheet.Application.Repositories.ReadRepositories;
using KiotVietTimeSheet.Domain.AggregatesModels.GpsInfoAggregate.Models;
using KiotVietTimeSheet.SharedKernel.Models;
using MediatR;

namespace KiotVietTimeSheet.Application.Queries.GetListGpsInfo
{
    public class GetListGpsInfoQueryHandler : QueryHandlerBase,
        IRequestHandler<GetListGpsInfoQuery, PagingDataSource<GpsInfoDto>>
    {
        private readonly IMapper _mapper;
        private readonly IGpsInfoReadOnlyRepository _gpsInfoReadOnlyRepository;
        public GetListGpsInfoQueryHandler(
            IAuthService authService,
            IGpsInfoReadOnlyRepository gpsInfoReadOnlyRepository,
            IMapper mapper) 
            : base(authService)
        {
            _mapper = mapper;
            _gpsInfoReadOnlyRepository = gpsInfoReadOnlyRepository;
        }

        public async Task<PagingDataSource<GpsInfoDto>> Handle(GetListGpsInfoQuery request, CancellationToken cancellationToken)
        {
            var result = await _gpsInfoReadOnlyRepository.FiltersAsync(request.Query, request.IncludeSoftDelete);
            var ret = _mapper.Map<PagingDataSource<GpsInfo>, PagingDataSource<GpsInfoDto>>(result);
            return ret;

        }
    }
}

using AutoMapper;
using KiotVietTimeSheet.Application.Abstractions;
using KiotVietTimeSheet.Application.Auth;
using KiotVietTimeSheet.Application.Dto;
using KiotVietTimeSheet.Application.Repositories.ReadRepositories;
using KiotVietTimeSheet.Domain.AggregatesModels.ClockingAggregate.Specifications;
using MediatR;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace KiotVietTimeSheet.Application.Queries.GetClockingsByTimeSheetId
{
    public class GetClockingsByTimeSheetIdQueryHandler : QueryHandlerBase,
        IRequestHandler<GetClockingsByTimeSheetIdQuery, List<ClockingDto>>
    {
        private readonly IClockingReadOnlyRepository _clockingReadOnlyRepository;
        private readonly IMapper _mapper;

        public GetClockingsByTimeSheetIdQueryHandler(
            IClockingReadOnlyRepository clockingReadOnlyRepository,
            IAuthService authService,
            IMapper mapper
        ) : base(authService)
        {
            _clockingReadOnlyRepository = clockingReadOnlyRepository;
            _mapper = mapper;
        }

        public async Task<List<ClockingDto>> Handle(GetClockingsByTimeSheetIdQuery request, CancellationToken cancellationToken)
        {
            var obj = await _clockingReadOnlyRepository.GetBySpecificationAsync(new FindClockingByTimeSheetIdSpec(request.TimeSheetId));
            var result = _mapper.Map<List<ClockingDto>>(obj);
            return result;
        }
    }
}

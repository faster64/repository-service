using AutoMapper;
using KiotVietTimeSheet.Application.Abstractions;
using KiotVietTimeSheet.Application.Auth;
using KiotVietTimeSheet.Application.Dto;
using KiotVietTimeSheet.Application.Repositories.ReadRepositories;
using MediatR;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace KiotVietTimeSheet.Application.Queries.GetClockingForPaySheet
{
    public class GetClockingForPaySheetQueryHandler : QueryHandlerBase,
        IRequestHandler<GetClockingForPaySheetQuery, List<ClockingDto>>
    {
        private readonly IClockingReadOnlyRepository _clockingReadOnlyRepository;
        private readonly IMapper _mapper;

        public GetClockingForPaySheetQueryHandler(
            IClockingReadOnlyRepository clockingReadOnlyRepository,
            IAuthService authService,
            IMapper mapper
        ) : base(authService)
        {
            _clockingReadOnlyRepository = clockingReadOnlyRepository;
            _mapper = mapper;
        }

        public async Task<List<ClockingDto>> Handle(GetClockingForPaySheetQuery request, CancellationToken cancellationToken)
        {
            var clockings = await _clockingReadOnlyRepository.GetClockingForPaySheet(request.From, request.To, request.EmployeeIds);
            return _mapper.Map<List<ClockingDto>>(clockings);
        }
    }
}

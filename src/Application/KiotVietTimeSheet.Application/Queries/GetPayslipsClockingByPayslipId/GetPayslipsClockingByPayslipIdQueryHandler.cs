using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using KiotVietTimeSheet.Application.Abstractions;
using KiotVietTimeSheet.Application.Auth;
using KiotVietTimeSheet.Application.Dto;
using KiotVietTimeSheet.Application.Repositories.ReadRepositories;
using KiotVietTimeSheet.SharedKernel.Models;
using MediatR;

namespace KiotVietTimeSheet.Application.Queries.GetPayslipsClockingByPayslipId
{
    public class GetPayslipsClockingByPayslipIdQueryHandler : QueryHandlerBase,
        IRequestHandler<GetPayslipsClockingByPayslipIdQuery, PagingDataSource<PayslipClockingDto>>
    {
        #region Properties
        private readonly IMapper _mapper;
        private readonly IPayslipClockingReadOnlyRepository _payslipClockingReadOnlyRepository;

        #endregion

        public GetPayslipsClockingByPayslipIdQueryHandler(
            IAuthService authService,
            IMapper mapper,
            IPayslipClockingReadOnlyRepository payslipClockingReadOnlyRepository
        ) : base(authService)
        {
            _mapper = mapper;
            _payslipClockingReadOnlyRepository = payslipClockingReadOnlyRepository;
        }

        public async Task<PagingDataSource<PayslipClockingDto>> Handle(GetPayslipsClockingByPayslipIdQuery request,
            CancellationToken cancellationToken)
        {
            var ds = await _payslipClockingReadOnlyRepository.GetPayslipsClockingByPayslipIdAsync(request.Filter);

            var pgDataSource = new PagingDataSource<PayslipClockingDto>
            {
                Total = ds.Total,
                Data = _mapper.Map<List<PayslipClockingDto>>(ds.Data),
            };
            return pgDataSource;
        }
    }
}

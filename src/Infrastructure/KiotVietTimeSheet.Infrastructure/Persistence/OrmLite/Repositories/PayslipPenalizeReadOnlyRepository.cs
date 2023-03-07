using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using KiotVietTimeSheet.Application.Auth;
using KiotVietTimeSheet.Application.Dto;
using KiotVietTimeSheet.Application.Repositories.ReadRepositories;
using KiotVietTimeSheet.Domain.AggregatesModels.PayslipAggregate.Models;
using KiotVietTimeSheet.Domain.AggregatesModels.PayslipAggregate.Specifications;
using ServiceStack.Data;

namespace KiotVietTimeSheet.Infrastructure.Persistence.OrmLite.Repositories
{
    public class PayslipPenalizeReadOnlyRepository : OrmLiteRepository<PayslipPenalize, long>, IPayslipPenalizeReadOnlyRepository
    {
        private readonly IMapper _mapper;

        public PayslipPenalizeReadOnlyRepository(IMapper mapper, IDbConnectionFactory db, IAuthService authService) :
            base(db, authService)
        {
            _mapper = mapper;
        }

        public async Task<List<PayslipPenalizeDto>> GetPayslipsPenalizes(List<long> payslipIds)
        {
            var payslipPenalizes = await GetBySpecificationAsync(
                new FindPayslipPenalizeByPayslipIds(payslipIds)
                    .And(new FindPayslipPenalizeByIsActiveSpec(true))
                );
            return _mapper.Map<List<PayslipPenalizeDto>>(payslipPenalizes);
        }
    }
}

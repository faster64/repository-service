using System.Collections.Generic;
using KiotVietTimeSheet.Application.Auth;
using KiotVietTimeSheet.Application.Dto;
using KiotVietTimeSheet.Application.Repositories.ReadRepositories;
using KiotVietTimeSheet.SharedKernel.Models;
using ServiceStack.Data;
using ServiceStack.OrmLite;
using System.Threading.Tasks;
using KiotVietTimeSheet.Domain.AggregatesModels.PayRateAggregate.Models;
using KiotVietTimeSheet.Domain.AggregatesModels.PayRateTemplateAggregate.Models;
using KiotVietTimeSheet.Utilities;

namespace KiotVietTimeSheet.Infrastructure.Persistence.OrmLite.Repositories
{
    public class PayRateReadOnlyRepository : OrmLiteRepository<PayRate, long>, IPayRateReadOnlyRepository
    {
        public PayRateReadOnlyRepository(IDbConnectionFactory db, IAuthService authService) : base(db, authService) { }
        public async Task<PagingDataSource<PayRateDto>> FiltersAsync(ISqlExpression query) => await LoadSelectDataSourceAsync<PayRateDto>(query);
        public async Task<List<PayRate>> FiltersAsync(List<long> payrateTemplateIdList)
        {
            if (payrateTemplateIdList != null && payrateTemplateIdList.Count > 0)
            {
                var query = Db.From<PayRate>().Where(x => payrateTemplateIdList.Contains(x.PayRateTemplateId.Value));
                return await Db.LoadSelectAsync(query);
            }
            return null;
        }

        public async Task<(bool, string)> CheckPayRateTemplateDeleteByEmployeeIds(List<long> employeeIds)
        {
            var query = Db.From<PayRate>().Join<PayRateTemplate>((p, pt) => p.PayRateTemplateId == pt.Id)
                .Where<PayRate>(p => p.TenantId == AuthService.Context.TenantId && employeeIds.Contains(p.EmployeeId))
                .Where<PayRateTemplate>(pb => pb.Status == (byte)PayRateTemplateStatus.Deleted)
                .Select<PayRate, PayRateTemplate>((p, pt) => new
                {
                    p.Id,
                    Name = pt.Name
                });
            var result = await Db.SingleAsync<PayRateTemplateDto>(query);
            return (result != null, result?.Name);

        }
    }
}

using KiotVietTimeSheet.Domain.AggregatesModels.CommissionAggregate.Models;
using ServiceStack;
using ServiceStack.DataAnnotations;

namespace KiotVietTimeSheet.Infrastructure.Persistence.OrmLite.PocoMapping
{
    public class CommissionBranchMapping
    {
        protected CommissionBranchMapping(){}
        public static void Mapping()
        {
            var model = typeof(CommissionBranch);

            model.AddAttributes(new AliasAttribute("CommissionBranch"));
            model.GetProperty("Id")
                .AddAttributes(new AutoIncrementAttribute());
            model.GetProperty("Commission")
                .AddAttributes(new ReferenceAttribute());
        }
    }
}

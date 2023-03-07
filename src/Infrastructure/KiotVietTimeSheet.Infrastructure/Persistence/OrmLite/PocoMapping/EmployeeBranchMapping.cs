using KiotVietTimeSheet.Domain.AggregatesModels.EmployeeAggregate.Models;
using ServiceStack;
using ServiceStack.DataAnnotations;

namespace KiotVietTimeSheet.Infrastructure.Persistence.OrmLite.PocoMapping
{
    public class EmployeeBranchMapping
    {
        protected EmployeeBranchMapping(){}
        public static void Mapping()
        {
            var model = typeof(EmployeeBranch);

            model.AddAttributes(new AliasAttribute("EmployeeBranch"));
            model.GetProperty("Id")
                .AddAttributes(new AutoIncrementAttribute());
            model.GetProperty("Employee")
                .AddAttributes(new ReferenceAttribute());
        }
    }
}

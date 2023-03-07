using KiotVietTimeSheet.Domain.AggregatesModels.DepartmentAggregate.Models;
using ServiceStack;
using ServiceStack.DataAnnotations;

namespace KiotVietTimeSheet.Infrastructure.Persistence.OrmLite.PocoMapping
{
    public class DepartmentMapping
    {
        protected DepartmentMapping(){}
        public static void Mapping()
        {
            var model = typeof(Department);

            model.AddAttributes(new AliasAttribute("Department"));
            model.GetProperty("Id")
                 .AddAttributes(new AutoIncrementAttribute());
        }
    }
}

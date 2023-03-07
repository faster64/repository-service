using KiotVietTimeSheet.Domain.AggregatesModels.EmployeeAggregate.Models;
using ServiceStack;
using ServiceStack.DataAnnotations;

namespace KiotVietTimeSheet.Infrastructure.Persistence.OrmLite.PocoMapping
{
    public class EmployeeProfilePictureMapping
    {
        protected EmployeeProfilePictureMapping(){}
        public static void Mapping()
        {
            var model = typeof(EmployeeProfilePicture);

            model.AddAttributes(new AliasAttribute("EmployeeProfilePicture"));
            model.GetProperty("Id")
                 .AddAttributes(new AutoIncrementAttribute());
            model.GetProperty("EmployeeId")
                 .AddAttributes(new ForeignKeyAttribute(typeof(Employee)));
            model.GetProperty("Employee")
                 .AddAttributes(new ReferenceAttribute());
        }
    }
}

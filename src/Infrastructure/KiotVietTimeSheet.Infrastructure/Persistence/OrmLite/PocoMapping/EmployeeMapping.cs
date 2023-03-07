using KiotVietTimeSheet.Domain.AggregatesModels.EmployeeAggregate.Models;
using ServiceStack;
using ServiceStack.DataAnnotations;
using KiotVietTimeSheet.Application.Dto;

namespace KiotVietTimeSheet.Infrastructure.Persistence.OrmLite.PocoMapping
{
    public class EmployeeMapping
    {
        protected EmployeeMapping(){}
        public static void Mapping()
        {
            var model = typeof(Employee);

            model.AddAttributes(new AliasAttribute("Employee"));
            model.GetProperty("Id")
                 .AddAttributes(new AutoIncrementAttribute());
            model.GetProperty("ProfilePictures")
                 .AddAttributes(new ReferenceAttribute());
            model.GetProperty("EmployeeBranches")
                .AddAttributes(new ReferenceAttribute());
            model.GetProperty("Department")
                 .AddAttributes(new ReferenceAttribute());
            model.GetProperty("JobTitle")
                 .AddAttributes(new ReferenceAttribute());
            model.GetProperty("Clockings")
                .AddAttributes(new ReferenceAttribute());

            // For read model
            var readModel = typeof(EmployeeDto);
            readModel.AddAttributes(new AliasAttribute("Employee"));
            readModel.GetProperty("ProfilePictures")
                 .AddAttributes(new ReferenceAttribute());
            readModel.GetProperty("EmployeeBranches")
                .AddAttributes(new ReferenceAttribute());
            readModel.GetProperty("Department")
                 .AddAttributes(new ReferenceAttribute());
            readModel.GetProperty("JobTitle")
                 .AddAttributes(new ReferenceAttribute());
            readModel.GetProperty("Clockings")
                .AddAttributes(new IgnoreAttribute());
        }
    }
}

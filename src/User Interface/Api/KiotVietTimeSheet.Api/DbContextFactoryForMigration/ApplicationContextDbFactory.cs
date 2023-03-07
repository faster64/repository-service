using KiotVietTimeSheet.Infrastructure.Persistence.Ef;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.IO;
using KiotVietTimeSheet.Infrastructure.Configuration;

namespace KiotVietTimeSheet.Api.DbContextFactoryForMigration
{
    public class ApplicationContextDbFactory : IDesignTimeDbContextFactory<EfDbContext>
    {
        EfDbContext IDesignTimeDbContextFactory<EfDbContext>.CreateDbContext(string[] args)
        {
            IConfigurationRoot configuration = new ConfigurationBuilder()
                                                .SetBasePath(Directory.GetCurrentDirectory())
                                                .AddJsonFile("appsettings.json")
                                                .Build();
            var infrastructureConfiguration = new InfrastructureConfiguration(configuration);

            var optionsBuilder = new DbContextOptionsBuilder<EfDbContext>();
            optionsBuilder.UseSqlServer(InfrastructureConfiguration.ConnectionString,
                        b => b.MigrationsAssembly("KiotVietTimeSheet.Api"));

            return new EfDbContext(optionsBuilder.Options);
        }
    }
}

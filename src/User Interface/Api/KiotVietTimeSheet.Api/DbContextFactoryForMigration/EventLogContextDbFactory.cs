using System.IO;
using KiotVietTimeSheet.Infrastructure.Configuration;
using KiotVietTimeSheet.Infrastructure.EventBus.EventLog.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace KiotVietTimeSheet.Api.DbContextFactoryForMigration
{
    public class EventLogContextDbFactory : IDesignTimeDbContextFactory<EfIntegrationEventLogContext>
    {
        EfIntegrationEventLogContext IDesignTimeDbContextFactory<EfIntegrationEventLogContext>.CreateDbContext(string[] args)
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            var infrastructureConfiguration = new InfrastructureConfiguration(configuration);

            var optionsBuilder = new DbContextOptionsBuilder<EfIntegrationEventLogContext>();
            optionsBuilder.UseSqlServer(InfrastructureConfiguration.ConnectionString,
                b => b.MigrationsAssembly("KiotVietTimeSheet.Api"));

            return new EfIntegrationEventLogContext(optionsBuilder.Options);
        }
    }
}

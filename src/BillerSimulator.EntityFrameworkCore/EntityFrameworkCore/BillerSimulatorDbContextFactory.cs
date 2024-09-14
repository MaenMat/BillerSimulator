using System;
using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace BillerSimulator.EntityFrameworkCore;

/* This class is needed for EF Core console commands
 * (like Add-Migration and Update-Database commands) */
public class BillerSimulatorDbContextFactory : IDesignTimeDbContextFactory<BillerSimulatorDbContext>
{
    public BillerSimulatorDbContext CreateDbContext(string[] args)
    {
        BillerSimulatorEfCoreEntityExtensionMappings.Configure();

        var configuration = BuildConfiguration();

        var builder = new DbContextOptionsBuilder<BillerSimulatorDbContext>()
            .UseSqlServer(configuration.GetConnectionString("Default"));

        return new BillerSimulatorDbContext(builder.Options);
    }

    private static IConfigurationRoot BuildConfiguration()
    {
        var builder = new ConfigurationBuilder()
            .SetBasePath(Path.Combine(Directory.GetCurrentDirectory(), "../BillerSimulator.DbMigrator/"))
            .AddJsonFile("appsettings.json", optional: false);

        return builder.Build();
    }
}

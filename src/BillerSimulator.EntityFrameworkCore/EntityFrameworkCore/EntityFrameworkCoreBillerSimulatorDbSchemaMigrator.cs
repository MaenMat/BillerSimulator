using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using BillerSimulator.Data;
using Volo.Abp.DependencyInjection;

namespace BillerSimulator.EntityFrameworkCore;

public class EntityFrameworkCoreBillerSimulatorDbSchemaMigrator
    : IBillerSimulatorDbSchemaMigrator, ITransientDependency
{
    private readonly IServiceProvider _serviceProvider;

    public EntityFrameworkCoreBillerSimulatorDbSchemaMigrator(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task MigrateAsync()
    {
        /* We intentionally resolving the BillerSimulatorDbContext
         * from IServiceProvider (instead of directly injecting it)
         * to properly get the connection string of the current tenant in the
         * current scope.
         */

        await _serviceProvider
            .GetRequiredService<BillerSimulatorDbContext>()
            .Database
            .MigrateAsync();
    }
}

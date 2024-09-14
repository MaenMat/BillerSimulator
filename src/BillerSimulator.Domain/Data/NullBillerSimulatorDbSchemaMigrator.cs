using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;

namespace BillerSimulator.Data;

/* This is used if database provider does't define
 * IBillerSimulatorDbSchemaMigrator implementation.
 */
public class NullBillerSimulatorDbSchemaMigrator : IBillerSimulatorDbSchemaMigrator, ITransientDependency
{
    public Task MigrateAsync()
    {
        return Task.CompletedTask;
    }
}

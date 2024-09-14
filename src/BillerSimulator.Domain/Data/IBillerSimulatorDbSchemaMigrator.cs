using System.Threading.Tasks;

namespace BillerSimulator.Data;

public interface IBillerSimulatorDbSchemaMigrator
{
    Task MigrateAsync();
}

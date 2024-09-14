using BillerSimulator.EntityFrameworkCore;
using Volo.Abp.Autofac;
using Volo.Abp.Modularity;

namespace BillerSimulator.DbMigrator;

[DependsOn(
    typeof(AbpAutofacModule),
    typeof(BillerSimulatorEntityFrameworkCoreModule),
    typeof(BillerSimulatorApplicationContractsModule)
)]
public class BillerSimulatorDbMigratorModule : AbpModule
{
}

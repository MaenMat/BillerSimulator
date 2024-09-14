using BillerSimulator.EntityFrameworkCore;
using Volo.Abp.Modularity;

namespace BillerSimulator;

[DependsOn(
    typeof(BillerSimulatorEntityFrameworkCoreTestModule)
    )]
public class BillerSimulatorDomainTestModule : AbpModule
{

}

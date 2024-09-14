using Volo.Abp.Modularity;

namespace BillerSimulator;

[DependsOn(
    typeof(BillerSimulatorApplicationModule),
    typeof(BillerSimulatorDomainTestModule)
    )]
public class BillerSimulatorApplicationTestModule : AbpModule
{

}

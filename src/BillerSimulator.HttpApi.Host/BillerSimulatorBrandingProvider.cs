using Volo.Abp.DependencyInjection;
using Volo.Abp.Ui.Branding;

namespace BillerSimulator;

[Dependency(ReplaceServices = true)]
public class BillerSimulatorBrandingProvider : DefaultBrandingProvider
{
    public override string AppName => "BillerSimulator";
}

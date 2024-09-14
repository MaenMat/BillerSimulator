using BillerSimulator.Localization;
using Volo.Abp.AspNetCore.Components;

namespace BillerSimulator.Blazor;

public abstract class BillerSimulatorComponentBase : AbpComponentBase
{
    protected BillerSimulatorComponentBase()
    {
        LocalizationResource = typeof(BillerSimulatorResource);
    }
}

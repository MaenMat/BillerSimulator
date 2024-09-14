using BillerSimulator.Localization;
using Volo.Abp.Application.Services;

namespace BillerSimulator;

/* Inherit your application services from this class.
 */
public abstract class BillerSimulatorAppService : ApplicationService
{
    protected BillerSimulatorAppService()
    {
        LocalizationResource = typeof(BillerSimulatorResource);
    }
}

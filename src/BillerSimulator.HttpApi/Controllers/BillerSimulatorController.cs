using BillerSimulator.Localization;
using Volo.Abp.AspNetCore.Mvc;

namespace BillerSimulator.Controllers;

/* Inherit your controllers from this class.
 */
public abstract class BillerSimulatorController : AbpControllerBase
{
    protected BillerSimulatorController()
    {
        LocalizationResource = typeof(BillerSimulatorResource);
    }
}

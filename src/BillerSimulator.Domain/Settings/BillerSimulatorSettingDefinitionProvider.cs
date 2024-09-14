using Volo.Abp.Settings;

namespace BillerSimulator.Settings;

public class BillerSimulatorSettingDefinitionProvider : SettingDefinitionProvider
{
    public override void Define(ISettingDefinitionContext context)
    {
        //Define your own settings here. Example:
        //context.Add(new SettingDefinition(BillerSimulatorSettings.MySetting1));
    }
}

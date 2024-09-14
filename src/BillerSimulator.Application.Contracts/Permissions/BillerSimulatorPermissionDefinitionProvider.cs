using BillerSimulator.Localization;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Localization;
using Volo.Abp.MultiTenancy;

namespace BillerSimulator.Permissions;

public class BillerSimulatorPermissionDefinitionProvider : PermissionDefinitionProvider
{
    public override void Define(IPermissionDefinitionContext context)
    {
        var myGroup = context.AddGroup(BillerSimulatorPermissions.GroupName);

        myGroup.AddPermission(BillerSimulatorPermissions.Dashboard.Host, L("Permission:Dashboard"), MultiTenancySides.Host);
        myGroup.AddPermission(BillerSimulatorPermissions.Dashboard.Tenant, L("Permission:Dashboard"), MultiTenancySides.Tenant);

        //Define your own permissions here. Example:
        //myGroup.AddPermission(BillerSimulatorPermissions.MyPermission1, L("Permission:MyPermission1"));

        var customerPermission = myGroup.AddPermission(BillerSimulatorPermissions.Customers.Default, L("Permission:Customers"));
        customerPermission.AddChild(BillerSimulatorPermissions.Customers.Create, L("Permission:Create"));
        customerPermission.AddChild(BillerSimulatorPermissions.Customers.Edit, L("Permission:Edit"));
        customerPermission.AddChild(BillerSimulatorPermissions.Customers.Delete, L("Permission:Delete"));

        var invoicePermission = myGroup.AddPermission(BillerSimulatorPermissions.Invoices.Default, L("Permission:Invoices"));
        invoicePermission.AddChild(BillerSimulatorPermissions.Invoices.Create, L("Permission:Create"));
        invoicePermission.AddChild(BillerSimulatorPermissions.Invoices.Edit, L("Permission:Edit"));
        invoicePermission.AddChild(BillerSimulatorPermissions.Invoices.Delete, L("Permission:Delete"));
    }

    private static LocalizableString L(string name)
    {
        return LocalizableString.Create<BillerSimulatorResource>(name);
    }
}
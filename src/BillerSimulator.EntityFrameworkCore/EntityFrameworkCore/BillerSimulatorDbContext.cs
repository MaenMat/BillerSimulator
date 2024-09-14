using BillerSimulator.Invoices;
using BillerSimulator.Customers;
using Volo.Abp.EntityFrameworkCore.Modeling;
using Microsoft.EntityFrameworkCore;
using Volo.Abp.AuditLogging.EntityFrameworkCore;
using Volo.Abp.BackgroundJobs.EntityFrameworkCore;
using Volo.Abp.BlobStoring.Database.EntityFrameworkCore;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.FeatureManagement.EntityFrameworkCore;
using Volo.Abp.Identity;
using Volo.Abp.Identity.EntityFrameworkCore;
using Volo.Abp.LanguageManagement.EntityFrameworkCore;
using Volo.Abp.PermissionManagement.EntityFrameworkCore;
using Volo.Abp.SettingManagement.EntityFrameworkCore;
using Volo.Abp.TextTemplateManagement.EntityFrameworkCore;
using Volo.Saas.EntityFrameworkCore;
using Volo.Saas.Editions;
using Volo.Saas.Tenants;
using Volo.Abp.Gdpr;
using Volo.Abp.OpenIddict.EntityFrameworkCore;

namespace BillerSimulator.EntityFrameworkCore;

[ReplaceDbContext(typeof(IIdentityProDbContext))]
[ReplaceDbContext(typeof(ISaasDbContext))]
[ConnectionStringName("Default")]
public class BillerSimulatorDbContext :
    AbpDbContext<BillerSimulatorDbContext>,
    IIdentityProDbContext,
    ISaasDbContext
{
    public DbSet<Invoice> Invoices { get; set; }
    public DbSet<Customer> Customers { get; set; }
    /* Add DbSet properties for your Aggregate Roots / Entities here. */

    #region Entities from the modules

    /* Notice: We only implemented IIdentityProDbContext and ISaasDbContext
     * and replaced them for this DbContext. This allows you to perform JOIN
     * queries for the entities of these modules over the repositories easily. You
     * typically don't need that for other modules. But, if you need, you can
     * implement the DbContext interface of the needed module and use ReplaceDbContext
     * attribute just like IIdentityProDbContext and ISaasDbContext.
     *
     * More info: Replacing a DbContext of a module ensures that the related module
     * uses this DbContext on runtime. Otherwise, it will use its own DbContext class.
     */

    // Identity
    public DbSet<IdentityUser> Users { get; set; }
    public DbSet<IdentityRole> Roles { get; set; }
    public DbSet<IdentityClaimType> ClaimTypes { get; set; }
    public DbSet<OrganizationUnit> OrganizationUnits { get; set; }
    public DbSet<IdentitySecurityLog> SecurityLogs { get; set; }
    public DbSet<IdentityLinkUser> LinkUsers { get; set; }
    public DbSet<IdentityUserDelegation> UserDelegations { get; }

    // SaaS
    public DbSet<Tenant> Tenants { get; set; }
    public DbSet<Edition> Editions { get; set; }
    public DbSet<TenantConnectionString> TenantConnectionStrings { get; set; }

    #endregion

    public BillerSimulatorDbContext(DbContextOptions<BillerSimulatorDbContext> options)
        : base(options)
    {

    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        /* Include modules to your migration db context */

        builder.ConfigurePermissionManagement();
        builder.ConfigureSettingManagement();
        builder.ConfigureBackgroundJobs();
        builder.ConfigureAuditLogging();
        builder.ConfigureIdentityPro();
        builder.ConfigureOpenIddictPro();
        builder.ConfigureFeatureManagement();
        builder.ConfigureLanguageManagement();
        builder.ConfigureSaas();
        builder.ConfigureTextTemplateManagement();
        builder.ConfigureBlobStoring();
        builder.ConfigureGdpr();

        /* Configure your own tables/entities inside here */

        //builder.Entity<YourEntity>(b =>
        //{
        //    b.ToTable(BillerSimulatorConsts.DbTablePrefix + "YourEntities", BillerSimulatorConsts.DbSchema);
        //    b.ConfigureByConvention(); //auto configure for the base class props
        //    //...
        //});
        if (builder.IsHostDatabase())
        {

        }
        if (builder.IsHostDatabase())
        {

        }
        if (builder.IsHostDatabase())
        {

        }
        if (builder.IsHostDatabase())
        {

        }
        if (builder.IsHostDatabase())
        {

        }
        if (builder.IsHostDatabase())
        {
            builder.Entity<Customer>(b =>
{
    b.ToTable(BillerSimulatorConsts.DbTablePrefix + "Customers", BillerSimulatorConsts.DbSchema);
    b.ConfigureByConvention();
    b.Property(x => x.CustomerNumber).HasColumnName(nameof(Customer.CustomerNumber));
    b.Property(x => x.FullName).HasColumnName(nameof(Customer.FullName)).IsRequired();
    b.Property(x => x.PhoneNumber).HasColumnName(nameof(Customer.PhoneNumber)).IsRequired();
    b.Property(x => x.Email).HasColumnName(nameof(Customer.Email));
    b.Property(x => x.Address).HasColumnName(nameof(Customer.Address));
});

        }
        if (builder.IsHostDatabase())
        {
            builder.Entity<Invoice>(b =>
{
    b.ToTable(BillerSimulatorConsts.DbTablePrefix + "Invoices", BillerSimulatorConsts.DbSchema);
    b.ConfigureByConvention();
    b.Property(x => x.InvoiceNumber).HasColumnName(nameof(Invoice.InvoiceNumber));
    b.Property(x => x.Amount).HasColumnName(nameof(Invoice.Amount));
    b.Property(x => x.DueDate).HasColumnName(nameof(Invoice.DueDate));
    b.Property(x => x.PaidDate).HasColumnName(nameof(Invoice.PaidDate));
    b.Property(x => x.InvoiceStatus).HasColumnName(nameof(Invoice.InvoiceStatus));
    b.HasOne<Customer>().WithMany().IsRequired().HasForeignKey(x => x.CustomerId).OnDelete(DeleteBehavior.NoAction);
});

        }
    }
}
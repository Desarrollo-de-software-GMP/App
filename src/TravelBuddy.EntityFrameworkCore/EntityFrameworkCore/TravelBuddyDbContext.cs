using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Reflection;
using TravelBuddy.Califications;
using TravelBuddy.Common;
using TravelBuddy.Destinations;
using Volo.Abp.AuditLogging.EntityFrameworkCore;
using Volo.Abp.BackgroundJobs.EntityFrameworkCore;
using Volo.Abp.BlobStoring.Database.EntityFrameworkCore;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore.Modeling;
using Volo.Abp.FeatureManagement.EntityFrameworkCore;
using Volo.Abp.Identity;
using Volo.Abp.Identity.EntityFrameworkCore;
using Volo.Abp.OpenIddict.EntityFrameworkCore;
using Volo.Abp.PermissionManagement;
using Volo.Abp.PermissionManagement.EntityFrameworkCore;
using Volo.Abp.SettingManagement.EntityFrameworkCore;
using Volo.Abp.TenantManagement;
using Volo.Abp.TenantManagement.EntityFrameworkCore;
using Volo.Abp.Users;

namespace TravelBuddy.EntityFrameworkCore;

[ReplaceDbContext(typeof(IIdentityDbContext))]
[ReplaceDbContext(typeof(ITenantManagementDbContext))]
[ReplaceDbContext(typeof(IPermissionManagementDbContext))]
[ConnectionStringName("Default")]
public class TravelBuddyDbContext :
    AbpDbContext<TravelBuddyDbContext>,
    ITenantManagementDbContext,
    IIdentityDbContext,
    IPermissionManagementDbContext
{
    /* Entidades de tu aplicación */
    public DbSet<Destination> Destinations { get; set; }
    public DbSet<Calification> Califications { get; set; }

    #region Entities from the modules

    // Identity
    public DbSet<IdentityUser> Users { get; set; }
    public DbSet<IdentityRole> Roles { get; set; }
    public DbSet<IdentityClaimType> ClaimTypes { get; set; }
    public DbSet<OrganizationUnit> OrganizationUnits { get; set; }
    public DbSet<IdentitySecurityLog> SecurityLogs { get; set; }
    public DbSet<IdentityLinkUser> LinkUsers { get; set; }
    public DbSet<IdentityUserDelegation> UserDelegations { get; set; }
    public DbSet<IdentitySession> Sessions { get; set; }

    // Tenant Management 
    public DbSet<Tenant> Tenants { get; set; }
    public DbSet<TenantConnectionString> TenantConnectionStrings { get; set; }

    // Permission Management
    public DbSet<PermissionGrant> PermissionGrants { get; set; }
    public DbSet<PermissionGroupDefinitionRecord> PermissionGroups { get; set; }
    public DbSet<PermissionDefinitionRecord> Permissions { get; set; }

    #endregion



 
    private readonly ICurrentUser? _currentUser;


    private Guid? CurrentUserId { get; set; }


    public TravelBuddyDbContext(DbContextOptions<TravelBuddyDbContext> options, ICurrentUser currentUser)
        : base(options)
    {
        _currentUser = currentUser;
        CurrentUserId = _currentUser?.Id;
    }

   
    public TravelBuddyDbContext(DbContextOptions<TravelBuddyDbContext> options)
        : base(options)
    {
        _currentUser = null;
        CurrentUserId = null;
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        /* Configurar módulos */
        builder.ConfigurePermissionManagement();
        builder.ConfigureSettingManagement();
        builder.ConfigureBackgroundJobs();
        builder.ConfigureAuditLogging();
        builder.ConfigureFeatureManagement();
        builder.ConfigureIdentity();
        builder.ConfigureOpenIddict();
        builder.ConfigureTenantManagement();
        builder.ConfigureBlobStoring();

        /* Configuración de tus entidades */
        builder.Entity<Destination>(d =>
        {
            d.ToTable("Destinations");
            d.ConfigureByConvention();
            d.Property(x => x.Name).IsRequired().HasMaxLength(128);
            d.Property(x => x.Country).IsRequired().HasMaxLength(64);
            d.Property(x => x.Poblation).IsRequired().HasMaxLength(64);

            d.OwnsOne(x => x.Coordinates, coord =>
            {
                coord.Property(c => c.Latitude).HasColumnName("Latitude").IsRequired();
                coord.Property(c => c.Longitude).HasColumnName("Longitude").IsRequired();
            });
        });

        builder.Entity<Calification>(b =>
        {
            b.ToTable("AppCalifications");
            b.ConfigureByConvention();
        });

        var userOwnedTypes = builder.Model.GetEntityTypes()
            .Where(t => typeof(IUserOwned).IsAssignableFrom(t.ClrType))
            .ToList();

        foreach (var et in userOwnedTypes)
        {
            var method = typeof(TravelBuddyDbContext)
                .GetMethod(nameof(ApplyUserFilter), BindingFlags.NonPublic | BindingFlags.Instance)
                ?.MakeGenericMethod(et.ClrType);

            method?.Invoke(this, new object[] { builder });
        }
    }

    private void ApplyUserFilter<TEntity>(ModelBuilder builder)
        where TEntity : class, IUserOwned
    {
        builder.Entity<TEntity>().HasQueryFilter(e => e.UserId == CurrentUserId);
    }
}
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using NSubstitute;
using Volo.Abp;
using Volo.Abp.Authorization;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Autofac;
using Volo.Abp.BackgroundJobs;
using Volo.Abp.Data;
using Volo.Abp.Modularity;
using Volo.Abp.PermissionManagement;
using Volo.Abp.Threading;
using Volo.Abp.Users;

namespace TravelBuddy;

[DependsOn(
    typeof(AbpAutofacModule),
    typeof(AbpTestBaseModule),
    typeof(AbpAuthorizationModule),
    typeof(AbpBackgroundJobsAbstractionsModule)
)]
public class TravelBuddyTestBaseModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        Configure<AbpBackgroundJobOptions>(options =>
        {
            options.IsJobExecutionEnabled = false;
        });

        
        var currentUserMock = Substitute.For<ICurrentUser>();
        currentUserMock.IsAuthenticated.Returns(true);
        currentUserMock.Id.Returns(Guid.NewGuid());
        context.Services.Replace(ServiceDescriptor.Singleton<ICurrentUser>(currentUserMock));

        context.Services.AddAlwaysAllowAuthorization();

   

        // Store
        var storeMock = Substitute.For<IDynamicPermissionDefinitionStore>();
        storeMock.GetGroupsAsync().ReturnsForAnyArgs(Task.FromResult((IReadOnlyList<PermissionGroupDefinition>)new List<PermissionGroupDefinition>()));
        context.Services.Replace(ServiceDescriptor.Singleton(storeMock));

        // Repositorios
        var groupRepoMock = Substitute.For<IPermissionGroupDefinitionRecordRepository>();
        groupRepoMock.GetListAsync().ReturnsForAnyArgs(Task.FromResult(new List<PermissionGroupDefinitionRecord>()));
        context.Services.Replace(ServiceDescriptor.Singleton(groupRepoMock));

        var defRepoMock = Substitute.For<IPermissionDefinitionRecordRepository>();
        defRepoMock.GetListAsync().ReturnsForAnyArgs(Task.FromResult(new List<PermissionDefinitionRecord>()));
        context.Services.Replace(ServiceDescriptor.Singleton(defRepoMock));

        var grantRepoMock = Substitute.For<IPermissionGrantRepository>();
        grantRepoMock.GetListAsync(default, default).ReturnsForAnyArgs(Task.FromResult(new List<PermissionGrant>()));
        context.Services.Replace(ServiceDescriptor.Singleton(grantRepoMock));
    }

    public override void OnApplicationInitialization(ApplicationInitializationContext context)
    {
        SeedTestData(context);
    }

    private static void SeedTestData(ApplicationInitializationContext context)
    {
        AsyncHelper.RunSync(async () =>
        {
            using (var scope = context.ServiceProvider.CreateScope())
            {
                await scope.ServiceProvider
                    .GetRequiredService<IDataSeeder>()
                    .SeedAsync();
            }
        });
    }
}
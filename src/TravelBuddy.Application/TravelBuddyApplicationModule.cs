using Microsoft.Extensions.DependencyInjection;
using System.Net.Http;
using TravelBuddy.Destinations;
using Volo.Abp.Account;
using Volo.Abp.AutoMapper;
using Volo.Abp.FeatureManagement;
using Volo.Abp.Identity;
using Volo.Abp.Modularity;
using Volo.Abp.PermissionManagement;
using Volo.Abp.SettingManagement;
using Volo.Abp.TenantManagement;

namespace TravelBuddy;

[DependsOn(
    typeof(TravelBuddyDomainModule),
    typeof(TravelBuddyApplicationContractsModule),
    typeof(AbpPermissionManagementApplicationModule),
    typeof(AbpFeatureManagementApplicationModule),
    typeof(AbpIdentityApplicationModule),
    typeof(AbpAccountApplicationModule),
    typeof(AbpTenantManagementApplicationModule),
    typeof(AbpSettingManagementApplicationModule)
    )]
public class TravelBuddyApplicationModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        Configure<AbpAutoMapperOptions>(options =>
        {
            options.AddMaps<TravelBuddyApplicationModule>();
        });
        context.Services.AddTransient<ICitySearchService, GeoDbCitySearchService>();
        context.Services.AddSingleton<HttpClient>();
    }
}

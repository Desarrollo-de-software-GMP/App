using System.Net.Http; 
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.Modularity;

namespace TravelBuddy;

[DependsOn(
    typeof(TravelBuddyApplicationModule),
    typeof(TravelBuddyDomainTestModule)
)]
public class TravelBuddyApplicationTestModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        // Registra una instancia simple de HttpClient para que
        // la inyección de dependencias funcione en los tests.
        context.Services.AddSingleton<HttpClient>();
    }
}

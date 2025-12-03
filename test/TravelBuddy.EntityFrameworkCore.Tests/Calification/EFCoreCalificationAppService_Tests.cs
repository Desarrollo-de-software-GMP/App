using TravelBuddy.Califications;
using TravelBuddy.EntityFrameworkCore;
using Xunit;

namespace TravelBuddy.Califications
{
    [Collection(TravelBuddyTestConsts.CollectionDefinitionName)] 
    public class EFCoreCalificationAppService_Tests : CalificationAppService_Tests<TravelBuddyEntityFrameworkCoreTestModule>
    {
   
    }
}
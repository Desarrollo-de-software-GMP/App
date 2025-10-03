using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TravelBuddy.EntityFrameworkCore;
using Xunit;
using TravelBuddy.Destinations;

namespace TravelBuddy.Destinations
{
    [Collection(TravelBuddyTestConsts.CollectionDefinitionName)]
    public class EfCoreDestinationAppService_Tests : DestinationAppService_Tests<TravelBuddyEntityFrameworkCoreTestModule>
    {

    }
}
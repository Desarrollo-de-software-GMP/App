using AutoMapper;
using TravelBuddy.Destinations;

namespace TravelBuddy;

public class TravelBuddyApplicationAutoMapperProfile : Profile
{
    public TravelBuddyApplicationAutoMapperProfile()
    {
 
        {

            CreateMap<Destination, DestinationDTO>();
            CreateMap<CreateUpdateDestinationDTO, Destination>();

        }     

    }
}

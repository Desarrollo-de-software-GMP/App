using AutoMapper;
using TravelBuddy.Califications;
using TravelBuddy.Califications.Dtos;
using TravelBuddy.Coordenadas;
using TravelBuddy.Destinations;

namespace TravelBuddy;

public class TravelBuddyApplicationAutoMapperProfile : Profile
{
    public TravelBuddyApplicationAutoMapperProfile()
    {
 
        {

            CreateMap<Destination, DestinationDTO>();
            CreateMap<CreateUpdateDestinationDTO, Destination>();
            CreateMap<Coordinates, CoordinatesDTO>().ReverseMap();

            CreateMap<Calification, CalificationDto>();
            CreateMap<CreateUpdateCalificationDTO, Calification>();

        }     

    }
}

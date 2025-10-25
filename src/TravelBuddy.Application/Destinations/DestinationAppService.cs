using System;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;
using TravelBuddy.Destinations;
using TravelBuddy.Application.Contracts.Destinations;
using System.Threading.Tasks;


namespace TravelBuddy.Application.Destinations
{
    public class DestinationAppService
        : CrudAppService<
            Destination,             // La entidad
            DestinationDTO,          // DTO para mostrar
            Guid,                    // Tipo de la clave primaria
            PagedAndSortedResultRequestDto, // Filtro/paginación
            CreateUpdateDestinationDTO>,    // DTO para crear/editar
          IDestinationAppService          // Interfaz opcional
    {
        private readonly ICitySearchService _citySearchService;
        public DestinationAppService(IRepository<Destination, Guid> repository, ICitySearchService citySearchService)
            : base(repository)
        {
            _citySearchService = citySearchService;

        }
        public async Task<CitySearchResultDto> SearchCitiesAsync(CitySearchRequestDTO request)
        {
            return await _citySearchService.SearchCities(request);
        }
    }
}


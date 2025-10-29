using System;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;
using TravelBuddy.Destinations;
using TravelBuddy.Application.Contracts.Destinations;


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
        public DestinationAppService(IRepository<Destination, Guid> repository)
            : base(repository)
        {

        }
    }
}


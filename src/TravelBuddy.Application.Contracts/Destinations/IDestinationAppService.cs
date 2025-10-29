using System;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace TravelBuddy.Application.Contracts.Destinations
{
    public interface IDestinationAppService
        : ICrudAppService<
            DestinationDTO,              // DTO para mostrar
            Guid,                        // Tipo de la clave primaria
            PagedAndSortedResultRequestDto, // Filtro/paginación
            CreateUpdateDestinationDTO>  // DTO para crear/editar
    {
    }
}
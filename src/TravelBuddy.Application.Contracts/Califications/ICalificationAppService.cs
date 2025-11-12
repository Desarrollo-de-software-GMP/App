using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TravelBuddy.Califications.Dtos; 
using Volo.Abp.Application.Services;

namespace TravelBuddy.Califications
{
    
    public interface ICalificationAppService : IApplicationService
    {
    
        Task<CalificationDTO> CreateAsync(CreateUpdateCalificationDTO input);

        // Método de ejemplo para obtener calificaciones por usuario
        Task<List<CalificationDTO>> ObtenerPorUsuarioAsync(Guid usuarioId);
    }
}
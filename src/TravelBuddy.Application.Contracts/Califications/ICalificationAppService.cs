using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TravelBuddy.Califications.Dtos;
using Volo.Abp.Application.Services;

namespace TravelBuddy.Califications
{
    public interface ICalificationAppService : IApplicationService
    {
        Task<CalificationDto> CreateAsync(CreateUpdateCalificationDTO input);
        Task<List<CalificationDto>> ObtenerPorUsuarioAsync(Guid usuarioId);
    }
}
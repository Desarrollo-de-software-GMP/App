using System;
using System.Threading.Tasks;
using TravelBuddy.Califications.Dtos;
using Volo.Abp.Application.Services;

namespace TravelBuddy.Califications
{
    public interface ICreateUpdateCalificationAppService : IApplicationService
    {
        Task<CalificationDTO> CreateCalificationAsync(CreateUpdateCalificationDTO input);
    }
}
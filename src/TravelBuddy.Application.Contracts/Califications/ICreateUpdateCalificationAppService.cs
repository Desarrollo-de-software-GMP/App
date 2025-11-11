using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TravelBuddy.Califications.Dtos;

namespace TravelBuddy.Califications
{
    public interface ICreateUpdateCalificationAppService
    {
        Task<CalificationDto> CreateCalificationAsync(CreateUpdateCalificationDTO input);
    }
}

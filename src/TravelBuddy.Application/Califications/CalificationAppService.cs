using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using OpenIddict.Validation.AspNetCore;
using TravelBuddy.Califications.Dtos;
using TravelBuddy.Califications;
using Volo.Abp.Application.Services;
using Volo.Abp.Authorization;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Users;

namespace TravelBuddy.Califications
{
    
    [Authorize(AuthenticationSchemes = OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme)]
    public class CalificationAppService : ApplicationService, ICalificationAppService
    {
       
        private readonly ICreateUpdateCalificationAppService _createCalificacionService;
        private readonly IRepository<Calification, Guid> _calificationRepository;
        private readonly ICurrentUser _currentUser;


        public CalificationAppService(
            IRepository<Calification, Guid> calificationRepository,
            ICurrentUser currentUser,
            ICreateUpdateCalificationAppService createCalificacionService)
        {
            _calificationRepository = calificationRepository;
            _currentUser = currentUser;
            _createCalificacionService = createCalificacionService; 
        }

    
        public async Task<CalificationDTO> CreateAsync(CreateUpdateCalificationDTO input)
        {
            return await _createCalificacionService.CreateCalificationAsync(input);
        }

  
        public async Task<List<CalificationDTO>> ObtenerPorUsuarioAsync(Guid usuarioId)
        {
         
            if (!_currentUser.IsAuthenticated)
            {
                throw new AbpAuthorizationException("Debe estar autenticado para ver sus calificaciones.");
            }

           
            if (_currentUser.Id != usuarioId)
            {
                throw new AbpAuthorizationException("No tiene permiso para ver las calificaciones de otro usuario.");
            }

           
            var calificaciones = await _calificationRepository.GetListAsync(c => c.UserId == usuarioId);

            return ObjectMapper.Map<List<Calification>, List<CalificationDTO>>(calificaciones);
        }
    }
}
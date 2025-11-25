using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
//using OpenIddict.Validation.AspNetCore;
using TravelBuddy.Califications.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Authorization;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Users;

namespace TravelBuddy.Califications
{
    
    [Authorize]//(AuthenticationSchemes = OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme)
    public class CalificationAppService : ApplicationService, ICalificationAppService
    {
        // 2. Inyecta el Repositorio para 'Calificacion' y el 'CurrentUser'
        private readonly IRepository<Calification, Guid> _calificacionRepository;
        private readonly ICurrentUser _currentUser;

        public CalificationAppService(
            IRepository<Calification, Guid> calificacionRepository,
            ICurrentUser currentUser)
        {
            _calificacionRepository = calificacionRepository;
            _currentUser = currentUser; // Esto te da el ID del usuario logueado
        }

      
        public async Task<CalificationDto> CreateAsync(CreateUpdateCalificationDTO input)
        {
 
            var calificacion = ObjectMapper.Map<CreateUpdateCalificationDTO, Calification>(input);

      
           
            calificacion.UserId = _currentUser.Id.Value; 

            
            var nuevaCalificacion = await _calificacionRepository.InsertAsync(calificacion);

            
            return ObjectMapper.Map<Calification, CalificationDto>(nuevaCalificacion);
        }


        public async Task<List<CalificationDto>> ObtenerPorUsuarioAsync(Guid usuarioId)
        {
            // 1. Verificar autenticación
            if (!_currentUser.IsAuthenticated)
            {
                throw new AbpAuthorizationException("Debe estar autenticado para ver sus calificaciones.");
            }

            // 2. Validar que solo consulte su propia información
            if (_currentUser.Id != usuarioId)
            {
                throw new AbpAuthorizationException("No tiene permiso para ver las calificaciones de otro usuario.");
            }

           
            var calificaciones = await _calificacionRepository.GetListAsync(c => c.UserId == usuarioId);

            // 4. Mapearlas al DTO
            return ObjectMapper.Map<List<Calification>, List<CalificationDto>>(calificaciones);
        }
    }
}
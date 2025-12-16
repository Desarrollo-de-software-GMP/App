using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using TravelBuddy.Califications.Dtos;
using Volo.Abp;
using Volo.Abp.Application.Services;
using Volo.Abp.Authorization;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Users;

namespace TravelBuddy.Califications
{
    
    [Authorize(AuthenticationSchemes = OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme)]
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
            _currentUser = currentUser;
        }

        public async Task<CalificationDto> CreateAsync(CreateUpdateCalificationDTO input)
        {
            // 1. Verificar que el usuario esté autenticado
            if (!_currentUser.IsAuthenticated)
            {
                throw new AbpAuthorizationException("Debes iniciar sesión para crear una calificación.");
            }

            var userId = _currentUser.Id.Value;

            // 2. REGLA DE NEGOCIO: Evitar duplicados
            // Verifica si ya existe una calificación para este destino hecha por este usuario
            var calificacionExistente = await _calificacionRepository.FirstOrDefaultAsync(
                c => c.DestinationId == input.DestinationId && c.UserId == userId
            );

            if (calificacionExistente != null)
            {
                throw new UserFriendlyException("Ya has calificado este destino.");
            }

            // 3. Mapear y crear la entidad
            var calificacion = ObjectMapper.Map<CreateUpdateCalificationDTO, Calification>(input);
            calificacion.UserId = userId; // Asignar el ID del usuario actual

            var nuevaCalificacion = await _calificacionRepository.InsertAsync(calificacion, autoSave: true);

            return ObjectMapper.Map<Calification, CalificationDto>(nuevaCalificacion);
        }

        public async Task<List<CalificationDto>> ObtenerPorUsuarioAsync(Guid usuarioId)
        {
            if (!_currentUser.IsAuthenticated)
            {
                throw new AbpAuthorizationException("Debe estar autenticado para ver sus calificaciones.");
            }

            // Validar que el usuario solo consulte sus propias calificaciones
            if (_currentUser.Id != usuarioId)
            {
                throw new AbpAuthorizationException("No tiene permiso para ver las calificaciones de otro usuario.");
            }

            var calificaciones = await _calificacionRepository.GetListAsync(c => c.UserId == usuarioId);

            return ObjectMapper.Map<List<Calification>, List<CalificationDto>>(calificaciones);
        }
    }
}
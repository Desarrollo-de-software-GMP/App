using System;
using System.Threading.Tasks;
using TravelBuddy.Califications.Dtos;
using TravelBuddy.Domain; // Para la entidad Calificacion
using Volo.Abp;
using Volo.Abp.Application.Services;
using Volo.Abp.Authorization;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Users;

namespace TravelBuddy.Califications
{
    public class CreateUpdateCalificacionService : ApplicationService, ICreateUpdateCalificationAppService
    {
        private readonly IRepository<Calification, Guid> _calificacionRepository;
        private readonly ICurrentUser _currentUser;

        public CreateUpdateCalificacionService(
            IRepository<Calification, Guid> calificacionRepository,
            ICurrentUser currentUser)
        {
            _calificacionRepository = calificacionRepository;
            _currentUser = currentUser;
        }

        public async Task<CalificationDto> CreateCalificationAsync(CreateUpdateCalificationDTO input)
        {
            if (!_currentUser.IsAuthenticated)
                throw new AbpAuthorizationException("Debes iniciar sesión para crear una calificación.");

            var userId = _currentUser.Id ?? throw new AbpAuthorizationException("No se pudo obtener el usuario.");

            // REGLA DE NEGOCIO: Evitar duplicados
            // Esto usa la propiedad DestinoId que acabas de agregar al DTO
            var calificacionExistente = await _calificacionRepository.FirstOrDefaultAsync(
                c => c.DestinationId == input.DestinationId && c.UserId == userId
            );

            if (calificacionExistente != null)
                throw new UserFriendlyException("Ya has calificado este destino.");

            // Mapea el DTO a la Entidad
            var calificacion = ObjectMapper.Map<CreateUpdateCalificationDTO, Calification>(input);

            // Asigna el UserId desde el token
            calificacion.UserId = userId;

            // Guarda en la BD
            var nuevaCalificacion = await _calificacionRepository.InsertAsync(calificacion, autoSave: true);

            // Mapea la Entidad al DTO de respuesta
            return ObjectMapper.Map<Calification, CalificationDto>(nuevaCalificacion);
        }
    }
}
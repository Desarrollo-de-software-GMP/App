using System;
using System.Threading.Tasks;
using TravelBuddy.Califications.Dtos;
using TravelBuddy.Califications; 
using Volo.Abp;
using Volo.Abp.Authorization;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Users;

namespace TravelBuddy.Califications
{

    public class CreateUpdateCalificationService : ICreateUpdateCalificationAppService
    {
        private readonly IRepository<Calification, Guid> _calificationRepository;
        private readonly ICurrentUser _currentUser;

        // Constructor simple
        public CreateUpdateCalificationService(
            IRepository<Calification, Guid> calificationRepository,
            ICurrentUser currentUser)
        {
            _calificationRepository = calificationRepository;
            _currentUser = currentUser;
        }

        public async Task<CalificationDTO> CreateCalificationAsync(CreateUpdateCalificationDTO input)
        {
            if (!_currentUser.IsAuthenticated)
                throw new AbpAuthorizationException("Debes iniciar sesión para crear una calificación.");

            var userId = _currentUser.Id ?? throw new AbpAuthorizationException("No se pudo obtener el usuario.");

            // REGLA DE NEGOCIO: Evitar duplicados
            var calificacionExistente = await _calificationRepository.FirstOrDefaultAsync(
                c => c.DestinationId == input.DestinationId && c.UserId == userId
            );

            if (calificacionExistente != null)
                throw new UserFriendlyException("Ya has calificado este destino.");

        
            var calificacion = new Calification()
            {
                punctuation = input.punctuation,
                comment = input.comment,
                DestinationId = input.DestinationId,
                UserId = userId
                
            };

            // Guardar en la BD
            await _calificationRepository.InsertAsync(calificacion, autoSave: true);

            // Mapeo MANUAL al DTO de respuesta 
            return new CalificationDTO
            {
                Id = calificacion.Id,
                UserId = calificacion.UserId,
                DestinationId = calificacion.DestinationId,
                punctuation = calificacion.punctuation,
                comment = calificacion.comment,
                CreationTime = calificacion.CreationTime 
            };
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using OpenIddict.Validation.AspNetCore;
using TravelBuddy.Califications.Dtos;
using TravelBuddy.Califications; // Para la entidad Calification
using Volo.Abp.Application.Services;
using Volo.Abp.Authorization;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Users;

namespace TravelBuddy.Califications
{
    // Esta es la "Puerta de API". Está autorizada.
    [Authorize(AuthenticationSchemes = OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme)]
    public class CalificationAppService : ApplicationService, ICalificationAppService
    {
        // 1. El servicio de Lógica/"Fábrica" (adaptado de _crearOpinionService)
        private readonly ICreateUpdateCalificationAppService _createCalificacionService;

        // 2. Repositorio y Usuario (adaptado de _opinionRepository y _currentUser)
        private readonly IRepository<Calification, Guid> _calificationRepository;
        private readonly ICurrentUser _currentUser;

        // 3. Constructor actualizado para inyectar TODOS los servicios
        public CalificationAppService(
            IRepository<Calification, Guid> calificationRepository,
            ICurrentUser currentUser,
            ICreateUpdateCalificationAppService createCalificacionService) // <-- Inyecta la "fábrica"
        {
            _calificationRepository = calificationRepository;
            _currentUser = currentUser;
            _createCalificacionService = createCalificacionService; // <-- Lo asigna
        }

        // 4. Este método DELEGA el trabajo al servicio de lógica
        public async Task<CalificationDTO> CreateAsync(CreateUpdateCalificationDTO input)
        {
            return await _createCalificacionService.CreateCalificationAsync(input);
        }

        // 5. Este método implementa la lógica de lectura, tal como en el ejemplo
        public async Task<List<CalificationDTO>> ObtenerPorUsuarioAsync(Guid usuarioId)
        {
            // 1️⃣ Verificar autenticación
            if (!_currentUser.IsAuthenticated)
            {
                throw new AbpAuthorizationException("Debe estar autenticado para ver sus calificaciones.");
            }

            // 2️⃣ Validar que solo consulte su propia información
            if (_currentUser.Id != usuarioId)
            {
                throw new AbpAuthorizationException("No tiene permiso para ver las calificaciones de otro usuario.");
            }

            // 3️⃣ Obtener calificaciones filtradas por usuario
            var calificaciones = await _calificationRepository.GetListAsync(c => c.UserId == usuarioId);

            // 4️⃣ Mapearlas al DTO (Usamos AutoMapper, que viene con ApplicationService)
            return ObjectMapper.Map<List<Calification>, List<CalificationDTO>>(calificaciones);
        }
    }
}
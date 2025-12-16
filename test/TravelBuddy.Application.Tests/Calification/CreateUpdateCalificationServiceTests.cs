using System;
using System.Threading.Tasks;
using NSubstitute; // Para manipular el Mock del usuario
using Shouldly;    // Para las aserciones (ShouldBe, ShouldNotBeNull)
using TravelBuddy.Califications;
using TravelBuddy.Califications.Dtos;
using Volo.Abp;
using Volo.Abp.Authorization;
using Volo.Abp.Modularity;
using Volo.Abp.Users;
using Xunit;

namespace TravelBuddy.Califications
{
    // Heredamos de TravelBuddyApplicationTestBase para tener el entorno de base de datos en memoria
    public class CalificationAppServiceTests : TravelBuddyApplicationTestBase<TravelBuddyApplicationModule>
    {
        private readonly ICalificationAppService _calificationAppService;
        private readonly ICurrentUser _currentUser;

        public CalificationAppServiceTests()
        {
            // Obtenemos los servicios del contenedor de dependencias de prueba
            _calificationAppService = GetRequiredService<ICalificationAppService>();
            _currentUser = GetRequiredService<ICurrentUser>();
        }

        [Fact]
        public async Task CreateAsync_DebeRetornarCalificationDto()
        {
            // Arrange
            var input = new CreateUpdateCalificationDTO
            {
                DestinationId = Guid.NewGuid(),
                punctuation = 5, // En tu caso es int, no Enum
                comment = "Excelente destino turístico!"
            };

            // Act
            var result = await _calificationAppService.CreateAsync(input);

            // Assert
            result.ShouldNotBeNull();
            result.Id.ShouldNotBe(Guid.Empty);
            result.comment.ShouldBe(input.comment);
        }

        [Fact]
        public async Task CreateAsync_NoDebePermitirDuplicados()
        {
            // Arrange
            var destinoId = Guid.NewGuid();

            var input = new CreateUpdateCalificationDTO
            {
                DestinationId = destinoId,
                punctuation = 4,
                comment = "Muy lindo lugar"
            };

            // Act - Creamos la primera vez
            await _calificationAppService.CreateAsync(input);

            // Assert - Intentamos crear la segunda vez y esperamos la excepción
            var ex = await Assert.ThrowsAsync<UserFriendlyException>(() => _calificationAppService.CreateAsync(input));

            ex.Message.ShouldBe("Ya has calificado este destino.");
        }

        [Fact]
        public async Task Debe_RespetarFiltroPorUsuario_Y_RequerirAutenticacion()
        {
            // --- PARTE 1: Verificar flujo normal ---

            // Requisito 1: Requerir Autenticación (se verifica al inicio que el mock esté autenticado por defecto)
            _currentUser.IsAuthenticated.Returns(true);
            _currentUser.Id.Returns(Guid.NewGuid()); // Simulamos un usuario válido

            var input = new CreateUpdateCalificationDTO
            {
                DestinationId = Guid.NewGuid(),
                punctuation = 3,
                comment = "Correcto."
            };

            // Creamos una calificación con el usuario actual
            var calificacionCreada = await _calificationAppService.CreateAsync(input);

            // Requisito 2: Respetar Filtro por Usuario (El usuario solo ve su propia opinión)
            var currentUserId = _currentUser.Id.Value;
            var calificacionesUsuario = await _calificationAppService.ObtenerPorUsuarioAsync(currentUserId);

            calificacionesUsuario.ShouldContain(o => o.Id == calificacionCreada.Id);


            // --- PARTE 2: Verificar seguridad (Intentar ver datos de otro) ---

            // Intentamos ver las calificaciones de un ID diferente al nuestro
            await Should.ThrowAsync<AbpAuthorizationException>(
                async () => await _calificationAppService.ObtenerPorUsuarioAsync(Guid.NewGuid())
            );
        }

        [Fact]
        public async Task CreateAsync_DebeFallarCon401SiNoSeProveeToken()
        {
            // Arrange - Simular un contexto sin autenticación
            // NSubstitute nos permite cambiar el comportamiento del mock en caliente
            _currentUser.IsAuthenticated.Returns(false);
            _currentUser.Id.Returns((Guid?)null);

            var input = new CreateUpdateCalificationDTO
            {
                DestinationId = Guid.NewGuid(),
                punctuation = 2,
                comment = "No me gustó mucho."
            };

            // Act & Assert
            // Verificar que al intentar crear una opinión sin autenticación, se lance la excepción de autorización.
            // Nota: En tests de integración, 401 se manifiesta como AbpAuthorizationException.
            await Should.ThrowAsync<AbpAuthorizationException>(
                async () => await _calificationAppService.CreateAsync(input)
            );
        }
    }
}
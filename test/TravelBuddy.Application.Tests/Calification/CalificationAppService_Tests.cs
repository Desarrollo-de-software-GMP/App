using Shouldly;
using System;
using System.Linq;
using System.Threading.Tasks;
using TravelBuddy.Califications.Dtos;
using TravelBuddy.Coordenadas;
using TravelBuddy.Coordenadas.TravelBuddy.Coordenadas;
using TravelBuddy.Destinations;
using Volo.Abp;
using Volo.Abp.Authorization;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Modularity;
using Volo.Abp.Security.Claims;
using Volo.Abp.Users;
using Xunit;

namespace TravelBuddy.Califications
{
    public class CalificationAppService_Tests : TravelBuddyApplicationTestBase<TravelBuddyApplicationTestModule>
    {
        private readonly ICalificationAppService _calificationService;
        private readonly IRepository<Destination, Guid> _destinationRepository;
        private readonly ICurrentPrincipalAccessor _currentPrincipalAccessor;
        private readonly ICurrentUser _currentUser;

        public CalificationAppService_Tests()
        {
            _calificationService = GetRequiredService<ICalificationAppService>();
            _destinationRepository = GetRequiredService<IRepository<Destination, Guid>>();
            _currentPrincipalAccessor = GetRequiredService<ICurrentPrincipalAccessor>();
            _currentUser = GetRequiredService<ICurrentUser>();
        }

        [Fact]
        public async Task CrearCalificacionAsync_DebeRetornarCalificationDto()
        {
            // Arrange
            var destination = await CreateDestinationAsync();
            var input = new CreateUpdateCalificationDTO
            {
                DestinationId = destination.Id,
                punctuation = 5,
                comment = "Excelente destino"
            };

            // Act
            var result = await _calificationService.CreateAsync(input);

            // Assert
            result.ShouldNotBeNull();
            result.Id.ShouldNotBe(Guid.Empty);
            result.DestinoId.ShouldBe(input.DestinationId);
            result.puntuation.ShouldBe(input.punctuation);
            result.puntuation.ShouldBeInRange(1, 5);
            result.comment.ShouldBe(input.comment);
        }

        [Fact]
        public async Task CrearCalificacionAsync_NoDebePermitirDuplicados()
        {
            // Arrange
            var destination = await CreateDestinationAsync();
            var input = new CreateUpdateCalificationDTO
            {
                DestinationId = destination.Id,
                punctuation = 4,
                comment = "lindo lugar"
            };

            // Creamos la primera opinión
            await _calificationService.CreateAsync(input);

            // Act & Assert
            // Intentamos crear la segunda (debe fallar)
            var ex = await Assert.ThrowsAsync<UserFriendlyException>(async () =>
                await _calificationService.CreateAsync(input)
            );

            ex.Message.ShouldBe("Ya has calificado este destino.");
        }

        [Fact]
        public async Task Debe_RespetarFiltroPorUsuario_Y_RequerirAutenticacion()
        {
            // Requisito 1: Requerir Autenticación (se verifica al inicio, por defecto somos admin)
            _currentUser.IsAuthenticated.ShouldBeTrue();

            var destination = await CreateDestinationAsync();
            var input = new CreateUpdateCalificationDTO
            {
                DestinationId = destination.Id,
                punctuation = 3,
                comment = "Bien."
            };

            var opinion = await _calificationService.CreateAsync(input);

            // Requisito 2: Respetar Filtro por Usuario (El usuario solo ve su propia opinión)
            var currentUserId = _currentUser.Id.Value;
            var calificacionesUsuario = await _calificationService.ObtenerPorUsuarioAsync(currentUserId);

            calificacionesUsuario.ShouldContain(o => o.Id == opinion.Id);

            // 🔸 Simular un contexto sin autenticación (Logout)
            using (_currentPrincipalAccessor.Change(null))
            {
                _currentUser.IsAuthenticated.ShouldBeFalse();

                // Verificar que al intentar obtener datos sin autenticación, falla
                await Assert.ThrowsAsync<AbpAuthorizationException>(async () =>
                    await _calificationService.ObtenerPorUsuarioAsync(Guid.NewGuid())
                );
            }
        }

        [Fact]
        public async Task CrearCalificacionAsync_DebeFallarCon401SiNoSeProveeToken()
        {
            var destination = await CreateDestinationAsync();
            var input = new CreateUpdateCalificationDTO
            {
                DestinationId = destination.Id,
                punctuation = 2,
                comment = "mmm."
            };

            // Simular contexto sin autenticación
            using (_currentPrincipalAccessor.Change(null))
            {
                _currentUser.IsAuthenticated.ShouldBeFalse();

                // Debe fallar con excepción de autorización
                await Assert.ThrowsAsync<AbpAuthorizationException>(async () =>
                    await _calificationService.CreateAsync(input)
                );
            }
        }

        // Helper privado para crear datos necesarios
        private async Task<Destination> CreateDestinationAsync()
        {
            var destination = new Destination(
                Guid.NewGuid(),
                "Destino Test",
                "Pais Test",
                1000,
                new Coordinates(10.5f, 20.5f),
                "http://photo.url",
                DateTime.Now
            );
            return await _destinationRepository.InsertAsync(destination);
        }
    }
}
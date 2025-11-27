using NSubstitute;
using Shouldly;
using System;
using System.Threading.Tasks;
using TravelBuddy.Califications.Dtos;
using TravelBuddy.Coordenadas;
using TravelBuddy.Coordenadas.TravelBuddy.Coordenadas;
using TravelBuddy.Destinations;
using Volo.Abp;
using Volo.Abp.Authorization;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Modularity;
using Volo.Abp.Users;
using Xunit;

namespace TravelBuddy.Califications
{
    
    [Collection(TravelBuddyTestConsts.CollectionDefinitionName)]
    public abstract class CalificationAppService_Tests<TStartupModule> : TravelBuddyApplicationTestBase<TStartupModule>
        where TStartupModule : IAbpModule
    {
        private readonly ICalificationAppService _calificationService;
        private readonly IRepository<Destination, Guid> _destinationRepository;

        protected CalificationAppService_Tests()
        {
            _calificationService = GetRequiredService<ICalificationAppService>();
            _destinationRepository = GetRequiredService<IRepository<Destination, Guid>>();
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
            result.DestinationId.ShouldBe(input.DestinationId);
            result.punctuation.ShouldBe(input.punctuation);
            result.punctuation.ShouldBeInRange(1, 5);
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
                comment = "Lindo lugar"
            };

            // Creamos la primera vez
            await _calificationService.CreateAsync(input);

            // Act & Assert
            // Intentamos crear la segunda vez (debe fallar)
            var ex = await Assert.ThrowsAsync<UserFriendlyException>(async () =>
                await _calificationService.CreateAsync(input)
            );

            ex.Message.ShouldBe("Ya has calificado este destino.");
        }

        [Fact]
        public async Task Debe_RespetarFiltroPorUsuario_Y_RequerirAutenticacion()
        {
            // 1. Obtener el Mock de Usuario del contenedor
            var currentUser = GetRequiredService<ICurrentUser>();

            // Verificar autenticación inicial (dada por el Mock en TestBaseModule)
            currentUser.IsAuthenticated.ShouldBeTrue();

            var destination = await CreateDestinationAsync();
            var input = new CreateUpdateCalificationDTO
            {
                DestinationId = destination.Id,
                punctuation = 3,
                comment = "Bueno."
            };

            var calification = await _calificationService.CreateAsync(input);

            // 2. Verificar filtro por usuario
            // Solo debe devolver las calificaciones creadas por ESTE usuario mockeado
            var currentUserId = currentUser.Id.Value;
            var calificacionesUsuario = await _calificationService.ObtenerPorUsuarioAsync(currentUserId);

            calificacionesUsuario.ShouldContain(o => o.Id == calification.Id);

            // 3. Simular contexto SIN autenticación
            // Al usar NSubstitute, cambiamos el comportamiento del mock dinámicamente
            currentUser.IsAuthenticated.Returns(false);
            currentUser.Id.Returns((Guid?)null);

            // Verificar que falla si no hay usuario autenticado
            await Should.ThrowAsync<AbpAuthorizationException>(async () =>
                await _calificationService.ObtenerPorUsuarioAsync(Guid.NewGuid())
            );

            // Restaurar estado del Mock (Importante para otros tests en la misma colección)
            currentUser.IsAuthenticated.Returns(true);
            currentUser.Id.Returns(Guid.NewGuid());
        }

        [Fact]
        public async Task CrearCalificacionAsync_DebeFallarCon401SiNoSeProveeToken()
        {
            var destination = await CreateDestinationAsync();
            var input = new CreateUpdateCalificationDTO
            {
                DestinationId = destination.Id,
                punctuation = 2,
                comment = "No me gustó."
            };

            // Simular Logout
            var currentUser = GetRequiredService<ICurrentUser>();
            currentUser.IsAuthenticated.Returns(false);
            currentUser.Id.Returns((Guid?)null);

            // Debe lanzar excepción de autorización
            await Should.ThrowAsync<AbpAuthorizationException>(async () =>
                await _calificationService.CreateAsync(input)
            );

            // Restaurar estado
            currentUser.IsAuthenticated.Returns(true);
            currentUser.Id.Returns(Guid.NewGuid());
        }

        // Helper para crear datos de prueba en la BD en memoria
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
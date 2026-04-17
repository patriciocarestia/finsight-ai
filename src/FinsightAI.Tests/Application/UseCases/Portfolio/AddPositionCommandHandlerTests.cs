using FinsightAI.Application.Interfaces;
using FinsightAI.Application.UseCases.Portfolio.Commands.AddPosition;
using FinsightAI.Domain.Entities;
using Moq;

namespace FinsightAI.Tests.Application.UseCases.Portfolio;

public class AddPositionCommandHandlerTests
{
    public class The_Constructor : AddPositionCommandHandlerTests
    {
        [Fact]
        public void Should_throw_ArgumentNullException_when_repository_is_null()
        {
            Assert.Throws<ArgumentNullException>(() =>
                new AddPositionCommandHandler(null!));
        }
    }

    public class The_Method_Handle : AddPositionCommandHandlerTests
    {
        [Fact]
        public async Task Should_return_PositionResponse_with_correct_values()
        {
            // Arrange
            var command = new AddPositionCommand
            {
                UserId = 1,
                AssetType = "USD_BLUE",
                Amount = 500m,
                PurchasePrice = 1250m,
                PurchaseDate = new DateTime(2025, 3, 15)
            };

            var repository = Mock.Of<IPositionRepository>();
            Mock.Get(repository)
                .Setup(r => r.AddAsync(It.IsAny<Position>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((Position p, CancellationToken _) =>
                {
                    p.Id = 42;
                    p.CreatedAt = DateTime.UtcNow;
                    return p;
                });

            var sut = new AddPositionCommandHandler(repository);

            // Act
            var result = await sut.Handle(command, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(42, result.Id);
            Assert.Equal("USD_BLUE", result.AssetType);
            Assert.Equal(500m, result.Amount);
            Assert.Equal(1250m, result.PurchasePrice);
        }

        [Fact]
        public async Task Should_call_repository_AddAsync_once()
        {
            // Arrange
            var command = new AddPositionCommand
            {
                UserId = 1,
                AssetType = "BTC",
                Amount = 0.01m,
                PurchasePrice = 65000m,
                PurchaseDate = DateTime.UtcNow
            };

            var repository = Mock.Of<IPositionRepository>();
            Mock.Get(repository)
                .Setup(r => r.AddAsync(It.IsAny<Position>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((Position p, CancellationToken _) => p);

            var sut = new AddPositionCommandHandler(repository);

            // Act
            await sut.Handle(command, CancellationToken.None);

            // Assert
            Mock.Get(repository).Verify(r => r.AddAsync(
                It.Is<Position>(p => p.UserId == 1 && p.AssetType == "BTC"),
                It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public async Task Should_set_UserId_from_command()
        {
            // Arrange
            var command = new AddPositionCommand
            {
                UserId = 99,
                AssetType = "USDT",
                Amount = 1000m,
                PurchasePrice = 1m,
                PurchaseDate = DateTime.UtcNow
            };

            Position? savedPosition = null;
            var repository = Mock.Of<IPositionRepository>();
            Mock.Get(repository)
                .Setup(r => r.AddAsync(It.IsAny<Position>(), It.IsAny<CancellationToken>()))
                .Callback<Position, CancellationToken>((p, _) => savedPosition = p)
                .ReturnsAsync((Position p, CancellationToken _) => p);

            var sut = new AddPositionCommandHandler(repository);

            // Act
            await sut.Handle(command, CancellationToken.None);

            // Assert
            Assert.NotNull(savedPosition);
            Assert.Equal(99, savedPosition.UserId);
        }
    }
}

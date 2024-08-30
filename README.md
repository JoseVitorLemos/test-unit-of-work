using Gym.Data.DatabaseContext;
using Microsoft.EntityFrameworkCore.Storage;
using Moq;
using Xunit;
using Gym.Data.UnitOfWork;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace Gym.Tests.Gym.Data.UnitOfWorkTests
{
    public class UnitOfWorkTests
    {
        private readonly Mock<DataContext> _mockContext;
        private readonly Mock<IDbContextTransaction> _mockTransaction;
        private readonly Mock<DatabaseFacade> _mockDatabaseFacade;
        private readonly UnitOfWork _unitOfWork;

        public UnitOfWorkTests()
        {
            _mockContext = new Mock<DataContext>(new DbContextOptions<DataContext>());

            _mockTransaction = new Mock<IDbContextTransaction>();

            _mockDatabaseFacade = new Mock<DatabaseFacade>(_mockContext.Object);

            _mockDatabaseFacade
                .Setup(d => d.BeginTransactionAsync(default))
                .ReturnsAsync(_mockTransaction.Object);

            _mockContext.Setup(c => c.Database).Returns(_mockDatabaseFacade.Object);

            _unitOfWork = new UnitOfWork(_mockContext.Object);
        }

        [Fact]
        public async Task BeginTransactionAsync_Should_BeginTransaction()
        {
            await _unitOfWork.BeginTransactionAsync();

            _mockDatabaseFacade.Verify(d => d.BeginTransactionAsync(default), Times.Once);
        }

        [Fact]
        public async Task CommitAsync_Should_CommitTransaction()
        {
            // Arrange
            await _unitOfWork.BeginTransactionAsync();

            // Act
            await _unitOfWork.CommitAsync();

            // Assert
            _mockTransaction.Verify(t => t.CommitAsync(default), Times.Once);
        }

        [Fact]
        public async Task RollbackAsync_Should_RollbackTransaction()
        {
            await _unitOfWork.BeginTransactionAsync();

            await _unitOfWork.RollbackAsync();

            _mockTransaction.Verify(t => t.RollbackAsync(default), Times.Once);
        }

        [Fact]
        public async Task DisposeAsync_Should_DisposeContext()
        {
            // Act
            await _unitOfWork.DisposeAsync();

            // Assert
            _mockContext.Verify(c => c.DisposeAsync(), Times.Once);
        }
    }
}

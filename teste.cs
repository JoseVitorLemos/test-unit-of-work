using Gym.Data.Repositories;
using Gym.Data.DatabaseContext;
using Microsoft.EntityFrameworkCore;
using Gym.Domain.Entities;

namespace Gym.Tests.Gym.Data.RepositoryTest;

public class RepositoryTests : IDisposable
{
    private readonly DataContext _context;
    private readonly Repository<TiposCalculo> _repository;
    private readonly List<TiposCalculo> _tiposCalculos;

    public RepositoryTests()
    {
        // Configura o DbContext para usar o banco de dados In-Memory
        var options = new DbContextOptionsBuilder<DataContext>()
            .UseInMemoryDatabase(databaseName: "TestDatabase")
            .Options;

        _context = new DataContext(options);
        _repository = new Repository<TiposCalculo>(_context);

        _tiposCalculos = new List<TiposCalculo>
            {
                new () { CodigoTipoCalculo = 1, NomeTipoCalculo = "any" },
                new () { CodigoTipoCalculo = 2, NomeTipoCalculo = "any" }
            };
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
    }

    [Fact]
    public async Task AddAsync_ShouldAddEntity()
    {
        // Arrange
        var entity = _tiposCalculos.First();

        // Act
        await _repository.AddAsync(entity);

        // Assert
        var result = await _repository.GetAllAsync();
        Assert.Single(result);
        Assert.Equal(entity.CodigoTipoCalculo, result.First().CodigoTipoCalculo);
        Assert.Equal(entity.NomeTipoCalculo, result.First().NomeTipoCalculo);
        Assert.Equal(entity.Ativo, result.First().Ativo);
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnAllEntities()
    {
        _context.Set<TiposCalculo>().AddRange(_tiposCalculos);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetAllAsync();

        // Assert
        Assert.Equal(2, result.Count());
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnEntity()
    {
        // Arrange
        var entity = _tiposCalculos.First();
        await _context.Set<TiposCalculo>().AddAsync(entity);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetByIdAsync(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(entity.CodigoTipoCalculo, result.CodigoTipoCalculo);
        Assert.Equal(entity.NomeTipoCalculo, result.NomeTipoCalculo);
        Assert.Equal(entity.Ativo, result.Ativo);
    }

    [Fact]
    public async Task DeleteAsync_ShouldRemoveEntity()
    {
        // Arrange
        var entity = _tiposCalculos.First();
        await _context.Set<TiposCalculo>().AddAsync(entity);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.DeleteAsync(entity);

        // Assert
        Assert.True(result);
        Assert.Empty(await _repository.GetAllAsync());
    }
}

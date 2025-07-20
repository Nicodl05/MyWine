using WineCellar.Core.Entities;
using WineCellar.Infrastructure.Repositories;

namespace WineCellar.Tests;

public class InMemoryWineRepositoryTests
{
    private readonly InMemoryWineRepository _repository;

    public InMemoryWineRepositoryTests()
    {
        _repository = new InMemoryWineRepository();
    }

    [Fact]
    public async Task CreateAsync_ShouldCreateWineWithId()
    {

        var wine = new Wine
        {
            Name = "Test Wine",
            Producer = "Test Producer",
            Year = 2020,
            Region = "Test Region",
            Type = "Rouge",
            EstimatedPrice = 25.50m,
            Quantity = 2
        };


        var result = await _repository.CreateAsync(wine);


        Assert.NotEqual(Guid.Empty, result.Id);
        Assert.Equal("Test Wine", result.Name);
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnAllWines()
    {

        var wine1 = new Wine { Name = "Wine 1", Producer = "Producer 1", Year = 2020 };
        var wine2 = new Wine { Name = "Wine 2", Producer = "Producer 2", Year = 2021 };

        await _repository.CreateAsync(wine1);
        await _repository.CreateAsync(wine2);


        var result = await _repository.GetAllAsync();


        Assert.Equal(2, result.Count());
    }

    [Fact]
    public async Task GetByIdAsync_WithValidId_ShouldReturnWine()
    {

        var wine = new Wine { Name = "Test Wine", Producer = "Test Producer", Year = 2020 };
        var created = await _repository.CreateAsync(wine);


        var result = await _repository.GetByIdAsync(created.Id);


        Assert.NotNull(result);
        Assert.Equal("Test Wine", result.Name);
    }

    [Fact]
    public async Task GetByIdAsync_WithInvalidId_ShouldReturnNull()
    {

        var result = await _repository.GetByIdAsync(Guid.NewGuid());


        Assert.Null(result);
    }

    [Fact]
    public async Task DeleteAsync_ShouldRemoveWine()
    {

        var wine = new Wine { Name = "Test Wine", Producer = "Test Producer", Year = 2020 };
        var created = await _repository.CreateAsync(wine);


        await _repository.DeleteAsync(created.Id);
        var result = await _repository.GetByIdAsync(created.Id);


        Assert.Null(result);
    }

    [Fact]
    public async Task GetTotalValueAsync_ShouldCalculateCorrectTotal()
    {

        var wine1 = new Wine { EstimatedPrice = 20m, Quantity = 2 }; // 40
        var wine2 = new Wine { EstimatedPrice = 30m, Quantity = 1 }; // 30

        await _repository.CreateAsync(wine1);
        await _repository.CreateAsync(wine2);


        var result = await _repository.GetTotalValueAsync();


        Assert.Equal(70m, result);
    }
}
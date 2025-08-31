using NUnit.Framework;
using WineCellar.Core.Entities;
using WineCellar.Infrastructure.Repositories;

namespace WineCellar.Tests.Unit.Repositories;

public class InMemoryWineRepositoryTests
{
    private InMemoryWineRepository _repository;

    [SetUp]
    public void SetUp()
    {
        _repository = new InMemoryWineRepository();
    }

    [Test]
    public async Task CreateAsync_ShouldCreateWineWithId()
    {
        // Arrange
        var wine = new Wine
        {
            Name = "Test Wine",
            Producer = "Test Producer",
            Year = 2020,
            Region = "Test Region",
            Type = "Red",
            EstimatedPrice = 25.50m,
            Quantity = 2
        };

        // Act
        var result = await _repository.CreateAsync(wine);

        // Assert
        Assert.That(result.Id, Is.Not.EqualTo(Guid.Empty));
        Assert.That(result.Name, Is.EqualTo("Test Wine"));
        Assert.That(result.Producer, Is.EqualTo("Test Producer"));
        Assert.That(result.Year, Is.EqualTo(2020));
        Assert.That(result.EstimatedPrice, Is.EqualTo(25.50m));
    }

    [Test]
    public async Task CreateAsync_WithNotes_ShouldCreateWineWithNotes()
    {
        // Arrange
        var wine = new Wine
        {
            Name = "Wine with Notes",
            Producer = "Producer",
            Notes = new List<Note>
            {
                new() { Reviewer = "John Doe", Score = 85 }
            }
        };

        // Act
        var result = await _repository.CreateAsync(wine);

        // Assert
        Assert.That(result.Id, Is.Not.EqualTo(Guid.Empty));
        Assert.That(result.Notes, Has.Count.EqualTo(1));
        Assert.That(result.Notes.First().Reviewer, Is.EqualTo("John Doe"));
        Assert.That(result.Notes.First().Score, Is.EqualTo(85));
    }

    [Test]
    public async Task GetAllAsync_ShouldReturnAllWines()
    {
        // Arrange
        var wine1 = new Wine { Name = "Wine 1", Producer = "Producer 1", Year = 2020 };
        var wine2 = new Wine { Name = "Wine 2", Producer = "Producer 2", Year = 2021 };

        await _repository.CreateAsync(wine1);
        await _repository.CreateAsync(wine2);

        // Act
        var result = await _repository.GetAllAsync();

        // Assert
        Assert.That(result.Count(), Is.EqualTo(2));
        Assert.That(result, Has.Some.Matches<Wine>(w => w.Name == "Wine 1"));
        Assert.That(result, Has.Some.Matches<Wine>(w => w.Name == "Wine 2"));
    }

    [Test]
    public async Task GetAllAsync_WithEmptyRepository_ShouldReturnEmptyList()
    {
        // Act
        var result = await _repository.GetAllAsync();

        // Assert
        Assert.That(result, Is.Empty);
    }

    [Test]
    public async Task GetByIdAsync_WithValidId_ShouldReturnWine()
    {
        // Arrange
        var wine = new Wine { Name = "Test Wine", Producer = "Test Producer", Year = 2020 };
        var created = await _repository.CreateAsync(wine);

        // Act
        var result = await _repository.GetByIdAsync(created.Id);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Name, Is.EqualTo("Test Wine"));
        Assert.That(result.Id, Is.EqualTo(created.Id));
    }

    [Test]
    public async Task GetByIdAsync_WithInvalidId_ShouldReturnNull()
    {
        // Act
        var result = await _repository.GetByIdAsync(Guid.NewGuid());

        // Assert
        Assert.That(result, Is.Null);
    }

    [Test]
    public async Task UpdateAsync_WithExistingWine_ShouldUpdateAllProperties()
    {
        // Arrange
        var originalWine = new Wine
        {
            Name = "Original Wine",
            Producer = "Original Producer",
            Year = 2020,
            Region = "Original Region",
            Type = "Red",
            EstimatedPrice = 20.00m,
            Quantity = 1
        };
        var created = await _repository.CreateAsync(originalWine);

        var updatedWine = new Wine
        {
            Id = created.Id,
            Name = "Updated Wine",
            Producer = "Updated Producer",
            Year = 2021,
            Region = "Updated Region",
            Type = "White",
            EstimatedPrice = 30.00m,
            Quantity = 2,
            Notes = new List<Note> { new() { Reviewer = "Tester", Score = 90 } }
        };

        // Act
        var result = await _repository.UpdateAsync(updatedWine);

        // Assert
        Assert.That(result.Name, Is.EqualTo("Updated Wine"));
        Assert.That(result.Producer, Is.EqualTo("Updated Producer"));
        Assert.That(result.Year, Is.EqualTo(2021));
        Assert.That(result.Region, Is.EqualTo("Updated Region"));
        Assert.That(result.Type, Is.EqualTo("White"));
        Assert.That(result.EstimatedPrice, Is.EqualTo(30.00m));
        Assert.That(result.Quantity, Is.EqualTo(2));
        Assert.That(result.Notes, Has.Count.EqualTo(1));
        Assert.That(result.Notes.First().Reviewer, Is.EqualTo("Tester"));
    }

    [Test]
    public async Task UpdateAsync_WithNonExistentWine_ShouldThrowException()
    {
        // Arrange
        var nonExistentWine = new Wine
        {
            Id = Guid.NewGuid(),
            Name = "Non-existent Wine"
        };

        // Act & Assert
        Assert.That(async () => await _repository.UpdateAsync(nonExistentWine),
            Throws.TypeOf<InvalidOperationException>());
    }

    [Test]
    public async Task DeleteAsync_ShouldRemoveWine()
    {
        // Arrange
        var wine = new Wine { Name = "Test Wine", Producer = "Test Producer", Year = 2020 };
        var created = await _repository.CreateAsync(wine);

        // Act
        await _repository.DeleteAsync(created.Id);
        var result = await _repository.GetByIdAsync(created.Id);

        // Assert
        Assert.That(result, Is.Null);
    }

    [Test]
    public async Task DeleteAsync_WithNonExistentId_ShouldNotThrow()
    {
        // Arrange
        var nonExistentId = Guid.NewGuid();

        // Act & Assert
        await _repository.DeleteAsync(nonExistentId); // Should not throw
    }

    [Test]
    public async Task GetTotalValueAsync_ShouldCalculateCorrectTotal()
    {
        // Arrange
        var wine1 = new Wine { EstimatedPrice = 20m, Quantity = 2 }; // 40
        var wine2 = new Wine { EstimatedPrice = 30m, Quantity = 1 }; // 30
        var wine3 = new Wine { EstimatedPrice = 15m, Quantity = 3 }; // 45

        await _repository.CreateAsync(wine1);
        await _repository.CreateAsync(wine2);
        await _repository.CreateAsync(wine3);

        // Act
        var result = await _repository.GetTotalValueAsync();

        // Assert
        Assert.That(result, Is.EqualTo(115m));
    }

    [Test]
    public async Task GetTotalValueAsync_WithEmptyRepository_ShouldReturnZero()
    {
        // Act
        var result = await _repository.GetTotalValueAsync();

        // Assert
        Assert.That(result, Is.EqualTo(0m));
    }

    [Test]
    public async Task GetTotalValueAsync_WithZeroQuantity_ShouldNotContribute()
    {
        // Arrange
        var wine1 = new Wine { EstimatedPrice = 20m, Quantity = 0 };
        var wine2 = new Wine { EstimatedPrice = 30m, Quantity = 2 };

        await _repository.CreateAsync(wine1);
        await _repository.CreateAsync(wine2);

        // Act
        var result = await _repository.GetTotalValueAsync();

        // Assert
        Assert.That(result, Is.EqualTo(60m)); // Only wine2 contributes: 30 * 2
    }

    [Test]
    public async Task GetTotalValueAsync_WithZeroPrice_ShouldNotContribute()
    {
        // Arrange
        var wine1 = new Wine { EstimatedPrice = 0m, Quantity = 3 };
        var wine2 = new Wine { EstimatedPrice = 25m, Quantity = 1 };

        await _repository.CreateAsync(wine1);
        await _repository.CreateAsync(wine2);

        // Act
        var result = await _repository.GetTotalValueAsync();

        // Assert
        Assert.That(result, Is.EqualTo(25m)); // Only wine2 contributes: 25 * 1
    }
}
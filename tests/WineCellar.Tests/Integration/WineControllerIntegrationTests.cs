using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;
using WineCellar.Api.Controllers;
using WineCellar.Core.Entities;
using WineCellar.Infrastructure.Repositories;

namespace WineCellar.Tests.Integration;

public class WineControllerIntegrationTests
{
    private readonly WinesController _controller;
    private readonly InMemoryWineRepository _repository;

    public WineControllerIntegrationTests()
    {
        _repository = new InMemoryWineRepository();
        _controller = new WinesController(_repository);
    }

    [Test]
    public async Task CreateAndRetrieveWine_EndToEnd_ShouldWork()
    {
        // Arrange
        var wine = new Wine
        {
            Name = "Integration Test Wine",
            Producer = "Test Producer",
            Year = 2020,
            EstimatedPrice = 25.00m,
            Quantity = 2
        };

        // Act - Create
        var createResult = await _controller.CreateWine(wine);
        var createdWine = ((CreatedAtActionResult)createResult.Result!).Value as Wine;

        // Act - Retrieve
        var getResult = await _controller.GetWine(createdWine!.Id);
        var retrievedWine = ((OkObjectResult)getResult.Result!).Value as Wine;

        // Assert
        Assert.That(createdWine, Is.Not.Null);
        Assert.That(retrievedWine, Is.Not.Null);
        Assert.That(retrievedWine.Id, Is.EqualTo(createdWine.Id));
        Assert.That(retrievedWine.Name, Is.EqualTo("Integration Test Wine"));
        Assert.That(retrievedWine.Producer, Is.EqualTo("Test Producer"));
    }

    [Test]
    public async Task FullCrudOperations_EndToEnd_ShouldWork()
    {
        // Arrange
        var wine = new Wine
        {
            Name = "CRUD Test Wine",
            Producer = "CRUD Producer",
            EstimatedPrice = 30.00m,
            Quantity = 1
        };

        // Create
        var createResult = await _controller.CreateWine(wine);
        var createdWine = ((CreatedAtActionResult)createResult.Result!).Value as Wine;
        Assert.That(createdWine, Is.Not.Null);

        // Read
        var getResult = await _controller.GetWine(createdWine.Id);
        Assert.That(getResult.Result, Is.TypeOf<OkObjectResult>());

        // Update
        createdWine.Name = "Updated CRUD Wine";
        createdWine.EstimatedPrice = 35.00m;
        var updateResult = await _controller.UpdateWine(createdWine.Id, createdWine);
        var updatedWine = ((OkObjectResult)updateResult.Result!).Value as Wine;
        Assert.That(updatedWine!.Name, Is.EqualTo("Updated CRUD Wine"));
        Assert.That(updatedWine.EstimatedPrice, Is.EqualTo(35.00m));

        // Delete
        var deleteResult = await _controller.DeleteWine(createdWine.Id);
        Assert.That(deleteResult, Is.TypeOf<NoContentResult>());

        // Verify deletion
        var getAfterDeleteResult = await _controller.GetWine(createdWine.Id);
        Assert.That(getAfterDeleteResult.Result, Is.TypeOf<NotFoundResult>());
    }

    [Test]
    public async Task TotalValueCalculation_WithMultipleWines_ShouldBeAccurate()
    {
        var initialCount = (await _controller.GetWines()).Result;
        var initialWines = ((OkObjectResult)initialCount!).Value as IEnumerable<Wine>;
        var initialTotalResult = await _controller.GetTotalValue();
        var initialTotal = (decimal)((OkObjectResult)initialTotalResult.Result!).Value!
            .GetType().GetProperty("TotalValue")!.GetValue(((OkObjectResult)initialTotalResult.Result!).Value)!;

        var wines = new List<Wine>
        {
            new() { Name = "Wine 1", EstimatedPrice = 20.00m, Quantity = 2 }, // 40
            new() { Name = "Wine 2", EstimatedPrice = 15.50m, Quantity = 3 }, // 46.50
            new() { Name = "Wine 3", EstimatedPrice = 30.00m, Quantity = 1 } // 30
        };

        // Act - Create wines
        foreach (var wine in wines) await _controller.CreateWine(wine);

        // Act - Get total value
        var totalResult = await _controller.GetTotalValue();
        var totalValue = ((OkObjectResult)totalResult.Result!).Value;
        var totalValueProperty = totalValue!.GetType().GetProperty("TotalValue");
        var actualTotal = (decimal)totalValueProperty!.GetValue(totalValue)!;

        // Assert
        var expectedIncrease = 116.50m; // 40 + 46.50 + 30
        Assert.That(actualTotal, Is.EqualTo(initialTotal + expectedIncrease));
    }

    [Test]
    public async Task ConcurrentOperations_ShouldMaintainDataIntegrity()
    {
        // Arrange
        var initialResult = await _controller.GetWines();
        var initialWines = ((OkObjectResult)initialResult.Result!).Value as IEnumerable<Wine>;
        var initialCount = initialWines!.Count();

        var wines = Enumerable.Range(1, 10)
            .Select(i => new Wine
            {
                Name = $"Concurrent Wine {i}",
                Producer = $"Concurrent Producer {i}",
                EstimatedPrice = i * 10m,
                Quantity = i
            }).ToList();

        // Act - Create wines concurrently
        var createTasks = wines.Select(wine => _controller.CreateWine(wine));
        await Task.WhenAll(createTasks);

        // Act - Get all wines
        var getAllResult = await _controller.GetWines();
        var allWines = ((OkObjectResult)getAllResult.Result!).Value as IEnumerable<Wine>;

        // Assert
        Assert.That(allWines!.Count(), Is.EqualTo(initialCount + 10));
        for (var i = 1; i <= 10; i++)
            Assert.That(allWines, Has.Some.Matches<Wine>(w => w.Name == $"Concurrent Wine {i}"));
    }
}
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using WineCellar.Api.Controllers;
using WineCellar.Core.Entities;
using WineCellar.Core.Interfaces;

namespace WineCellar.Tests.Unit.Controllers;

public class WinesControllerTests
{
    private WinesController _controller;
    private Mock<IWineRepository> _mockRepository;

    [SetUp]
    public void SetUp()
    {
        _mockRepository = new Mock<IWineRepository>();
        _controller = new WinesController(_mockRepository.Object);
    }

    [Test]
    public async Task GetWines_ReturnsOkResult_WithWinesList()
    {
        // Arrange
        var wines = new List<Wine>
        {
            new() { Id = Guid.NewGuid(), Name = "Test Wine 1" },
            new() { Id = Guid.NewGuid(), Name = "Test Wine 2" }
        };
        _mockRepository.Setup(repo => repo.GetAllAsync()).ReturnsAsync(wines);

        // Act
        var result = await _controller.GetWines();

        // Assert
        Assert.That(result.Result, Is.TypeOf<OkObjectResult>());
        var okResult = (OkObjectResult)result.Result;

        var returnedWines = (IEnumerable<Wine>)okResult.Value;
        Assert.That(returnedWines.Count(), Is.EqualTo(2));
    }

    [Test]
    public async Task GetWines_ReturnsOkResult_WithEmptyList()
    {
        // Arrange
        _mockRepository.Setup(repo => repo.GetAllAsync()).ReturnsAsync(new List<Wine>());

        // Act
        var result = await _controller.GetWines();

        // Assert
        Assert.That(result.Result, Is.TypeOf<OkObjectResult>());
        var okResult = (OkObjectResult)result.Result;
        var returnedWines = (IEnumerable<Wine>)okResult.Value;
        Assert.That(returnedWines, Is.Empty);
    }

    [Test]
    public async Task GetWine_WithValidId_ReturnsOkResult()
    {
        // Arrange
        var wineId = Guid.NewGuid();
        var wine = new Wine { Id = wineId, Name = "Test Wine" };
        _mockRepository.Setup(repo => repo.GetByIdAsync(wineId)).ReturnsAsync(wine);

        // Act
        var result = await _controller.GetWine(wineId);

        // Assert
        Assert.That(result.Result, Is.TypeOf<OkObjectResult>());
        var okResult = (OkObjectResult)result.Result;
        Assert.That(okResult.Value, Is.TypeOf<Wine>());
        var returnedWine = (Wine)okResult.Value;
        Assert.That(returnedWine.Id, Is.EqualTo(wineId));
    }

    [Test]
    public async Task GetWine_WithInvalidId_ReturnsNotFound()
    {
        // Arrange
        var wineId = Guid.NewGuid();
        _mockRepository.Setup(repo => repo.GetByIdAsync(wineId)).ReturnsAsync((Wine?)null);

        // Act
        var result = await _controller.GetWine(wineId);

        // Assert
        Assert.That(result.Result, Is.TypeOf<NotFoundResult>());
    }

    [Test]
    public async Task CreateWine_ReturnsCreatedAtActionResult()
    {
        // Arrange
        var wine = new Wine { Name = "New Wine", Producer = "New Producer" };
        var createdWine = new Wine { Id = Guid.NewGuid(), Name = "New Wine", Producer = "New Producer" };
        _mockRepository.Setup(repo => repo.CreateAsync(wine)).ReturnsAsync(createdWine);

        // Act
        var result = await _controller.CreateWine(wine);

        // Assert
        Assert.That(result.Result, Is.TypeOf<CreatedAtActionResult>());
        var createdResult = (CreatedAtActionResult)result.Result;
        Assert.That(createdResult.ActionName, Is.EqualTo(nameof(_controller.GetWine)));
        Assert.That(((Wine)createdResult.Value!).Id, Is.EqualTo(createdWine.Id));
    }

    [Test]
    public async Task UpdateWine_WithValidId_ReturnsUpdatedWine()
    {
        // Arrange
        var wineId = Guid.NewGuid();
        var existingWine = new Wine { Id = wineId, Name = "Old Wine" };
        var updatedWine = new Wine { Id = wineId, Name = "Updated Wine", Producer = "Updated Producer" };

        _mockRepository.Setup(repo => repo.GetByIdAsync(wineId)).ReturnsAsync(existingWine);
        _mockRepository.Setup(repo => repo.UpdateAsync(updatedWine)).ReturnsAsync(updatedWine);

        // Act
        var result = await _controller.UpdateWine(wineId, updatedWine);

        // Assert
        Assert.That(result.Result, Is.TypeOf<OkObjectResult>());
        var okResult = (OkObjectResult)result.Result;
        Assert.That(okResult.Value, Is.TypeOf<Wine>());
        var returnedWine = (Wine)okResult.Value;
        Assert.That(returnedWine.Name, Is.EqualTo(updatedWine.Name));
    }

    [Test]
    public async Task UpdateWine_WithMismatchedIds_ReturnsBadRequest()
    {
        // Arrange
        var urlId = Guid.NewGuid();
        var bodyId = Guid.NewGuid();
        var wine = new Wine { Id = bodyId, Name = "Test Wine" };

        // Act
        var result = await _controller.UpdateWine(urlId, wine);

        // Assert
        Assert.That(result.Result, Is.TypeOf<BadRequestObjectResult>());
        var badRequestResult = (BadRequestObjectResult)result.Result;
        Assert.That(badRequestResult.Value, Is.EqualTo("URL ID does not match wine ID."));
    }

    [Test]
    public async Task UpdateWine_WithNonExistentWine_ReturnsNotFound()
    {
        // Arrange
        var wineId = Guid.NewGuid();
        var wine = new Wine { Id = wineId, Name = "Test Wine" };
        _mockRepository.Setup(repo => repo.GetByIdAsync(wineId)).ReturnsAsync((Wine?)null);

        // Act
        var result = await _controller.UpdateWine(wineId, wine);

        // Assert
        Assert.That(result.Result, Is.TypeOf<NotFoundObjectResult>());
        var notFoundResult = (NotFoundObjectResult)result.Result;
        Assert.That(notFoundResult.Value, Is.EqualTo($"Wine with ID {wineId} not found."));
    }

    [Test]
    public async Task UpdateWine_WhenRepositoryThrows_ReturnsInternalServerError()
    {
        // Arrange
        var wineId = Guid.NewGuid();
        var existingWine = new Wine { Id = wineId, Name = "Old Wine" };
        var updatedWine = new Wine { Id = wineId, Name = "Updated Wine" };

        _mockRepository.Setup(repo => repo.GetByIdAsync(wineId)).ReturnsAsync(existingWine);
        _mockRepository.Setup(repo => repo.UpdateAsync(updatedWine))
            .ThrowsAsync(new InvalidOperationException("simulated"));

        // Act
        var result = await _controller.UpdateWine(wineId, updatedWine);

        // Assert
        Assert.That(result.Result, Is.TypeOf<ObjectResult>());
        var objResult = (ObjectResult)result.Result;
        Assert.That(objResult.StatusCode, Is.EqualTo(500));
        Assert.That(objResult.Value?.ToString(), Does.Contain("Error updating wine"));
    }

    [Test]
    public async Task DeleteWine_WithExistingId_ReturnsNoContent()
    {
        // Arrange
        var wineId = Guid.NewGuid();
        var existingWine = new Wine { Id = wineId, Name = "Test Wine" };
        _mockRepository.Setup(repo => repo.GetByIdAsync(wineId)).ReturnsAsync(existingWine);
        _mockRepository.Setup(repo => repo.DeleteAsync(wineId)).Returns(Task.CompletedTask);

        // Act
        var result = await _controller.DeleteWine(wineId);

        // Assert
        Assert.That(result, Is.TypeOf<NoContentResult>());
        _mockRepository.Verify(repo => repo.DeleteAsync(wineId), Times.Once);
    }

    [Test]
    public async Task DeleteWine_WithNonExistentId_ReturnsNotFound()
    {
        // Arrange
        var wineId = Guid.NewGuid();
        _mockRepository.Setup(repo => repo.GetByIdAsync(wineId)).ReturnsAsync((Wine?)null);

        // Act
        var result = await _controller.DeleteWine(wineId);

        // Assert
        Assert.That(result, Is.TypeOf<NotFoundObjectResult>());
        var notFoundResult = (NotFoundObjectResult)result;
    }

    [Test]
    public async Task GetTotalValue_ReturnsOkResult_WithTotalValue()
    {
        // Arrange
        var totalValue = 150.75m;
        _mockRepository.Setup(repo => repo.GetTotalValueAsync()).ReturnsAsync(totalValue);

        // Act
        var result = await _controller.GetTotalValue();

        // Assert
        Assert.That(result.Result, Is.TypeOf<OkObjectResult>());
        var okResult = (OkObjectResult)result.Result;
        Assert.That(okResult.Value, Is.Not.Null);
        var totalValueProperty = okResult.Value.GetType().GetProperty("TotalValue");
        Assert.That(totalValueProperty?.GetValue(okResult.Value), Is.EqualTo(totalValue));
    }
}
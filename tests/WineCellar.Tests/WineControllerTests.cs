using Microsoft.AspNetCore.Mvc;
using Moq;
using WineCellar.Api.Controllers;
using WineCellar.Core.Entities;
using WineCellar.Core.Interfaces;

namespace WineCellar.Tests;

public class WinesControllerTests
{
    private readonly Mock<IWineRepository> _mockRepository;
    private readonly WinesController _controller;

    public WinesControllerTests()
    {
        _mockRepository = new Mock<IWineRepository>();
        _controller = new WinesController(_mockRepository.Object);
    }

    [Fact]
    public async Task GetWines_ReturnsOkResult_WithWinesList()
    {
        // Arrange
        var wines = new List<Wine>
        {
            new Wine { Id = Guid.NewGuid(), Name = "Test Wine 1" },
            new Wine { Id = Guid.NewGuid(), Name = "Test Wine 2" }
        };
        _mockRepository.Setup(repo => repo.GetAllAsync()).ReturnsAsync(wines);

        // Act
        var result = await _controller.GetWines();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedWines = Assert.IsAssignableFrom<IEnumerable<Wine>>(okResult.Value);
        Assert.Equal(2, returnedWines.Count());
    }

    [Fact]
    public async Task GetWine_WithValidId_ReturnsOkResult()
    {
        // Arrange
        var wineId = Guid.NewGuid();
        var wine = new Wine { Id = wineId, Name = "Test Wine" };
        _mockRepository.Setup(repo => repo.GetByIdAsync(wineId)).ReturnsAsync(wine);

        // Act
        var result = await _controller.GetWine(wineId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedWine = Assert.IsType<Wine>(okResult.Value);
        Assert.Equal(wineId, returnedWine.Id);
    }

    [Fact]
    public async Task GetWine_WithInvalidId_ReturnsNotFound()
    {
        // Arrange
        var wineId = Guid.NewGuid();
        _mockRepository.Setup(repo => repo.GetByIdAsync(wineId)).ReturnsAsync((Wine?)null);

        // Act
        var result = await _controller.GetWine(wineId);

        // Assert
        Assert.IsType<NotFoundResult>(result.Result);
    }

    [Fact]
    public async Task CreateWine_ReturnsCreatedAtActionResult()
    {
        // Arrange
        var wine = new Wine { Name = "New Wine", Producer = "New Producer" };
        var createdWine = new Wine { Id = Guid.NewGuid(), Name = "New Wine", Producer = "New Producer" };
        _mockRepository.Setup(repo => repo.CreateAsync(wine)).ReturnsAsync(createdWine);

        // Act
        var result = await _controller.CreateWine(wine);

        // Assert
        var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);
        Assert.Equal(nameof(_controller.GetWine), createdResult.ActionName);
        Assert.Equal(createdWine.Id, ((Wine)createdResult.Value!).Id);
    }

    [Fact]
    public async Task GetTotalValue_ReturnsOkResult_WithTotalValue()
    {
        // Arrange
        var totalValue = 150.75m;
        _mockRepository.Setup(repo => repo.GetTotalValueAsync()).ReturnsAsync(totalValue);

        // Act
        var result = await _controller.GetTotalValue();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var response = okResult.Value;
        Assert.NotNull(response);
        var totalValueProperty = response.GetType().GetProperty("TotalValue");
        Assert.Equal(totalValue, totalValueProperty?.GetValue(response));
    }
}
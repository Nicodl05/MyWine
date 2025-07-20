using WineCellar.Core.Entities;
using WineCellar.Core.Interfaces;

namespace WineCellar.Infrastructure.Repositories;

public class InMemoryWineRepository : IWineRepository
{
    private readonly List<Wine> _wines = new();

    public Task<IEnumerable<Wine>> GetAllAsync()
    {
        return Task.FromResult<IEnumerable<Wine>>(_wines.ToList());
    }

    public Task<Wine?> GetByIdAsync(Guid id)
    {
        var wine = _wines.FirstOrDefault(w => w.Id == id);
        return Task.FromResult(wine);
    }

    public Task<Wine> CreateAsync(Wine wine)
    {
        wine.Id = Guid.NewGuid();
        _wines.Add(wine);
        return Task.FromResult(wine);
    }

    public Task<Wine> UpdateAsync(Wine wine)
    {
        var existingWine = _wines.FirstOrDefault(w => w.Id == wine.Id);
        if (existingWine != null)
        {
            existingWine.Name = wine.Name;
            existingWine.Producer = wine.Producer;
            existingWine.Year = wine.Year;
            existingWine.Region = wine.Region;
            existingWine.Type = wine.Type;
            existingWine.EstimatedPrice = wine.EstimatedPrice;
            existingWine.Quantity = wine.Quantity;
            existingWine.Notes = wine.Notes;
        }
        return Task.FromResult(existingWine ?? wine);
    }

    public Task DeleteAsync(Guid id)
    {
        var wine = _wines.FirstOrDefault(w => w.Id == id);
        if (wine != null)
        {
            _wines.Remove(wine);
        }
        return Task.CompletedTask;
    }

    public Task<decimal> GetTotalValueAsync()
    {
        var totalValue = _wines.Sum(w => w.EstimatedPrice * w.Quantity);
        return Task.FromResult(totalValue);
    }
}
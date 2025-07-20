using WineCellar.Core.Entities;

namespace WineCellar.Core.Interfaces;

public interface IWineRepository
{
    Task<IEnumerable<Wine>> GetAllAsync();
    Task<Wine?> GetByIdAsync(Guid id);
    Task<Wine> CreateAsync(Wine wine);
    Task<Wine> UpdateAsync(Wine wine);
    Task DeleteAsync(Guid id);
    Task<decimal> GetTotalValueAsync();
}
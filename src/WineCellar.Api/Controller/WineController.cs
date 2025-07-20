using Microsoft.AspNetCore.Mvc;
using WineCellar.Core.Entities;
using WineCellar.Core.Interfaces;

namespace WineCellar.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class WinesController : ControllerBase
{
    private readonly IWineRepository _wineRepository;

    public WinesController(IWineRepository wineRepository)
    {
        _wineRepository = wineRepository;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Wine>>> GetWines()
    {
        var wines = await _wineRepository.GetAllAsync();
        return Ok(wines);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Wine>> GetWine(Guid id)
    {
        var wine = await _wineRepository.GetByIdAsync(id);
        if (wine == null)
        {
            return NotFound();
        }
        return Ok(wine);
    }

    [HttpPost]
    public async Task<ActionResult<Wine>> CreateWine(Wine wine)
    {
        var createdWine = await _wineRepository.CreateAsync(wine);
        return CreatedAtAction(nameof(GetWine), new { id = createdWine.Id }, createdWine);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateWine(Guid id, Wine wine)
    {
        if (id != wine.Id)
        {
            return BadRequest();
        }

        await _wineRepository.UpdateAsync(wine);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteWine(Guid id)
    {
        await _wineRepository.DeleteAsync(id);
        return NoContent();
    }

    [HttpGet("total-value")]
    public async Task<ActionResult<object>> GetTotalValue()
    {
        var totalValue = await _wineRepository.GetTotalValueAsync();
        return Ok(new { TotalValue = totalValue });
    }
}
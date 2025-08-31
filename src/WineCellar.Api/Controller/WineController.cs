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
        if (wine == null) return NotFound();
        return Ok(wine);
    }

    [HttpPost]
    public async Task<ActionResult<Wine>> CreateWine(Wine wine)
    {
        var createdWine = await _wineRepository.CreateAsync(wine);
        return CreatedAtAction(nameof(GetWine), new { id = createdWine.Id }, createdWine);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<Wine>> UpdateWine(Guid id, Wine wine)
    {
        if (id != wine.Id) return BadRequest("URL ID does not match wine ID.");

        var existingWine = await _wineRepository.GetByIdAsync(id);
        if (existingWine == null) return NotFound($"Wine with ID {id} not found.");

        try
        {
            var updatedWine = await _wineRepository.UpdateAsync(wine);
            return Ok(updatedWine);
        }
        catch (InvalidOperationException ex)
        {
            return StatusCode(500, $"Error updating wine: {ex.Message}");
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteWine(Guid id)
    {
        var existingWine = await _wineRepository.GetByIdAsync(id);
        if (existingWine == null) return NotFound($"Vin avec l'ID {id} introuvable.");

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
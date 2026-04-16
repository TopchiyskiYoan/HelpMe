using HelpMe.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace HelpMe.Web.Controllers;

[ApiController]
[Route("api/regions")]
public class RegionsController : ControllerBase
{
    private readonly IRegionService _regionService;

    public RegionsController(IRegionService regionService)
    {
        _regionService = regionService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var regions = await _regionService.GetAllAsync();
        return Ok(regions);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var region = await _regionService.GetByIdAsync(id);
        if (region is null) return NotFound();

        return Ok(region);
    }
}

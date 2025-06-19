using kiss_graph_api.DTOs;
using kiss_graph_api.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class FranchiseController : ControllerBase
{
    private readonly IFranchiseService _franchiseService;
    private readonly ILogger<FranchiseController> _logger;

    public FranchiseController(IFranchiseService franchiseService, ILogger<FranchiseController> logger)
    {
        _franchiseService = franchiseService ?? throw new ArgumentNullException(nameof(franchiseService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    [HttpGet]
    public async Task<IActionResult> GetAllFranchises()
    {
        _logger.LogInformation("API: Getting all franchises");
        var works = await _franchiseService.GetAllFranchisesAsync();
        return Ok(works);
    }

    [HttpGet("{uuid}")]
    public async Task<IActionResult> GetFranchiseByUuid(string uuid)
    {
        _logger.LogInformation($"Getting franchise by uuid: {uuid}");
        var work = await _franchiseService.GetFranchiseByUuidAsync(uuid);
        return Ok(work);
    }

    [HttpPost]
    public async Task<IActionResult> CreateFranchise([FromBody] CreateFranchiseDto franchiseDto)
    {
        _logger.LogInformation($"Creating new Franchise: {franchiseDto.Name}");
        var franchise = await _franchiseService.CreateFranchiseAsync(franchiseDto);
        return CreatedAtAction(nameof(CreateFranchise), franchise);
    }

    [HttpPatch("{uuid}")]
    public async Task<IActionResult> UpdateFranchise(string uuid,[FromBody] UpdateFranchiseDto franchiseDto)
    {
        _logger.LogInformation($"Creating new Franchise: {franchiseDto.Name}");
        var franchise = await _franchiseService.UpdateFranchiseAsync(uuid, franchiseDto);
        return CreatedAtAction(nameof(CreateFranchise), franchise);
    }

    [HttpDelete("{uuid}")]
    public async Task<IActionResult> DeleteMovie(string uuid)
    {
        _logger.LogInformation($"Deleting Franchise: {uuid}");
        await _franchiseService.DeleteFranchiseAsync(uuid);
        return NoContent();
    }
}


using kiss_graph_api.DTOs;
using kiss_graph_api.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Neo4j.Driver;
using System;

[ApiController]
[Route("api/[controller]")]
public class CreativeWorkController : ControllerBase
{
    private readonly ICreativeWorkService _creativeWorkService;
    private readonly ILogger<CreativeWorkController> _logger;

    // Constructor: ASP.NET Core will inject registered services here
    public CreativeWorkController(ICreativeWorkService creativeWorkService, ILogger<CreativeWorkController> logger)
    {
        _creativeWorkService = creativeWorkService ?? throw new ArgumentNullException(nameof(creativeWorkService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    [HttpGet]
    public async Task<IActionResult> GetAllCreativeWorks()
    {
        _logger.LogInformation("API: Getting all Creative Works");
        var works = await _creativeWorkService.GetAllCreativeWorksAsync();
        return Ok(works);
    }

    [HttpGet("{uuid}")]
    public async Task<IActionResult> GetCreativeWorkByUuid(string uuid)
    {
        _logger.LogInformation($"Getting CreativeWork by uuid: {uuid}");
        var work = await _creativeWorkService.GetCreativeWorkByUuidAsync(uuid);
        return Ok(work);
    }

    [HttpPost]
    public async Task<IActionResult> CreateCreativeWork([FromBody] CreateCreativeWorkDto creativeWork)
    {
        _logger.LogInformation($"Creating new Creative Work: {creativeWork.Title}");
        var work = await _creativeWorkService.CreateCreativeWorkAsync(creativeWork);
        return CreatedAtAction("Created Creative Work", work);
    }

    [HttpDelete("{uuid}")]
    public async Task<IActionResult> DeleteCreativeWork(string uuid)
    {
        _logger.LogInformation($"Deleting Creative Work: {uuid}");
        await _creativeWorkService.DeleteCreativeWorkAsync(uuid);
        return NoContent();
    }
}


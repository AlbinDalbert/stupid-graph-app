using kiss_graph_api.Domain;
using kiss_graph_api.DTOs;
using kiss_graph_api.Services.Implementations;
using kiss_graph_api.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class GenreController : ControllerBase
{
    private readonly IGenreService _genreService;
    private readonly ILogger<GenreController> _logger;

    public GenreController(IGenreService genreService, ILogger<GenreController> logger)
    {
        _genreService = genreService ?? throw new ArgumentNullException(nameof(genreService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    [HttpGet]
    public async Task<IActionResult> GetAllGenre()
    {
        _logger.LogInformation("API: Getting all Genres");
        return Ok(await _genreService.GetAllGenreAsync());
    }

    [HttpGet("{uuid}")]
    public async Task<IActionResult> GetGenreByUuid(string uuid)
    {
        _logger.LogInformation("API: Getting all Genres");
        return Ok(await _genreService.GetGenreByUuidAsync(uuid));
    }

    [HttpPost]
    public async Task<IActionResult> CreateGenre([FromForm] CreateGenreDto dto)
    {
        _logger.LogInformation("API: Create Genre");
        var genre = await _genreService.CreateGenreAsync(dto);
        return CreatedAtAction(nameof(CreateGenre), genre);
    }

    [HttpPatch("{uuid}")]
    public async Task<IActionResult> UpdateGenre(string uuid,[FromForm] UpdateGenreDto dto)
    {
        _logger.LogInformation("API: Update Genre");
        var genre = await _genreService.UpdateGenreAsync(uuid, dto);
        return CreatedAtAction(nameof(UpdateGenre), genre);
    }

    [HttpDelete("{uuid}")]
    public async Task<IActionResult> DeleteGenre(string uuid)
    {
        _logger.LogInformation("API: Delete Genre");
        await _genreService.DeleteGenreAsync(uuid);
        return NoContent();
    }
}

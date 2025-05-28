using kiss_graph_api.DTOs;
using kiss_graph_api.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class MovieController : ControllerBase
{
    private readonly IMovieService _movieService;
    private readonly ILogger<MovieController> _logger;

    public MovieController(IMovieService movieService, ILogger<MovieController> logger)
    {
        _movieService = movieService ?? throw new ArgumentNullException(nameof(movieService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    [HttpGet]
    public async Task<IActionResult> GetAllMovie()
    {
        _logger.LogInformation("API: Getting all Creative Works");
        var works = await _movieService.GetAllCreativeWorksAsync();
        return Ok(works);
    }

    [HttpGet("{uuid}")]
    public async Task<IActionResult> GetMovieByUuid(string uuid)
    {
        _logger.LogInformation($"Getting CreativeWork by uuid: {uuid}");
        var work = await _movieService.GetCreativeWorkByUuidAsync(uuid);
        return Ok(work);
    }

    [HttpPost]
    public async Task<IActionResult> CreateMovie([FromBody] CreateMovieDto movie)
    {
        _logger.LogInformation($"Creating new Creative Work: {movie.Title}");
        var work = await _movieService.CreateCreativeWorkAsync(movie);
        return CreatedAtAction(nameof(CreateMovie), work);
    }

    [HttpPatch("{uuid}")]
    public async Task<IActionResult> UpdateMovie(string uuid,[FromBody] UpdateMovieDto movie)
    {
        _logger.LogInformation($"Creating new Creative Work: {movie.Title}");
        var work = await _movieService.UpdateCreativeWorkAsync(uuid, movie);
        return CreatedAtAction(nameof(CreateMovie), work);
    }

    [HttpDelete("{uuid}")]
    public async Task<IActionResult> DeleteMovie(string uuid)
    {
        _logger.LogInformation($"Deleting Creative Work: {uuid}");
        await _movieService.DeleteCreativeWorkAsync(uuid);
        return NoContent();
    }
}


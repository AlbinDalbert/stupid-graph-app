using kiss_graph_api.DTOs;
using kiss_graph_api.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using static kiss_graph_api.Constants.NeoProp;

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
    public async Task<IActionResult> UpdateMovie(string uuid, [FromBody] UpdateMovieDto movie)
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

    [HttpPost("{movieUuid}/franchise/{franchiseUuid}")]
    public async Task<IActionResult> AssignMovieToFranchise(string movieUuid, string franchiseUuid)
    {
        _logger.LogInformation($"Assigning movie ({movieUuid}) to franchise ({franchiseUuid})");
        await _movieService.AssignToFranchise(movieUuid, franchiseUuid);
        return NoContent();
    }

    [HttpDelete("{movieUuid}/franchise/{franchiseUuid}")]
    public async Task<IActionResult> RemoveMovieFromFranchise(string movieUuid, string franchiseUuid)
    {
        _logger.LogInformation($"Remove movie ({movieUuid}) from franchise ({franchiseUuid})");
        await _movieService.RemoveFromFranchise(movieUuid, franchiseUuid);
        return NoContent();
    }

    [HttpPost("{movieUuid}/genre/{genreUuid}")]
    public async Task<IActionResult> AssignGenre(string movieUuid, string genreUuid)
    {
        _logger.LogInformation($"Assigning movie ({movieUuid}) to genre ({genreUuid})");
        await _movieService.AssignGenre(movieUuid, genreUuid);
        return NoContent();
    }

    [HttpDelete("{movieUuid}/genre/{genreUuid}")]
    public async Task<IActionResult> RemoveGenre(string movieUuid, string genreUuid)
    {
        _logger.LogInformation($"Remove genre ( {genreUuid}) from movie ({movieUuid})");
        await _movieService.RemoveGenre(movieUuid, genreUuid);
        return NoContent();
    }
}


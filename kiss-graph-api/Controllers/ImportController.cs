using kiss_graph_api.DTOs;
using kiss_graph_api.Services.Implementations;
using kiss_graph_api.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class ImportController : ControllerBase
{
    private readonly ITmdbImportService _importService;
    private readonly ILogger<ImportController> _logger;

    public ImportController(ITmdbImportService importService, ILogger<ImportController> logger)
    {
        _importService = importService;
        _logger = logger;
    }

    [HttpPost("tmdb/movies")]
    public IActionResult ImportMoviesFromTmdb([FromBody] ImportMoviesRequestDto request)
    {
        if (request == null || request.TmdbMovieIds == null || !request.TmdbMovieIds.Any())
        {
            return BadRequest("A list of TMDB movie IDs is required.");
        }

        _logger.LogInformation("Received import request for {Count} movies.", request.TmdbMovieIds.Count);

        // Don't await this task. Kick it off to run in the background.
        // This lets us return an immediate response to the client.
        _ = _importService.ImportMoviesByIdsAsync(request.TmdbMovieIds);

        // Return HTTP 202 Accepted to indicate the request was received
        // and is being processed asynchronously.
        return Accepted($"Import request for {request.TmdbMovieIds.Count} movies received and is being processed in the background.");
    }
}

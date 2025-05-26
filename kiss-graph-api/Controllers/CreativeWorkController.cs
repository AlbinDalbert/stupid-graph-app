using kiss_graph_api.DTOs;
using kiss_graph_api.Services.Implementations;
using kiss_graph_api.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Neo4j.Driver; // You'll need this for Neo4j interactions
using System; // For Environment.GetEnvironmentVariable if you configure driver here, though typically it's injected
using System.Collections.Generic; // For List<T>
using System.Threading.Tasks; // For asynchronous methods

[ApiController]
[Route("api/[controller]")]
public class CreativeWorkController : ControllerBase
{
    private readonly IDriver _neo4jDriver; // For interacting with Neo4j
    private readonly ICreativeWorkService _creativeWorkService;
    private readonly ILogger<CreativeWorkController> _logger; // Optional: for logging

    // Constructor: ASP.NET Core will inject registered services here
    public CreativeWorkController(IDriver neo4jDriver, ICreativeWorkService creativeWorkService, ILogger<CreativeWorkController> logger)
    {
        _creativeWorkService = creativeWorkService ?? throw new ArgumentNullException(nameof(creativeWorkService));
        _neo4jDriver = neo4jDriver ?? throw new ArgumentNullException(nameof(neo4jDriver));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    [HttpGet]
    public async Task<IActionResult> GetAllCreativeWorks()
    {
        _logger.LogInformation("API: Getting all Creative Works");

        //await using var session = _neo4jDriver.AsyncSession();

        try
        {
            var works = await _creativeWorkService.GetAllCreativeWorksAsync();
            return Ok(works); // Return the DTOs directly
        }
        catch (Exception ex)
        {
            // The service might handle/log, but we catch anything unexpected.
            _logger.LogError(ex, "API: Error getting all CreativeWorks");
            return StatusCode(500, "Internal server error"); // Generic error message
        }

        //try
        //{
        //    var reader = await session.ExecuteReadAsync(async tx =>
        //    {
        //        var cursor = await tx.RunAsync(@"
        //            MATCH (c:CreativeWork)
        //            RETURN c.uuid AS uuid, c.title AS title, c.type AS type
        //            ORDER BY c.title
        //        ");

        //        return await cursor.ToListAsync(record => new
        //        {
        //            Uuid = record["uuid"].As<string>(),
        //            Title = record["title"].As<string>(),
        //            WorkType = record["type"].As<string>(),
        //            ReleaseDate = record["ReleaseDate"].As<LocalDate>(),
        //        });
        //    });

        //    works.AddRange(reader);
        //    return Ok(works);
        //}
    }


    [HttpGet("{uuid}")]
    public async Task<IActionResult> GetCreativeWorkByUuid(string uuid)
    {
        _logger.LogInformation($"Getting CreativeWork by uuid: {uuid}");

        await using var session = _neo4jDriver.AsyncSession();

        try
        {
            var creativeWork = await session.ExecuteReadAsync(async tx =>
            {
                var cursor = await tx.RunAsync(@"
                    MATCH (c:CreativeWork)
                    WHERE c.uuid = $uuid
                    RETURN c.uuid AS uuid, c.title AS title, c.type AS type
                ", new { uuid });

                var record = await cursor.SingleAsync();
                return record == null ? null : new
                {
                    Uuid = record["uuid"].As<string>(),
                    Title = record["title"].As<string>(),
                    WorkType = record["type"].As<string>()
                };
            });

            if (creativeWork == null)
            {
                _logger.LogWarning($"CreativeWork with ID: {uuid} not found");
                return NotFound(); // HTTP 404
            }
            return Ok(creativeWork);
        }
        catch (InvalidOperationException ex) when (ex.Message.Contains("Sequence contains no elements") || ex.Message.Contains("Sequence contains more than one element"))
        {
            _logger.LogWarning(ex, $"creative work with ID: {uuid} not found or multiple found");
            return NotFound($"creative work with ID {uuid} not found.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error getting creative work by ID: {uuid} from Neo4j");
            return StatusCode(500, "Internal server error");
        }
    }

    // POST: api/Forests
    [HttpPost]
    public async Task<IActionResult> CreateForest([FromBody] CreateCreativeWork creativeWork)
    {

        _logger.LogInformation("Creating new forest: {ForestName}", creativeWork.Title);
        await using var session = _neo4jDriver.AsyncSession();

        LocalDate? neo4jDate = creativeWork.ReleaseDate is not null
            ? new LocalDate(
                creativeWork.ReleaseDate.Value.Year,
                creativeWork.ReleaseDate.Value.Month,
                creativeWork.ReleaseDate.Value.Day)
            : null;


        try
        {
            Guid guid = Guid.NewGuid();
            string uuid = guid.ToString();


            var createWork = await session.ExecuteWriteAsync(async tx =>
            {
                var cursor = await tx.RunAsync(@"
                    CREATE (c:CreativeWork {
                        uuid: $uuid,
                        title: $title,
                        type: $type,
                        releaseDate: $releaseDate
                    })
                    RETURN c.uuid AS uuid, c.title AS title, c.type AS type, c.releaseDate AS releaseDate
                ", new
                {
                    uuid,
                    title = creativeWork.Title,
                    type = EnumHelper.GetEnumMemberValue(creativeWork.Type),
                    releaseDate = neo4jDate
                });

                var record = await cursor.SingleAsync();
                return new
                {
                    Uuid = record["uuid"].As<string>(),
                    Title = record["title"].As<string>(),
                    WorkType = record["type"].As<string>(),
                    ReleaseDate = record["releaseDate"].As<LocalDate>()
                };
            });


            // HTTP 201 Created: Includes the new resource in the response and a Location header
            return CreatedAtAction(nameof(GetCreativeWorkByUuid), new { Uuid = createWork.Uuid }, createWork);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error creating creativeWork: {creativeWork.Title} in Neo4j");
            return StatusCode(500, "Internal server error while creating creative work");
        }
    }

    [HttpDelete("{uuid}")]
    public async Task<IActionResult> DeleteCreativeWork(string uuid)
    {
        await using var session = _neo4jDriver.AsyncSession();
        try
        {
            await session.ExecuteWriteAsync(async tx =>
            {
                await tx.RunAsync(@"
                MATCH (c:CreativeWork {uuid: $uuid})
                DETACH DELETE c
            ", new { uuid });
            });

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error deleting CreativeWork with uuid {uuid}");
            return StatusCode(500, "Failed to delete CreativeWork");
        }
        finally
        {
            await session.CloseAsync();
        }
    }
}


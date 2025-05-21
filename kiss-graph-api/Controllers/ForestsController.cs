using Microsoft.AspNetCore.Mvc;
using Neo4j.Driver; // You'll need this for Neo4j interactions
using System; // For Environment.GetEnvironmentVariable if you configure driver here, though typically it's injected
using System.Collections.Generic; // For List<T>
using System.Threading.Tasks; // For asynchronous methods

namespace kiss_graph_api
{

    // Define a simple model for creating a forest (Data Transfer Object - DTO)
    // It's good practice to put this in a separate file or Models folder
    public class CreateForestRequest
    {
        public required string Name { get; set; }
        public required string Location { get; set; }
        public double? AreaSqKm { get; set; } // Nullable if optional
    }

    [ApiController] // Marks this class as an API controller, enabling certain conventions
    [Route("api/[controller]")] // Sets the base route for this controller to "api/Forests"
    public class ForestsController : ControllerBase
    {
        private readonly IDriver _neo4jDriver; // For interacting with Neo4j
        private readonly ILogger<ForestsController> _logger; // Optional: for logging

        // Constructor: ASP.NET Core will inject registered services here
        public ForestsController(IDriver neo4jDriver, ILogger<ForestsController> logger)
        {
            _neo4jDriver = neo4jDriver ?? throw new ArgumentNullException(nameof(neo4jDriver));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpGet]
        public async Task<IActionResult> GetAllForests()
        {
            _logger.LogInformation("Getting all forests");
            var forests = new List<object>();

            await using var session = _neo4jDriver.AsyncSession();

            try
            {
                var reader = await session.ExecuteReadAsync(async tx =>
                {
                    // Conceptual Cypher query from previous discussion
                    var cursor = await tx.RunAsync(@"
                        MATCH (f:Forest)
                        RETURN id(f) AS id, f.name AS name, f.location AS location, f.areaSqKm AS areaSqKm
                        ORDER BY f.name
                    ");
                    // Materialize the results into a list of dictionaries or DTOs
                    return await cursor.ToListAsync(record => new // Create a Forest DTO here
                    {
                        Id = record["id"].As<long>(),
                        Name = record["name"].As<string>(),
                        Location = record["location"].As<string>(),
                        AreaSqKm = record["areaSqKm"].As<double?>() // Use nullable if property might be missing
                    });
                });

                forests.AddRange(reader);
                return Ok(forests);
            }
            catch (Exception ex)
            {
                _logger.LogError($"error getting all forests: {ex}");
                return StatusCode(500, "Internal server error while fetching forests");
            }
        }

        // GET: api/Forests/{id}
        [HttpGet("{id}")] // The "{id}" part is a route parameter
        public async Task<IActionResult> GetForestById(long id)
        {
            _logger.LogInformation($"Getting forest by id: {id}");
            var forests = new List<object>();

            await using var session = _neo4jDriver.AsyncSession();

            try
            {
                var forest = await session.ExecuteReadAsync(async tx =>
                {
                    var cursor = await tx.RunAsync(@"
                        MATCH (f:Forest)
                        WHERE id(f) = $id
                        RETURN id(f) AS id, f.name AS name, f.location AS location, f.areaSqKm AS areaSqKm
                    ", new { id }); // Pass the id as a parameter to the query

                    var record = await cursor.SingleAsync(); // Expecting a single result or it throws an error
                    return record == null ? null : new // Create a Forest DTO here
                    {
                        Id = record["id"].As<long>(),
                        Name = record["name"].As<string>(),
                        Location = record["location"].As<string>(),
                        AreaSqKm = record["areaSqKm"].As<double?>()
                    };
                });

                if (forest == null)
                {
                    _logger.LogWarning("Forest with ID: {ForestId} not found", id);
                    return NotFound(); // HTTP 404
                }
                return Ok(forest);
            }
            catch (InvalidOperationException ex) when (ex.Message.Contains("Sequence contains no elements") || ex.Message.Contains("Sequence contains more than one element"))
            {
                _logger.LogWarning(ex, "Forest with ID: {ForestId} not found or multiple found", id);
                return NotFound($"Forest with ID {id} not found.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting forest by ID: {ForestId} from Neo4j", id);
                return StatusCode(500, "Internal server error");
            }
        }

        // POST: api/Forests
        [HttpPost]
        public async Task<IActionResult> CreateForest([FromBody] CreateForestRequest forestRequest)
        // [FromBody] tells ASP.NET Core to get the 'forestRequest' data from the request body (JSON)
        {
            if (forestRequest == null || string.IsNullOrWhiteSpace(forestRequest.Name))
            {
                return BadRequest("Forest name is required."); // HTTP 400
            }

            _logger.LogInformation("Creating new forest: {ForestName}", forestRequest.Name);
            await using var session = _neo4jDriver.AsyncSession();
            try
            {
                var createdForest = await session.ExecuteWriteAsync(async tx =>
                {
                    var cursor = await tx.RunAsync(@"
                        CREATE (f:Forest {
                            name: $name,
                            location: $location,
                            areaSqKm: $areaSqKm,
                            createdAt: timestamp()
                        })
                        RETURN id(f) AS id, f.name AS name, f.location AS location, f.areaSqKm AS areaSqKm
                    ", new {
                        name = forestRequest.Name,
                        location = forestRequest.Location,
                        areaSqKm = forestRequest.AreaSqKm
                    });

                    var record = await cursor.SingleAsync();
                    return new // Create a Forest DTO/Response model here
                    {
                        Id = record["id"].As<long>(),
                        Name = record["name"].As<string>(),
                        Location = record["location"].As<string>(),
                        AreaSqKm = record["areaSqKm"].As<double?>()
                    };
                });

                // HTTP 201 Created: Includes the new resource in the response and a Location header
                return CreatedAtAction(nameof(GetForestById), new { id = createdForest.Id }, createdForest);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating forest: {ForestName} in Neo4j", forestRequest.Name);
                return StatusCode(500, "Internal server error while creating forest");
            }
        }
    }
}
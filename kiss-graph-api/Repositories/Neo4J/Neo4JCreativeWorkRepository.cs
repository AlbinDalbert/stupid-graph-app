using Neo4j.Driver;
using kiss_graph_api.DTOs; // Use your DTO namespace
using kiss_graph_api.Repositories.Interfaces; // Use your Interface namespace
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using kiss_graph_api.Models.Enums;

namespace kiss_graph_api.Repositories.Neo4j
{
    public class Neo4JCreativeWorkRepository : ICreativeWorkRepository
    {

    private readonly IDriver _driver;
    private readonly ILogger<Neo4JCreativeWorkRepository> _logger;


    public Neo4JCreativeWorkRepository(IDriver driver, ILogger<Neo4JCreativeWorkRepository> logger)
    {
        _driver = driver;
        _logger = logger;
    }

    public async Task<IEnumerable<CreativeWorkDto>> GetAllAsync()
    {
        _logger.LogInformation("Repository: Getting all Creative Works from Neo4j");
        await using var session = _driver.AsyncSession();

        try
        {
            var works = await session.ExecuteReadAsync(async tx =>
            {
                var cursor = await tx.RunAsync(@"
                    MATCH (c:CreativeWork)
                    RETURN c.uuid AS uuid, c.title AS title, c.type AS type, c.releaseDate AS releaseDate
                    ORDER BY c.title
                ");

                // Map directly to your DTO!
                return await cursor.ToListAsync(record => MapRecordToCreativeWorkDto(record));

            });
            _logger.LogInformation("Repository: Mapped {Count} works.", works.Count);
            return works;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Repository: Error getting all CreativeWorks from Neo4j");
            throw; // Re-throw the exception so the service/controller layer can handle it if needed, or return empty/null.
        }
    }

    // --- Add other methods (GetById, Create, Delete) here ---
    public Task<CreativeWorkDto?> GetByUuidAsync(string uuid) { throw new NotImplementedException(); }
    public Task<CreativeWorkDto> CreateAsync(CreativeWorkDto creativeWork) { throw new NotImplementedException(); }
    public Task<bool> DeleteAsync(string uuid) { throw new NotImplementedException(); }

    private CreativeWorkDto MapRecordToCreativeWorkDto(IRecord record)
    {
        // --- Determine all values first ---

        // Required fields (we assume they exist)
        var uuid = record["uuid"].As<string>();
        var title = record["title"].As<string>();
        var workType = ParseCreativeWorkType(record["type"]); // Use your helper

        DateOnly? releaseDate = null; // Start as null

        _logger.LogDebug("Mapping record. Keys available: {Keys}", string.Join(", ", record.Keys)); // Log all keys

        // --- DETAILED RELEASE DATE DEBUGGING ---
        if (record.Keys.Contains("releaseDate"))
        {
            var releaseDateObject = record["releaseDate"]; // Get the raw object

            // --- Log its value and, crucially, its TYPE ---
            _logger.LogInformation("Found 'releaseDate' key. Value: [{Value}], Type: [{Type}]",
                releaseDateObject?.ToString() ?? "NULL",
                releaseDateObject?.GetType().FullName ?? "NULL");

            if (releaseDateObject != null)
            {
                try
                {
                    // Attempt the conversion (adjust based on logged type if needed)
                    releaseDate = releaseDateObject.As<LocalDate>().ToDateOnly();
                    _logger.LogInformation("Successfully parsed ReleaseDate to: {ParsedDate}", releaseDate);
                }
                catch (Exception ex)
                {
                    // --- Use LogError here to make it REALLY stand out ---
                    _logger.LogError(ex, "*** FAILED to parse ReleaseDate. Original Value: {Value}, Type: {Type} ***",
                        releaseDateObject,
                        releaseDateObject.GetType().FullName);
                    releaseDate = null; // Explicitly set back to null on error
                }
            }
            else
            {
                _logger.LogWarning("'releaseDate' key found, but its value is NULL.");
            }
        }
        else
        {
            _logger.LogWarning("'releaseDate' key NOT found in record!"); // Warn if key is missing
        }

        //// Optional/Nullable fields (Use the 'Keys.Contains' check)
        //DateOnly? releaseDate = record.Keys.Contains("releaseDate") && record["releaseDate"] != null
        //                    ? record["releaseDate"].As<LocalDate>().ToDateOnly() 
        //                    : null;

        // --- Create the immutable record in one go ---
        return new CreativeWorkDto
        {
            Uuid = uuid,
            Title = title,
            Type = workType,
            ReleaseDate = releaseDate
        };
    }

        // --- THE NEW, DEDICATED TYPE PARSING HELPER ---
        private CreativeWorkType ParseCreativeWorkType(object typeObject)
    {
        var workTypeEnum = CreativeWorkType.Other; // Default value

        if (typeObject != null && typeObject is string typeString && !string.IsNullOrEmpty(typeString))
        {
            try
            {
                workTypeEnum = EnumHelper.ParseEnumMemberValue<CreativeWorkType>(typeString);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "An unknown CreativeWorkType '{TypeString}' was found. Defaulting to 'Other'.", typeString);
                workTypeEnum = CreativeWorkType.Other;
            }
        }
        else if (typeObject != null)
        {
            _logger.LogWarning("CreativeWork 'type' was found but was not a string: {TypeValue}. Defaulting to 'Other'.", typeObject);
        }

        return workTypeEnum;
    }
    }

}

using Neo4j.Driver;
using kiss_graph_api.DTOs;
using kiss_graph_api.Repositories.Interfaces;
using kiss_graph_api.Domain.Enums;
using kiss_graph_api.Constants;

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
                    var cursor = await tx.RunAsync($@"
                        MATCH (c:{NeoLabels.CreativeWork})
                        RETURN  c.{NeoPropKeys.Uuid} AS {NeoPropKeys.Uuid}, 
                                c.{NeoPropKeys.Title} AS {NeoPropKeys.Title}, 
                                c.{NeoPropKeys.Type} AS {NeoPropKeys.Type}, 
                                c.{NeoPropKeys.ReleaseDate} AS {NeoPropKeys.ReleaseDate}
                        ORDER BY c.{NeoPropKeys.Title}
                    ");

                    return await cursor.ToListAsync(record => MapRecordToCreativeWorkDto(record));

                });

                return works;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Repository: Error getting all CreativeWorks from Neo4j");
                throw;
            }
        }
        public async Task<CreativeWorkDto?> GetByUuidAsync(string uuid) 
        {
            _logger.LogInformation("Repository: Getting Creative Works by uuid from Neo4j");
            await using var session = _driver.AsyncSession();

            try
            {
                var creativeWork = await session.ExecuteReadAsync(async tx =>
                {
                    var cursor = await tx.RunAsync($@"
                        MATCH (c:{NeoLabels.CreativeWork})
                        WHERE c.{NeoPropKeys.Uuid} = ${NeoPropKeys.Uuid}
                        RETURN  c.{NeoPropKeys.Uuid} AS {NeoPropKeys.Uuid}, 
                                c.{NeoPropKeys.Title} AS {NeoPropKeys.Title}, 
                                c.{NeoPropKeys.Type} AS {NeoPropKeys.Type}, 
                                c.{NeoPropKeys.ReleaseDate} AS {NeoPropKeys.ReleaseDate}
                    ", new { uuid });

                    var results = await cursor.ToListAsync(record => MapRecordToCreativeWorkDto(record));
                    return results.FirstOrDefault();
                });

                if (creativeWork == null)
                {
                    _logger.LogWarning($"CreativeWork with ID: {uuid} not found");
                    return null;
                }
                return creativeWork;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Repository: Error getting all CreativeWorks from Neo4j");
                throw;
            }
        }
        public async Task<CreativeWorkDto> CreateAsync(CreateCreativeWorkDto creativeWork) 
        {
            _logger.LogInformation("Repository: Add Creative Works to Neo4j");
            await using var session = _driver.AsyncSession();

            LocalDate? neo4jDate = creativeWork.ReleaseDate is not null
                ? new LocalDate(
                    creativeWork.ReleaseDate.Value.Year,
                    creativeWork.ReleaseDate.Value.Month,
                    creativeWork.ReleaseDate.Value.Day)
                : null;

            Guid guid = Guid.NewGuid();
            string uuid = guid.ToString();

            var query = $@"
            CREATE (c:{NeoLabels.CreativeWork} {{
                {NeoPropKeys.Uuid}: ${NeoPropKeys.Uuid},
                {NeoPropKeys.Title}: ${NeoPropKeys.Title},
                {NeoPropKeys.Type}: ${NeoPropKeys.Type},
                {NeoPropKeys.ReleaseDate}: ${NeoPropKeys.ReleaseDate}
            }})
            RETURN  c.{NeoPropKeys.Uuid} AS {NeoPropKeys.Uuid}, 
                    c.{NeoPropKeys.Title} AS {NeoPropKeys.Title}, 
                    c.{NeoPropKeys.Type} AS {NeoPropKeys.Type}, 
                    c.{NeoPropKeys.ReleaseDate} AS {NeoPropKeys.ReleaseDate}
            ";

            var parameters = new Dictionary<string, object?>
            {
                { NeoPropKeys.Uuid, uuid },
                { NeoPropKeys.Title, creativeWork.Title },
                { NeoPropKeys.Type, EnumHelper.GetEnumMemberValue(creativeWork.Type) },
                { NeoPropKeys.ReleaseDate, neo4jDate }
            };

            try
            {
                var createWork = await session.ExecuteWriteAsync(async tx =>
                {
                    var cursor = await tx.RunAsync(query, parameters);
                    var record = await cursor.SingleAsync();
                    return MapRecordToCreativeWorkDto(record);
                });
                return createWork;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Repository: Error getting all CreativeWorks from Neo4j");
                throw;
            }

        }
        public async Task DeleteAsync(string uuid) 
        {
            _logger.LogInformation("Repository: Delete Creative Works from Neo4j");
            await using var session = _driver.AsyncSession();

            try
            {
                await session.ExecuteWriteAsync(async tx =>
                {
                    await tx.RunAsync($@"
                        MATCH (c:{NeoLabels.CreativeWork} {{{NeoPropKeys.Uuid}: ${NeoPropKeys.Uuid}})
                        DETACH DELETE c
                    ", new { uuid });
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Repository: Error Deleting CreativeWorks from Neo4j");
                throw;
            }
        }

        // ----- Helpers ----- //

        private CreativeWorkDto MapRecordToCreativeWorkDto(IRecord record)
        {
            // --- Determine all values first ---

            // Required fields (we assume they exist)
            var uuid = record[NeoPropKeys.Uuid].As<string>();
            var title = record[NeoPropKeys.Title].As<string>();
            var workType = ParseCreativeWorkType(record[NeoPropKeys.Type]); 

            DateOnly? releaseDate = null; // Start as null

            _logger.LogDebug("Mapping record. Keys available: {Keys}", string.Join(", ", record.Keys)); // Log all keys

            // --- DETAILED RELEASE DATE DEBUGGING ---
            if (record.Keys.Contains(NeoPropKeys.ReleaseDate))
            {
                var releaseDateObject = record[NeoPropKeys.ReleaseDate];

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

            // --- Create the immutable record in one go ---
            return new CreativeWorkDto
            {
                Uuid = uuid,
                Title = title,
                Type = workType,
                ReleaseDate = releaseDate
            };
        }
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

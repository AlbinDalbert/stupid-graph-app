using Neo4j.Driver;
using kiss_graph_api.DTOs;
using kiss_graph_api.Repositories.Interfaces;
using kiss_graph_api.Domain.Enums;
using kiss_graph_api.Constants;
using System.Text;

namespace kiss_graph_api.Repositories.Neo4j
{
    public class Neo4JMovieRepository : IMovieRepository
    {

        private readonly IDriver _driver;
        private readonly ILogger<Neo4JMovieRepository> _logger;
        public Neo4JMovieRepository(IDriver driver, ILogger<Neo4JMovieRepository> logger)
        {
            _driver = driver;
            _logger = logger;
        }

        public async Task<IEnumerable<MovieDto>> GetAllAsync()
        {
            _logger.LogInformation("Repository: Getting all Creative Works from Neo4j");
            await using var session = _driver.AsyncSession();

            try
            {
                var works = await session.ExecuteReadAsync(async tx =>
                {
                    var cursor = await tx.RunAsync($@"
                        MATCH (c:{NeoLabels.CreativeWork})
                        RETURN  c.{NeoProp.Movie.Uuid} AS {NeoProp.Movie.Uuid}, 
                                c.{NeoProp.Movie.Title} AS {NeoProp.Movie.Title}, 
                                c.{NeoProp.Movie.Type} AS {NeoProp.Movie.Type}, 
                                c.{NeoProp.Movie.ReleaseDate} AS {NeoProp.Movie.ReleaseDate}
                        ORDER BY c.{NeoProp.Movie.Title}
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
        public async Task<MovieDto?> GetByUuidAsync(string uuid) 
        {
            _logger.LogInformation("Repository: Getting Creative Works by uuid from Neo4j");
            await using var session = _driver.AsyncSession();

            try
            {
                var creativeWork = await session.ExecuteReadAsync(async tx =>
                {
                    var cursor = await tx.RunAsync($@"
                        MATCH (c:{NeoLabels.CreativeWork})
                        WHERE c.{NeoProp.Movie.Uuid} = ${NeoProp.Movie.Uuid}
                        RETURN  c.{NeoProp.Movie.Uuid} AS {NeoProp.Movie.Uuid}, 
                                c.{NeoProp.Movie.Title} AS {NeoProp.Movie.Title}, 
                                c.{NeoProp.Movie.Type} AS {NeoProp.Movie.Type}, 
                                c.{NeoProp.Movie.ReleaseDate} AS {NeoProp.Movie.ReleaseDate}
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
        public async Task<MovieDto> CreateAsync(CreateMovieDto movie) 
        {
            _logger.LogInformation("Repository: Add Creative Works to Neo4j");
            await using var session = _driver.AsyncSession();

            LocalDate? neo4jDate = movie.ReleaseDate is not null
                ? new LocalDate(
                    movie.ReleaseDate.Value.Year,
                    movie.ReleaseDate.Value.Month,
                    movie.ReleaseDate.Value.Day)
                : null;

            Guid guid = Guid.NewGuid();
            string uuid = guid.ToString();

            var query = $@"
            CREATE (c:{NeoLabels.CreativeWork} {{
                {NeoProp.Movie.Uuid}: ${NeoProp.Movie.Uuid},
                {NeoProp.Movie.Title}: ${NeoProp.Movie.Title},
                {NeoProp.Movie.Type}: ${NeoProp.Movie.Type},
                {NeoProp.Movie.ReleaseDate}: ${NeoProp.Movie.ReleaseDate}
            }})
            RETURN  c.{NeoProp.Movie.Uuid} AS {NeoProp.Movie.Uuid}, 
                    c.{NeoProp.Movie.Title} AS {NeoProp.Movie.Title}, 
                    c.{NeoProp.Movie.Type} AS {NeoProp.Movie.Type}, 
                    c.{NeoProp.Movie.ReleaseDate} AS {NeoProp.Movie.ReleaseDate}
            ";

            var parameters = new Dictionary<string, object?>
            {
                { NeoProp.Movie.Uuid, uuid },
                { NeoProp.Movie.Title, movie.Title },
                { NeoProp.Movie.Type, CreativeWorkType.Movie.ToString() },
                { NeoProp.Movie.ReleaseDate, neo4jDate }
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
        public async Task<MovieDto?> UpdateAsync(string uuid, UpdateMovieDto updateDto)
        {
            _logger.LogInformation("Repository: Updating CreativeWork {Uuid}", uuid);
            await using var session = _driver.AsyncSession();

            var setClauses = new List<string>();
            var parameters = new Dictionary<string, object?>
            {
                { NeoProp.Movie.Uuid, uuid }
            };

            if (updateDto.Title != null)
            {
                setClauses.Add($"c.{NeoProp.Movie.Title} = ${NeoProp.Movie.Title}");
                parameters.Add(NeoProp.Movie.Title, updateDto.Title);
            }

            if (updateDto.ReleaseDate.HasValue)
            {
                setClauses.Add($"c.{NeoProp.Movie.ReleaseDate} = ${NeoProp.Movie.ReleaseDate}");
                parameters.Add(NeoProp.Movie.ReleaseDate, new LocalDate(
                    updateDto.ReleaseDate.Value.Year,
                    updateDto.ReleaseDate.Value.Month,
                    updateDto.ReleaseDate.Value.Day));
            }

            if (!setClauses.Any())
            {
                _logger.LogInformation("Repository: No properties provided to update for CreativeWork {Uuid}. Fetching current.", uuid);
                return await GetByUuidAsync(uuid);
            }

            var queryBuilder = new StringBuilder();
            queryBuilder.Append($"MATCH (c:{NeoLabels.CreativeWork} {{{NeoProp.Movie.Uuid}: ${NeoProp.Movie.Uuid}}}) ");
            queryBuilder.Append("SET ");
            queryBuilder.Append(string.Join(", ", setClauses));
            queryBuilder.Append($@"
                RETURN c.{NeoProp.Movie.Uuid} AS {NeoProp.Movie.Uuid},
                        c.{NeoProp.Movie.Title} AS {NeoProp.Movie.Title},
                        c.{NeoProp.Movie.Type} AS {NeoProp.Movie.Type},
                        c.{NeoProp.Movie.ReleaseDate} AS {NeoProp.Movie.ReleaseDate}
            ");

            try
            {
                var updatedWork = await session.ExecuteWriteAsync(async tx =>
                {
                    var cursor = await tx.RunAsync(queryBuilder.ToString(), parameters);
                    var record = await cursor.SingleAsync();
                    return MapRecordToCreativeWorkDto(record);
                });
                return updatedWork;
            }
            catch (InvalidOperationException ex) when (ex.Message.ToLowerInvariant().Contains("sequence contains no elements"))
            {
                _logger.LogWarning(ex, "Repository: CreativeWork {Uuid} not found for update.", uuid);
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Repository: Error updating CreativeWork {Uuid}", uuid);
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
                        MATCH (c:{NeoLabels.CreativeWork} {{{NeoProp.Movie.Uuid}: ${NeoProp.Movie.Uuid}}})
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

        private MovieDto MapRecordToCreativeWorkDto(IRecord record)
        {

            var uuid = record[NeoProp.Movie.Uuid].As<string>();
            var title = record[NeoProp.Movie.Title].As<string>();
            var workType = ParseCreativeWorkType(record[NeoProp.Movie.Type]); 

            DateOnly? releaseDate = null;

            _logger.LogDebug("Mapping record. Keys available: {Keys}", string.Join(", ", record.Keys)); // Log all keys

            if (record.Keys.Contains(NeoProp.Movie.ReleaseDate))
            {
                var releaseDateObject = record[NeoProp.Movie.ReleaseDate];

                _logger.LogInformation("Found 'releaseDate' key. Value: [{Value}], Type: [{Type}]",
                    releaseDateObject?.ToString() ?? "NULL",
                    releaseDateObject?.GetType().FullName ?? "NULL");

                if (releaseDateObject != null)
                {
                    try
                    {
                        releaseDate = releaseDateObject.As<LocalDate>().ToDateOnly();
                        _logger.LogInformation("Successfully parsed ReleaseDate to: {ParsedDate}", releaseDate);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "*** FAILED to parse ReleaseDate. Original Value: {Value}, Type: {Type} ***",
                            releaseDateObject,
                            releaseDateObject.GetType().FullName);
                        releaseDate = null;
                    }
                }
                else
                {
                    _logger.LogWarning("'releaseDate' key found, but its value is NULL.");
                }
            }
            else
            {
                _logger.LogWarning("'releaseDate' key NOT found in record!");
            }

            return new MovieDto
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

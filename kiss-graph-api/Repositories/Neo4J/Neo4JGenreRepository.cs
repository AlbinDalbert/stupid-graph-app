using Neo4j.Driver;
using kiss_graph_api.DTOs;
using kiss_graph_api.Repositories.Interfaces;
using kiss_graph_api.Domain.Enums;
using kiss_graph_api.Constants;
using System.Text;

namespace kiss_graph_api.Repositories.Neo4j
{
    public class Neo4JGenreRepository : IGenreRepository
    {
        private readonly IDriver _driver;
        private readonly ILogger<Neo4JCharacterRepository> _logger;
        public Neo4JGenreRepository(IDriver driver, ILogger<Neo4JCharacterRepository> logger)
        {
            _driver = driver;
            _logger = logger;
        }

        public async Task<GenreDto> CreateAsync(CreateGenreDto genre)
        {
            _logger.LogInformation("Repository: Add genre to Neo4j");
            await using var session = _driver.AsyncSession();

            Guid guid = Guid.NewGuid();
            string uuid = guid.ToString();

            var query = $@"
            CREATE (c:{NeoLabels.Genre} {{
                {NeoProp.Genre.Uuid}: ${NeoProp.Genre.Uuid},
                {NeoProp.Genre.Name}: ${NeoProp.Genre.Name}
            }})
            RETURN  c.{NeoProp.Genre.Uuid} AS {NeoProp.Genre.Uuid}, 
                    c.{NeoProp.Genre.Name} AS {NeoProp.Genre.Name}
            ";

            var parameters = new Dictionary<string, object?>
            {
                { NeoProp.Genre.Uuid, uuid },
                { NeoProp.Genre.Name, genre.Name }
            };

            try
            {
                var genreRet = await session.ExecuteWriteAsync(async tx =>
                {
                    var cursor = await tx.RunAsync(query, parameters);
                    var record = await cursor.SingleAsync();
                    return MapRecordToGenreDto(record);
                });
                return genreRet;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Repository: Error adding Genre to Neo4j");
                throw;
            }
        }

        public async Task<GenreDto?> GetByUuidAsync(string uuid)
        {
            _logger.LogInformation("Repository: Getting Character by uuid from Neo4j");
            await using var session = _driver.AsyncSession();

            try
            {
                var character = await session.ExecuteReadAsync(async tx =>
                {
                    var cursor = await tx.RunAsync($@"
                        MATCH (c:{NeoLabels.Genre})
                        WHERE c.{NeoProp.Genre.Uuid} = ${NeoProp.Genre.Uuid}
                        RETURN  c.{NeoProp.Genre.Uuid} AS {NeoProp.Genre.Uuid}, 
                                c.{NeoProp.Genre.Name} AS {NeoProp.Genre.Name}
                    ", new { uuid });

                    var results = await cursor.ToListAsync(record => MapRecordToGenreDto(record));
                    return results.FirstOrDefault();
                });

                if (character == null)
                {
                    _logger.LogWarning($"Genre with ID: {uuid} not found");
                    return null;
                }
                return character;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Repository: Error getting Genre by uuid from Neo4j");
                throw;
            }
        }

        public async Task DeleteAsync(string uuid)
        {
            _logger.LogInformation("Repository: Delete Character from Neo4j");
            await using var session = _driver.AsyncSession();

            try
            {
                await session.ExecuteWriteAsync(async tx =>
                {
                    await tx.RunAsync($@"
                        MATCH (c:{NeoLabels.Genre} {{{NeoProp.Genre.Uuid}: ${NeoProp.Genre.Uuid}}})
                        DETACH DELETE c
                    ", new { uuid });
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Repository: Error deleting Genre from Neo4j");
                throw;
            }
        }

        public async Task<IEnumerable<GenreDto>> GetAllAsync()
        {
            _logger.LogInformation("Repository: Getting all Genres from Neo4j");
            await using var session = _driver.AsyncSession();

            try
            {
                var genres = await session.ExecuteReadAsync(async tx =>
                {
                    var cursor = await tx.RunAsync($@"
                        MATCH (c:{NeoLabels.Genre})
                        RETURN  c.{NeoProp.Genre.Uuid} AS {NeoProp.Genre.Uuid}, 
                                c.{NeoProp.Genre.Name} AS {NeoProp.Genre.Name}
                        ORDER BY c.{NeoProp.Genre.Name}
                    ");

                    return await cursor.ToListAsync(record => MapRecordToGenreDto(record));

                });

                return genres;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Repository: Error getting all Characters from Neo4j");
                throw;
            }
        }

        public async Task<GenreDto?> UpdateAsync(string uuid, UpdateGenreDto genreDto)
        {
            _logger.LogInformation("Repository: Updating Genre {Uuid}", uuid);
            await using var session = _driver.AsyncSession();

            var setClauses = new List<string>();
            var parameters = new Dictionary<string, object?>
            {
                { NeoProp.Genre.Uuid, uuid }
            };

            if (genreDto.Name != null)
            {
                setClauses.Add($"c.{NeoProp.Genre.Name} = ${NeoProp.Genre.Name}");
                parameters.Add(NeoProp.Genre.Name, genreDto.Name);
            }

            if (!setClauses.Any())
            {
                _logger.LogInformation("Repository: No properties provided to update for Character {Uuid}. Fetching current.", uuid);
                return await GetByUuidAsync(uuid);
            }

            var queryBuilder = new StringBuilder();
            queryBuilder.Append($"MATCH (c:{NeoLabels.Genre} {{{NeoProp.Genre.Uuid}: ${NeoProp.Genre.Uuid}}}) ");
            queryBuilder.Append("SET ");
            queryBuilder.Append(string.Join(", ", setClauses));
            queryBuilder.Append($@"
                RETURN c.{NeoProp.Genre.Uuid} AS {NeoProp.Genre.Uuid},
                        c.{NeoProp.Genre.Name} AS {NeoProp.Genre.Name}
            ");

            try
            {
                var updateGenre = await session.ExecuteWriteAsync(async tx =>
                {
                    var cursor = await tx.RunAsync(queryBuilder.ToString(), parameters);
                    var record = await cursor.SingleAsync();
                    return MapRecordToGenreDto(record);
                });
                return updateGenre;
            }
            catch (InvalidOperationException ex) when (ex.Message.ToLowerInvariant().Contains("sequence contains no elements"))
            {
                _logger.LogWarning(ex, "Repository: Genre {Uuid} not found for update.", uuid);
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Repository: Error updating Genre {Uuid}", uuid);
                throw;
            }
        }

        // --- HELPERS --- //

        private GenreDto MapRecordToGenreDto(IRecord record)
        {

            var uuid = record[NeoProp.Genre.Uuid].As<string>();
            var name = record[NeoProp.Genre.Name].As<string>();

            _logger.LogDebug("Mapping record. Keys available: {Keys}", string.Join(", ", record.Keys));

            return new GenreDto
            {
                Uuid = uuid,
                Name = name
            };
        }
    }
}

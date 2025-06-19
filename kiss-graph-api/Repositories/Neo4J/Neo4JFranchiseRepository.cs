using Neo4j.Driver;
using kiss_graph_api.DTOs;
using kiss_graph_api.Repositories.Interfaces;
using kiss_graph_api.Domain.Enums;
using kiss_graph_api.Constants;
using System.Text;

namespace kiss_graph_api.Repositories.Neo4j
{
    public class Neo4JFranchiseRepository : IFranchiseRepository
    {

        private readonly IDriver _driver;
        private readonly ILogger<Neo4JFranchiseRepository> _logger;
        public Neo4JFranchiseRepository(IDriver driver, ILogger<Neo4JFranchiseRepository> logger)
        {
            _driver = driver;
            _logger = logger;
        }

        public async Task<IEnumerable<FranchiseDto>> GetAllAsync()
        {
            _logger.LogInformation("Repository: Getting all franchises from Neo4j");
            await using var session = _driver.AsyncSession();

            try
            {
                var franchises = await session.ExecuteReadAsync(async tx =>
                {
                    var cursor = await tx.RunAsync($@"
                        MATCH (c:{NeoLabels.Franchise})
                        RETURN  c.{NeoProp.Franchise.Uuid} AS {NeoProp.Franchise.Uuid}, 
                                c.{NeoProp.Franchise.Name} AS {NeoProp.Franchise.Name}
                        ORDER BY c.{NeoProp.Franchise.Name}
                    ");

                    return await cursor.ToListAsync(record => MapRecordToFranchiseDto(record));

                });

                return franchises;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Repository: Error getting all franchises from Neo4j");
                throw;
            }
        }
        public async Task<FranchiseDto?> GetByUuidAsync(string uuid) 
        {
            _logger.LogInformation("Repository: Getting franchise by uuid from Neo4j");
            await using var session = _driver.AsyncSession();

            try
            {
                var franchise = await session.ExecuteReadAsync(async tx =>
                {
                    var cursor = await tx.RunAsync($@"
                        MATCH (c:{NeoLabels.Franchise})
                        WHERE c.{NeoProp.Franchise.Uuid} = ${NeoProp.Franchise.Uuid}
                        RETURN  c.{NeoProp.Franchise.Uuid} AS {NeoProp.Franchise.Uuid},
                                c.{NeoProp.Franchise.Name} AS {NeoProp.Franchise.Name}
                    ", new { uuid });

                    var results = await cursor.ToListAsync(record => MapRecordToFranchiseDto(record));
                    return results.FirstOrDefault();
                });

                if (franchise == null)
                {
                    _logger.LogWarning($"Franchise with ID: {uuid} not found");
                    return null;
                }
                return franchise;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Repository: Error getting Franchise by uuid from Neo4j");
                throw;
            }
        }
        public async Task<FranchiseDto> CreateAsync(CreateFranchiseDto franch) 
        {
            _logger.LogInformation("Repository: Add franchise to Neo4j");
            await using var session = _driver.AsyncSession();

            Guid guid = Guid.NewGuid();
            string uuid = guid.ToString();

            var query = $@"
            CREATE (c:{NeoLabels.Franchise} {{
                {NeoProp.Franchise.Uuid}: ${NeoProp.Franchise.Uuid},
                {NeoProp.Franchise.Name}: ${NeoProp.Franchise.Name}
            }})
            RETURN  c.{NeoProp.Franchise.Uuid} AS {NeoProp.Franchise.Uuid}, 
                    c.{NeoProp.Franchise.Name} AS {NeoProp.Franchise.Name}
            ";

            var parameters = new Dictionary<string, object?>
            {
                { NeoProp.Franchise.Uuid, uuid },
                { NeoProp.Franchise.Name, franch.Name }
            };

            try
            {
                var createWork = await session.ExecuteWriteAsync(async tx =>
                {
                    var cursor = await tx.RunAsync(query, parameters);
                    var record = await cursor.SingleAsync();
                    return MapRecordToFranchiseDto(record);
                });
                return createWork;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Repository: Error adding Franchise to Neo4j");
                throw;
            }
        }
        public async Task<FranchiseDto?> UpdateAsync(string uuid, UpdateFranchiseDto updateDto)
        {
            _logger.LogInformation("Repository: Updating franchise {Uuid}", uuid);
            await using var session = _driver.AsyncSession();

            var setClauses = new List<string>();
            var parameters = new Dictionary<string, object?>
            {
                { NeoProp.Movie.Uuid, uuid }
            };

            if (updateDto.Name != null)
            {
                setClauses.Add($"c.{NeoProp.Franchise.Name} = ${NeoProp.Franchise.Name}");
                parameters.Add(NeoProp.Franchise.Name, updateDto.Name);
            }

            if (!setClauses.Any())
            {
                _logger.LogInformation("Repository: No properties provided to update for Franchise {Uuid}. Fetching current.", uuid);
                return await GetByUuidAsync(uuid);
            }

            var queryBuilder = new StringBuilder();
            queryBuilder.Append($"MATCH (c:{NeoLabels.Franchise} {{{NeoProp.Franchise.Uuid}: ${NeoProp.Franchise.Uuid}}}) ");
            queryBuilder.Append("SET ");
            queryBuilder.Append(string.Join(", ", setClauses));
            queryBuilder.Append($@"
                RETURN c.{NeoProp.Franchise.Uuid} AS {NeoProp.Franchise.Uuid},
                        c.{NeoProp.Franchise.Name} AS {NeoProp.Franchise.Name}
            ");

            try
            {
                var updatedWork = await session.ExecuteWriteAsync(async tx =>
                {
                    var cursor = await tx.RunAsync(queryBuilder.ToString(), parameters);
                    var record = await cursor.SingleAsync();
                    return MapRecordToFranchiseDto(record);
                });
                return updatedWork;
            }
            catch (InvalidOperationException ex) when (ex.Message.ToLowerInvariant().Contains("sequence contains no elements"))
            {
                _logger.LogWarning(ex, "Repository: Franchise {Uuid} not found for update.", uuid);
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Repository: Error updating Franchise {Uuid}", uuid);
                throw;
            }
        }
        public async Task DeleteAsync(string uuid) 
        {
            _logger.LogInformation("Repository: Delete Movie from Neo4j");
            await using var session = _driver.AsyncSession();

            try
            {
                await session.ExecuteWriteAsync(async tx =>
                {
                    await tx.RunAsync($@"
                        MATCH (c:{NeoLabels.Franchise} {{{NeoProp.Franchise.Uuid}: ${NeoProp.Franchise.Uuid}}})
                        DETACH DELETE c
                    ", new { uuid });
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Repository: Error Deleting Franchise from Neo4j");
                throw;
            }
        }

        // ----- Helpers ----- //

        private FranchiseDto MapRecordToFranchiseDto(IRecord record)
        {

            var uuid = record[NeoProp.Franchise.Uuid].As<string>();
            var name = record[NeoProp.Franchise.Name].As<string>();

            _logger.LogDebug("Mapping record. Keys available: {Keys}", string.Join(", ", record.Keys));

            return new FranchiseDto
            {
                Uuid = uuid,
                Name = name
            };
        }
    }
}

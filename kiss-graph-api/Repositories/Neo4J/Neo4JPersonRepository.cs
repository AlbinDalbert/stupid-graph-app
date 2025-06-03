using Neo4j.Driver;
using kiss_graph_api.DTOs;
using kiss_graph_api.Repositories.Interfaces;
using kiss_graph_api.Domain.Enums;
using kiss_graph_api.Constants;
using System.Text;

namespace kiss_graph_api.Repositories.Neo4j
{
    public class Neo4JPersonRepository : IPersonRepository
    {

        private readonly IDriver _driver;
        private readonly ILogger<Neo4JMovieRepository> _logger;
        public Neo4JPersonRepository(IDriver driver, ILogger<Neo4JMovieRepository> logger)
        {
            _driver = driver;
            _logger = logger;
        }

        public async Task<IEnumerable<PersonDto>> GetAllAsync()
        {
            _logger.LogInformation("Repository: Getting all Persons from Neo4j");
            await using var session = _driver.AsyncSession();

            try
            {
                var works = await session.ExecuteReadAsync(async tx =>
                {
                    var cursor = await tx.RunAsync($@"
                        MATCH (c:{NeoLabels.Person})
                        RETURN  c.{NeoProp.Person.Uuid} AS {NeoProp.Person.Uuid}, 
                                c.{NeoProp.Person.Name} AS {NeoProp.Person.Name}, 
                                c.{NeoProp.Person.BornDate} AS {NeoProp.Person.BornDate}
                        ORDER BY c.{NeoProp.Person.Name}
                    ");

                    return await cursor.ToListAsync(record => MapRecordToPersonDto(record));

                });

                return works;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Repository: Error getting all Persons from Neo4j");
                throw;
            }
        }

        public async Task<PersonDto?> GetByUuidAsync(string uuid)
        {
            _logger.LogInformation("Repository: Getting Person by uuid from Neo4j");
            await using var session = _driver.AsyncSession();

            try
            {
                var creativeWork = await session.ExecuteReadAsync(async tx =>
                {
                    var cursor = await tx.RunAsync($@"
                        MATCH (c:{NeoLabels.Person})
                        WHERE c.{NeoProp.Person.Uuid} = ${NeoProp.Person.Uuid}
                        RETURN  c.{NeoProp.Person.Uuid} AS {NeoProp.Person.Uuid}, 
                                c.{NeoProp.Person.Name} AS {NeoProp.Person.Name}, 
                                c.{NeoProp.Person.BornDate} AS {NeoProp.Person.BornDate}
                    ", new { uuid });

                    var results = await cursor.ToListAsync(record => MapRecordToPersonDto(record));
                    return results.FirstOrDefault();
                });

                if (creativeWork == null)
                {
                    _logger.LogWarning($"Person with ID: {uuid} not found");
                    return null;
                }
                return creativeWork;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Repository: Error getting Person by uuid from Neo4j");
                throw;
            }
        }
        public async Task<PersonDto> CreateAsync(CreatePersonDto person)
        {
            _logger.LogInformation("Repository: Add Person to Neo4j");
            await using var session = _driver.AsyncSession();

            LocalDate? neo4jDate = person.BornDate is not null
                ? new LocalDate(
                    person.BornDate.Value.Year,
                    person.BornDate.Value.Month,
                    person.BornDate.Value.Day)
                : null;

            Guid guid = Guid.NewGuid();
            string uuid = guid.ToString();

            var query = $@"
            CREATE (c:{NeoLabels.Person} {{
                {NeoProp.Person.Uuid}: ${NeoProp.Person.Uuid},
                {NeoProp.Person.Name}: ${NeoProp.Person.Name},
                {NeoProp.Person.BornDate}: ${NeoProp.Person.BornDate}
            }})
            RETURN  c.{NeoProp.Person.Uuid} AS {NeoProp.Person.Uuid}, 
                    c.{NeoProp.Person.Name} AS {NeoProp.Person.Name}, 
                    c.{NeoProp.Person.BornDate} AS {NeoProp.Person.BornDate}
            ";

            var parameters = new Dictionary<string, object?>
            {
                { NeoProp.Person.Uuid, uuid },
                { NeoProp.Person.Name, person.Name },
                { NeoProp.Person.BornDate, neo4jDate }
            };

            try
            {
                var createWork = await session.ExecuteWriteAsync(async tx =>
                {
                    var cursor = await tx.RunAsync(query, parameters);
                    var record = await cursor.SingleAsync();
                    return MapRecordToPersonDto(record);
                });
                return createWork;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Repository: Error adding Person to Neo4j");
                throw;
            }
        }
        public async Task<PersonDto?> UpdateAsync(string uuid, UpdatePersonDto personDto)
        {
            _logger.LogInformation("Repository: Updating Person {Uuid}", uuid);
            await using var session = _driver.AsyncSession();

            var setClauses = new List<string>();
            var parameters = new Dictionary<string, object?>
            {
                { NeoProp.Person.Uuid, uuid }
            };

            if (personDto.Name != null)
            {
                setClauses.Add($"c.{NeoProp.Person.Name} = ${NeoProp.Person.Name}");
                parameters.Add(NeoProp.Person.Name, personDto.Name);
            }

            if (personDto.BornDate.HasValue)
            {
                setClauses.Add($"c.{NeoProp.Person.BornDate} = ${NeoProp.Person.BornDate}");
                parameters.Add(NeoProp.Person.BornDate, new LocalDate(
                    personDto.BornDate.Value.Year,
                    personDto.BornDate.Value.Month,
                    personDto.BornDate.Value.Day));
            }

            if (!setClauses.Any())
            {
                _logger.LogInformation("Repository: No properties provided to update for Person {Uuid}. Fetching current.", uuid);
                return await GetByUuidAsync(uuid);
            }

            var queryBuilder = new StringBuilder();
            queryBuilder.Append($"MATCH (c:{NeoLabels.Person} {{{NeoProp.Person.Uuid}: ${NeoProp.Person.Uuid}}}) ");
            queryBuilder.Append("SET ");
            queryBuilder.Append(string.Join(", ", setClauses));
            queryBuilder.Append($@"
            RETURN  c.{NeoProp.Person.Uuid} AS {NeoProp.Person.Uuid}, 
                    c.{NeoProp.Person.Name} AS {NeoProp.Person.Name}, 
                    c.{NeoProp.Person.BornDate} AS {NeoProp.Person.BornDate}
            ");

            try
            {
                var updatedWork = await session.ExecuteWriteAsync(async tx =>
                {
                    var cursor = await tx.RunAsync(queryBuilder.ToString(), parameters);
                    var record = await cursor.SingleAsync();
                    return MapRecordToPersonDto(record);
                });
                return updatedWork;
            }
            catch (InvalidOperationException ex) when (ex.Message.ToLowerInvariant().Contains("sequence contains no elements"))
            {
                _logger.LogWarning(ex, "Repository: Person {Uuid} not found for update.", uuid);
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Repository: Error updating Person {Uuid}", uuid);
                throw;
            }
        }
        public async Task DeleteAsync(string uuid)
        {
            _logger.LogInformation("Repository: Delete Person from Neo4j");
            await using var session = _driver.AsyncSession();

            try
            {
                await session.ExecuteWriteAsync(async tx =>
                {
                    await tx.RunAsync($@"
                        MATCH (c:{NeoLabels.Person} {{{NeoProp.Person.Uuid}: ${NeoProp.Person.Uuid}}})
                        DETACH DELETE c
                    ", new { uuid });
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Repository: Error deleting Person from Neo4j");
                throw;
            }
        }


        public async Task<IEnumerable<ActedInSummaryDto>> GetAllActedInAsync(string uuid)
        {
            await using var session = _driver.AsyncSession(); // Assuming _driver is your injected IDriver
            return await session.ExecuteReadAsync(async tx =>
            {
                var parameters = new Dictionary<string, object?>
        {
            { "personUuidParam", uuid }
        };

                // Construct the Cypher query using your NeoProp constants
                var query = $@"
            MATCH (p:{NeoLabels.Person})-[r:{NeoLabels.ActedIn}]->(cw:{NeoLabels.CreativeWork})
            WHERE p.{NeoProp.Person.Uuid} = $personUuidParam
            RETURN 
                p.{NeoProp.Person.Name} AS personName,
                p.{NeoProp.Person.Uuid} AS personUuidString, // Use personUuid from input or return from DB
                cw.{NeoProp.CreativeWork.Title} AS creativeWorkTitle,
                cw.{NeoProp.CreativeWork.Uuid} AS creativeWorkUuidString,
                cw.{NeoProp.CreativeWork.Type} AS creativeWorkType,
                r.{NeoProp.ActedIn.RoleType} AS roleString // Get the role as a string
            ORDER BY cw.{NeoProp.CreativeWork.ReleaseDate} DESC, creativeWorkTitle";

                var cursor = await tx.RunAsync(query, parameters);

                return await cursor.ToListAsync(record =>
                {
                    ActingRoleType? role = null;
                    var roleStringFromDb = record["roleString"]?.As<string>();
                    if (!string.IsNullOrEmpty(roleStringFromDb))
                    {
                        if (Enum.TryParse<ActingRoleType>(roleStringFromDb, true, out var parsedRole))
                        {
                            role = parsedRole;
                        }
                        else
                        {
                            _logger.LogWarning($"Could not parse role string '{roleStringFromDb}' to ActingRoleType.");
                        }
                    }

                    return new ActedInSummaryDto
                    {
                        Role = role,
                        PersonName = record["personName"].As<string>(),
                        PersonUuid = record["personUuidString"].As<string>(), // Or just use the input personUuid
                        CreativeWorkTitle = record["creativeWorkTitle"].As<string>(),
                        CreativeWorkUuid = record["creativeWorkUuidString"].As<string>(),
                        CreativeWorkType = record["creativeWorkType"].As<string>()
                    };
                });
            });
        }
        // ----- Helpers ----- //

        private PersonDto MapRecordToPersonDto(IRecord record)
        {

            var uuid = record[NeoProp.Person.Uuid].As<string>();
            var name = record[NeoProp.Person.Name].As<string>();

            DateOnly? bornDate = null;

            _logger.LogDebug("Mapping record. Keys available: {Keys}", string.Join(", ", record.Keys)); // Log all keys

            if (record.Keys.Contains(NeoProp.Person.BornDate))
            {
                var bornDateObject = record[NeoProp.Person.BornDate];

                _logger.LogInformation("Found 'BornDate' key. Value: [{Value}], Type: [{Type}]",
                    bornDateObject?.ToString() ?? "NULL",
                    bornDateObject?.GetType().FullName ?? "NULL");

                if (bornDateObject != null)
                {
                    try
                    {
                        bornDate = bornDateObject.As<LocalDate>().ToDateOnly();
                        _logger.LogInformation("Successfully parsed BornDate to: {ParsedDate}", bornDate);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "*** FAILED to parse BornDate. Original Value: {Value}, Type: {Type} ***",
                            bornDateObject,
                            bornDateObject.GetType().FullName);
                        bornDate = null;
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

            return new PersonDto
            {
                Uuid = uuid,
                Name = name,
                BornDate = bornDate
            };
        }
    }
}

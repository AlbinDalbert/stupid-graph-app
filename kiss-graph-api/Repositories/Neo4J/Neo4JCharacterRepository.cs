using Neo4j.Driver;
using kiss_graph_api.DTOs;
using kiss_graph_api.Repositories.Interfaces;
using kiss_graph_api.Domain.Enums;
using kiss_graph_api.Constants;
using System.Text;

namespace kiss_graph_api.Repositories.Neo4j
{
    public class Neo4JCharacterRepository : ICharacterRepository
    {
        private readonly IDriver _driver;
        private readonly ILogger<Neo4JCharacterRepository> _logger;
        public Neo4JCharacterRepository(IDriver driver, ILogger<Neo4JCharacterRepository> logger)
        {
            _driver = driver;
            _logger = logger;
        }

        public async Task<CharacterDto> CreateAsync(CreateCharacterDto character)
        {
            _logger.LogInformation("Repository: Add Character to Neo4j");
            await using var session = _driver.AsyncSession();

            Guid guid = Guid.NewGuid();
            string uuid = guid.ToString();

            var query = $@"
            CREATE (c:{NeoLabels.Character} {{
                {NeoProp.Character.Uuid}: ${NeoProp.Character.Uuid},
                {NeoProp.Character.Name}: ${NeoProp.Character.Name},
                {NeoProp.Character.Gender}: ${NeoProp.Character.Gender}
            }})
            RETURN  c.{NeoProp.Character.Uuid} AS {NeoProp.Character.Uuid}, 
                    c.{NeoProp.Character.Name} AS {NeoProp.Character.Name},
                    c.{NeoProp.Character.Gender} AS {NeoProp.Character.Gender}
            ";

            var parameters = new Dictionary<string, object?>
            {
                { NeoProp.Character.Uuid, uuid },
                { NeoProp.Character.Name, character.Name },
                { NeoProp.Character.Gender, character.Gender.ToString() },
            };

            try
            {
                var characterRet = await session.ExecuteWriteAsync(async tx =>
                {
                    var cursor = await tx.RunAsync(query, parameters);
                    var record = await cursor.SingleAsync();
                    return MapRecordToCharacterDto(record);
                });
                return characterRet;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Repository: Error adding Character to Neo4j");
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
                        MATCH (c:{NeoLabels.Character} {{{NeoProp.Character.Uuid}: ${NeoProp.Character.Uuid}}})
                        DETACH DELETE c
                    ", new { uuid });
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Repository: Error deleting Character from Neo4j");
                throw;
            }
        }

        public async Task<IEnumerable<CharacterDto>> GetAllAsync()
        {
            _logger.LogInformation("Repository: Getting all Characters from Neo4j");
            await using var session = _driver.AsyncSession();

            try
            {
                var works = await session.ExecuteReadAsync(async tx =>
                {
                    var cursor = await tx.RunAsync($@"
                        MATCH (c:{NeoLabels.Character})
                        RETURN  c.{NeoProp.Character.Uuid} AS {NeoProp.Character.Uuid}, 
                                c.{NeoProp.Character.Name} AS {NeoProp.Character.Name},
                                c.{NeoProp.Character.Gender} AS {NeoProp.Character.Gender}
                        ORDER BY c.{NeoProp.Character.Name}
                    ");

                    return await cursor.ToListAsync(record => MapRecordToCharacterDto(record));

                });

                return works;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Repository: Error getting all Characters from Neo4j");
                throw;
            }
        }

        public async Task<CharacterDto?> GetByUuidAsync(string uuid)
        {
            _logger.LogInformation("Repository: Getting Character by uuid from Neo4j");
            await using var session = _driver.AsyncSession();

            try
            {
                var character = await session.ExecuteReadAsync(async tx =>
                {
                    var cursor = await tx.RunAsync($@"
                        MATCH (c:{NeoLabels.Character})
                        WHERE c.{NeoProp.Character.Uuid} = ${NeoProp.Character.Uuid}
                        RETURN  c.{NeoProp.Character.Uuid} AS {NeoProp.Character.Uuid}, 
                                c.{NeoProp.Character.Name} AS {NeoProp.Character.Name},
                                c.{NeoProp.Character.Gender} AS {NeoProp.Character.Gender}
                    ", new { uuid });

                    var results = await cursor.ToListAsync(record => MapRecordToCharacterDto(record));
                    return results.FirstOrDefault();
                });

                if (character == null)
                {
                    _logger.LogWarning($"Character with ID: {uuid} not found");
                    return null;
                }
                return character;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Repository: Error getting Character by uuid from Neo4j");
                throw;
            }
        }

        public async Task<CharacterDto?> UpdateAsync(string uuid, UpdateCharacterDto characterDto)
        {
            _logger.LogInformation("Repository: Updating Movie {Uuid}", uuid);
            await using var session = _driver.AsyncSession();

            var setClauses = new List<string>();
            var parameters = new Dictionary<string, object?>
            {
                { NeoProp.Character.Uuid, uuid }
            };

            if (characterDto.Name != null)
            {
                setClauses.Add($"c.{NeoProp.Character.Name} = ${NeoProp.Character.Name}");
                parameters.Add(NeoProp.Character.Name, characterDto.Name);
            }

            if (characterDto.Gender != null)
            {
                setClauses.Add($"c.{NeoProp.Character.Gender} = ${NeoProp.Character.Gender}");
                parameters.Add(NeoProp.Character.Gender, characterDto.Gender);
            }

            if (!setClauses.Any())
            {
                _logger.LogInformation("Repository: No properties provided to update for Character {Uuid}. Fetching current.", uuid);
                return await GetByUuidAsync(uuid);
            }

            var queryBuilder = new StringBuilder();
            queryBuilder.Append($"MATCH (c:{NeoLabels.Character} {{{NeoProp.Character.Uuid}: ${NeoProp.Character.Uuid}}}) ");
            queryBuilder.Append("SET ");
            queryBuilder.Append(string.Join(", ", setClauses));
            queryBuilder.Append($@"
                RETURN c.{NeoProp.Character.Uuid} AS {NeoProp.Character.Uuid},
                        c.{NeoProp.Character.Name} AS {NeoProp.Character.Name},
                        c.{NeoProp.Character.Gender} AS {NeoProp.Character.Gender}
            ");

            try
            {
                var updateCharacter = await session.ExecuteWriteAsync(async tx =>
                {
                    var cursor = await tx.RunAsync(queryBuilder.ToString(), parameters);
                    var record = await cursor.SingleAsync();
                    return MapRecordToCharacterDto(record);
                });
                return updateCharacter;
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

        // --- HELPERS --- //

        private CharacterDto MapRecordToCharacterDto(IRecord record)
        {

            var uuid = record[NeoProp.Character.Uuid].As<string>();
            var name = record[NeoProp.Character.Name].As<string>();
            var gender = record[NeoProp.Character.Gender].As<string>();

            _logger.LogDebug("Mapping record. Keys available: {Keys}", string.Join(", ", record.Keys));

            return new CharacterDto
            {
                Uuid = uuid,
                Name = name,
                Gender = ParseGender(gender)
            };
        }

        private Gender? ParseGender(object typeObject)
        {
            Gender? genderEnumVal = null;

            if (typeObject != null && typeObject is string typeString && !string.IsNullOrEmpty(typeString))
            {
                try
                {
                    genderEnumVal = EnumHelper.ParseEnumMemberValue<Gender>(typeString);
                }
                catch (ArgumentException ex)
                {
                    _logger.LogWarning(ex, "An unknown CreativeWorkType '{TypeString}' was found. Defaulting to 'Other'.", typeString);
                }
            }
            else if (typeObject != null)
            {
                _logger.LogWarning("Gender 'type' was found but was not a string: {TypeValue}. Defaulting to 'null'.", typeObject);
            }

            return genderEnumVal;
        }
    }
}

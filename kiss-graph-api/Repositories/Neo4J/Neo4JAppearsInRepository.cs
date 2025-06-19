using Neo4j.Driver;
using kiss_graph_api.DTOs;
using kiss_graph_api.Repositories.Interfaces;
using kiss_graph_api.Domain.Enums;
using kiss_graph_api.Constants;
using System.Text;
using kiss_graph_api.Domain;
using System.Data;
using static kiss_graph_api.Constants.NeoProp;
using System;

namespace kiss_graph_api.Repositories.Neo4j
{
    public class Neo4JAppearsInRepository : IAppearsInRepository
    {

        private readonly IDriver _driver;
        private readonly ILogger<Neo4JActedInRepository> _logger;
        public Neo4JAppearsInRepository(IDriver driver, ILogger<Neo4JActedInRepository> logger)
        {
            _driver = driver;
            _logger = logger;
        }

        async Task<AppearsInSummaryDto> IAppearsInRepository.CreateLink(string characterUuid, string creativeWorkUuid, AppearsInDto dto)
        {
            await using var session = _driver.AsyncSession();
            AppearsInSummaryDto result = await session.ExecuteWriteAsync(async tx =>
            {

                var parameters = new Dictionary<string, object?>
                {
                    { "cUuid", characterUuid} ,
                    { "cwUuid", creativeWorkUuid} ,
                    { "typeValue", dto.CharacterType?.ToString()}
                };

                var query = $@"
                    MATCH (c:{NeoLabels.Character}), (cw:{NeoLabels.CreativeWork})
                    WHERE c.{NeoProp.Character.Uuid} = $cUuid AND cw.{NeoProp.CreativeWork.Uuid} = $cwUuid
                    MERGE (c)-[r:{NeoLabels.AppearsIn}]->(cw)
                    ON CREATE SET r.{NeoProp.AppearsIn.CharacterType} = $typeValue
                    ON MATCH SET r.{NeoProp.AppearsIn.CharacterType} = $typeValue
                    RETURN c.{NeoProp.Character.Name} AS CharacterName, 
                        c.{NeoProp.Character.Uuid} AS CharacterUuid,
                        cw.{NeoProp.CreativeWork.Title} AS CreativeWorkTitle,
                        cw.{NeoProp.CreativeWork.Uuid} AS CreativeWorkUuid,
                        cw.{NeoProp.CreativeWork.Type} AS CreativeWorkType,
                        r.{NeoProp.AppearsIn.CharacterType} AS Type";
                var cursor = await tx.RunAsync(query, parameters);
                var record = await cursor.SingleAsync();

                string? typeString = record["Type"]?.As<string>();
                CharacterType? characterType = null;

                if (!string.IsNullOrEmpty(typeString))
                {
                    if (Enum.TryParse<CharacterType>(typeString, true, out CharacterType parsedType))
                    {
                        characterType = parsedType;
                    }
                    else
                    {
                        _logger.LogWarning("Could not parse role string '{RoleString}' from database to CharacterType enum.", typeString);
                    }
                }

                return new AppearsInSummaryDto
                {
                    CharacterName = record["CharacterName"].As<string>(),
                    CharacterUuid = record["CharacterUuid"].As<string>(), 
                    CreativeWorkTitle = record["CreativeWorkTitle"].As<string>(),
                    CreativeWorkUuid = record["CreativeWorkUuid"].As<string>(),
                    CreativeWorkType = record["CreativeWorkType"].As<string>()
                };
            });
            return result;
        }


        async Task IAppearsInRepository.DeleteLink(string characterUuid, string creativeWorkUuid)
        {
            await using var session = _driver.AsyncSession();
            await session.ExecuteWriteAsync(async tx =>
            {
                var parameters = new Dictionary<string, object?>
                {
                    { "cUuid", characterUuid} ,
                    { "cwUuid", creativeWorkUuid}
                };

                var query = $@"
                    MATCH (p:{NeoLabels.Character} {{uuid: $cUuid}})-[r:{NeoLabels.AppearsIn}]->(cw:{NeoLabels.CreativeWork} {{uuid: $cwUuid}})
                    DELETE r";
                await tx.RunAsync(query, parameters);
            });
        }


        // ----- Helpers ----- //
    }
}

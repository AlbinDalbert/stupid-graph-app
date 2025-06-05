using Neo4j.Driver;
using kiss_graph_api.DTOs;
using kiss_graph_api.Repositories.Interfaces;
using kiss_graph_api.Domain.Enums;
using kiss_graph_api.Constants;

namespace kiss_graph_api.Repositories.Neo4j
{
    public class Neo4JPortrayedRepository : IPortrayedRepository
    {

        private readonly IDriver _driver;
        private readonly ILogger<Neo4JPortrayedRepository> _logger;
        public Neo4JPortrayedRepository(IDriver driver, ILogger<Neo4JPortrayedRepository> logger)
        {
            _driver = driver;
            _logger = logger;
        }

        public async Task<PortrayedSummaryDto> CreateLink(string personUuid, string characterUuid, PortrayedDto dto)
        {
            
            await using var session = _driver.AsyncSession();
            PortrayedSummaryDto result = await session.ExecuteWriteAsync(async tx =>
            {

                var parameters = new Dictionary<string, object?>
                {
                    { "pUuid", personUuid} ,
                    { "cUuid", characterUuid}
                };

                var query = $@"
                    MATCH (p:{NeoLabels.Person}), (c:{NeoLabels.Character})
                    WHERE p.{NeoProp.Person.Uuid} = $pUuid AND c.{NeoProp.Character.Uuid} = $cUuid
                    MERGE (p)-[r:{NeoLabels.Portrayed}]->(c)
                    RETURN p.{NeoProp.Person.Name} AS PersonName, 
                        p.{NeoProp.Person.Uuid} AS PersonUuid,
                        c.{NeoProp.Character.Name} AS CharacterName,
                        c.{NeoProp.Character.Uuid} AS CharacterUuid";

                var cursor = await tx.RunAsync(query, parameters);
                var record = await cursor.SingleAsync();

                return new PortrayedSummaryDto
                {
                    PersonName = record["PersonName"].As<string>(),
                    PersonUuid = record["PersonUuid"].As<string>(), 
                    CharacterName = record["CharacterName"].As<string>(),
                    CharacterUuid = record["CharacterUuid"].As<string>()
                };
            });
            return result;
        }

        async Task DeleteLink(string personUuid, string characterUuid)
        {
            await using var session = _driver.AsyncSession();
            await session.ExecuteWriteAsync(async tx =>
            {
                var parameters = new Dictionary<string, object?>
                {
                    { "pUuid", personUuid} ,
                    { "cUuid", characterUuid}
                };

                var query = $@"
                    MATCH (p:{NeoLabels.Person} {{uuid: $pUuid}})-[r:{NeoLabels.Portrayed}]->(cw:{NeoLabels.Character} {{uuid: $cUuid}})
                    DELETE r";
                await tx.RunAsync(query, parameters);
            });
        }

        Task IPortrayedRepository.DeleteLink(string personUuid, string characterUuid)
        {
            return DeleteLink(personUuid, characterUuid);
        }


        // ----- Helpers ----- //
    }
}

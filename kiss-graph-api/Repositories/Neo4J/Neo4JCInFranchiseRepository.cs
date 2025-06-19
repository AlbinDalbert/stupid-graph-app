using Neo4j.Driver;
using kiss_graph_api.DTOs;
using kiss_graph_api.Repositories.Interfaces;
using kiss_graph_api.Constants;

namespace kiss_graph_api.Repositories.Neo4j
{
    public class Neo4JCInFranchiseRepository : ICInFranchiseRepository
    {

        private readonly IDriver _driver;
        private readonly ILogger<Neo4JCInFranchiseRepository> _logger;
        public Neo4JCInFranchiseRepository(IDriver driver, ILogger<Neo4JCInFranchiseRepository> logger)
        {
            _driver = driver;
            _logger = logger;
        }

        async Task<CInFranchiseSummaryDto> ICInFranchiseRepository.CreateLink(string characterUuid, string franchiseUuid)
        {
            await using var session = _driver.AsyncSession();
            CInFranchiseSummaryDto result = await session.ExecuteWriteAsync(async tx =>
            {

                var parameters = new Dictionary<string, object?>
                {
                    { "cUuid", characterUuid} ,
                    { "fUuid", franchiseUuid}
                };

                var query = $@"
                    MATCH (c:{NeoLabels.Character}), (f:{NeoLabels.Franchise})
                    WHERE c.{NeoProp.Character.Uuid} = $cUuid AND f.{NeoProp.Franchise.Uuid} = $fUuid
                    MERGE (c)-[r:{NeoLabels.InFranchise}]->(f)
                    RETURN c.{NeoProp.Character.Name} AS CharacterName, 
                        c.{NeoProp.Character.Uuid} AS CharacterUuid,
                        f.{NeoProp.Franchise.Name} AS FranchiseName,
                        f.{NeoProp.Franchise.Uuid} AS FranchiseUuid";
                var cursor = await tx.RunAsync(query, parameters);
                var record = await cursor.SingleAsync();

                return new CInFranchiseSummaryDto
                {
                    CharacterName = record["CharacterName"].As<string>(),
                    CharacterUuid = record["CharacterUuid"].As<string>(),
                    FranchiseName = record["FranchiseName"].As<string>(),
                    FranchiseUuid = record["FranchiseUuid"].As<string>()
                };
            });
            return result;
        }


        async Task ICInFranchiseRepository.DeleteLink(string characterUuid, string franchiseUuid)
        {
            await using var session = _driver.AsyncSession();
            await session.ExecuteWriteAsync(async tx =>
            {
                var parameters = new Dictionary<string, object?>
                {
                    { "cUuid", characterUuid} ,
                    { "fUuid", franchiseUuid}
                };

                var query = $@"
                    MATCH (p:{NeoLabels.Character} {{uuid: $cUuid}})-[r:{NeoLabels.InFranchise}]->(c:{NeoLabels.Franchise} {{uuid: $fUuid}})
                    DELETE r";
                await tx.RunAsync(query, parameters);
            });
        }


        // ----- Helpers ----- //
    }
}

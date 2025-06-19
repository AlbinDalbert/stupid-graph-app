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
    public class Neo4JCWInFranchiseRepository : ICWInFranchiseRepository
    {

        private readonly IDriver _driver;
        private readonly ILogger<Neo4JCWInFranchiseRepository> _logger;
        public Neo4JCWInFranchiseRepository(IDriver driver, ILogger<Neo4JCWInFranchiseRepository> logger)
        {
            _driver = driver;
            _logger = logger;
        }

        async Task<CWInFranchiseSummaryDto> ICWInFranchiseRepository.CreateLink(string creativeWorkUuid, string franchiseUuid)
        {
            await using var session = _driver.AsyncSession();
            CWInFranchiseSummaryDto result = await session.ExecuteWriteAsync(async tx =>
            {

                var parameters = new Dictionary<string, object?>
                {
                    { "cwUuid", creativeWorkUuid} ,
                    { "fUuid", franchiseUuid}
                };

                var query = $@"
                    MATCH (cw:{NeoLabels.CreativeWork}), (f:{NeoLabels.Franchise})
                    WHERE cw.{NeoProp.CreativeWork.Uuid} = $cwUuid AND f.{NeoProp.Franchise.Uuid} = $fUuid
                    MERGE (cw)-[r:{NeoLabels.InFranchise}]->(f)
                    RETURN cw.{NeoProp.CreativeWork.Title} AS CreativeWorkTitle, 
                        cw.{NeoProp.CreativeWork.Uuid} AS CreativeWorkUuid,
                        f.{NeoProp.Franchise.Name} AS FranchiseName,
                        f.{NeoProp.Franchise.Uuid} AS FranchiseUuid";
                var cursor = await tx.RunAsync(query, parameters);
                var record = await cursor.SingleAsync();

                return new CWInFranchiseSummaryDto
                {
                    CreativeWorkTitle = record["CreativeWorkTitle"].As<string>(),
                    CreativeWorkUuid = record["CreativeWorkUuid"].As<string>(),
                    FranchiseName = record["FranchiseName"].As<string>(),
                    FranchiseUuid = record["FranchiseUuid"].As<string>()
                };
            });
            return result;
        }


        async Task ICWInFranchiseRepository.DeleteLink(string creativeWorkUuid, string franchiseUuid)
        {
            await using var session = _driver.AsyncSession();
            await session.ExecuteWriteAsync(async tx =>
            {
                var parameters = new Dictionary<string, object?>
                {
                    { "cwUuid", creativeWorkUuid} ,
                    { "fUuid", franchiseUuid}
                };

                var query = $@"
                    MATCH (p:{NeoLabels.CreativeWork} {{uuid: $cwUuid}})-[r:{NeoLabels.InFranchise}]->(cw:{NeoLabels.Franchise} {{uuid: $fUuid}})
                    DELETE r";
                await tx.RunAsync(query, parameters);
            });
        }


        // ----- Helpers ----- //
    }
}

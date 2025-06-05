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
    public class Neo4JActedInRepository : IActedInRepository
    {

        private readonly IDriver _driver;
        private readonly ILogger<Neo4JActedInRepository> _logger;
        public Neo4JActedInRepository(IDriver driver, ILogger<Neo4JActedInRepository> logger)
        {
            _driver = driver;
            _logger = logger;
        }

        async Task<ActedInSummaryDto> IActedInRepository.CreateLink(string personUuid, string creativeWorkUuid, ActedInDto dto)
        {
            await using var session = _driver.AsyncSession();
            ActedInSummaryDto result = await session.ExecuteWriteAsync(async tx =>
            {

                var parameters = new Dictionary<string, object?>
                {
                    { "pUuid", personUuid} ,
                    { "cwUuid", creativeWorkUuid} ,
                    { "roleValue", dto.Role?.ToString()}
                };

                var query = $@"
                    MATCH (p:{NeoLabels.Person}), (cw:{NeoLabels.CreativeWork})
                    WHERE p.{NeoProp.Person.Uuid} = $pUuid AND cw.{NeoProp.CreativeWork.Uuid} = $cwUuid
                    MERGE (p)-[r:{NeoLabels.ActedIn}]->(cw)
                    ON CREATE SET r.{NeoProp.ActedIn.RoleType} = $roleValue
                    ON MATCH SET r.{NeoProp.ActedIn.RoleType} = $roleValue
                    RETURN p.{NeoProp.Person.Name} AS PersonName, 
                        p.{NeoProp.Person.Uuid} AS PersonUuid,
                        cw.{NeoProp.CreativeWork.Title} AS CreativeWorkTitle,
                        cw.{NeoProp.CreativeWork.Uuid} AS CreativeWorkUuid,
                        cw.{NeoProp.CreativeWork.Type} AS CreativeWorkType,
                        r.{NeoProp.ActedIn.RoleType} AS Role";
                var cursor = await tx.RunAsync(query, parameters);
                var record = await cursor.SingleAsync();

                string? roleString = record["Role"]?.As<string>();
                ActingRoleType? actingRole = null;

                if (!string.IsNullOrEmpty(roleString))
                {
                    if (Enum.TryParse<ActingRoleType>(roleString, true, out ActingRoleType parsedRole))
                    {
                        actingRole = parsedRole;
                    }
                    else
                    {
                        _logger.LogWarning("Could not parse role string '{RoleString}' from database to ActingRoleType enum.", roleString);
                    }
                }

                return new ActedInSummaryDto
                {
                    Role = actingRole,
                    PersonName = record["PersonName"].As<string>(),
                    PersonUuid = record["PersonUuid"].As<string>(), 
                    CreativeWorkTitle = record["CreativeWorkTitle"].As<string>(),
                    CreativeWorkUuid = record["CreativeWorkUuid"].As<string>(),
                    CreativeWorkType = record["CreativeWorkType"].As<string>()
                };
            });
            return result;
        }

        async Task IActedInRepository.DeleteLink(string personUuid, string movieUuid)
        {
            await using var session = _driver.AsyncSession();
            await session.ExecuteWriteAsync(async tx =>
            {
                var parameters = new Dictionary<string, object?>
                {
                    { "p"+NeoProp.Person.Uuid, personUuid} ,
                    { "cw"+NeoProp.Movie.Uuid, movieUuid}
                };

                var query = $@"
                    MATCH (p:{NeoLabels.Person} {{uuid: $p{NeoProp.Person.Uuid}}})-[r:{NeoLabels.ActedIn}]->(cw:{NeoLabels.CreativeWork} {{uuid: $cw{NeoProp.Movie.Uuid}}})
                    DELETE r";
                await tx.RunAsync(query, parameters);
            });
        }


        // ----- Helpers ----- //
    }
}

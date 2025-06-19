using Neo4j.Driver;
using kiss_graph_api.DTOs;
using kiss_graph_api.Repositories.Interfaces;
using kiss_graph_api.Constants;

namespace kiss_graph_api.Repositories.Neo4j
{
    public class Neo4JInGenreRepository : IInGenreRepository
    {

        private readonly IDriver _driver;
        private readonly ILogger<Neo4JInGenreRepository> _logger;
        public Neo4JInGenreRepository(IDriver driver, ILogger<Neo4JInGenreRepository> logger)
        {
            _driver = driver;
            _logger = logger;
        }

        async Task IInGenreRepository.CreateLink(string creativeWorkUuid, string genreUuid)
        {
            await using var session = _driver.AsyncSession();
            await session.ExecuteWriteAsync(async tx =>
            {

                var parameters = new Dictionary<string, object?>
                {
                    { "cwUuid", creativeWorkUuid} ,
                    { "gUuid", genreUuid}
                };

                var query = $@"
                    MATCH (cw:{NeoLabels.CreativeWork}), (g:{NeoLabels.Genre})
                    WHERE cw.{NeoProp.CreativeWork.Uuid} = $cwUuid AND g.{NeoProp.Genre.Uuid} = $gUuid
                    MERGE (cw)-[r:{NeoLabels.InGenre}]->(g)";
                var cursor = await tx.RunAsync(query, parameters);
                var record = await cursor.ToListAsync();
            });
        }

        async Task IInGenreRepository.DeleteLink(string creativeWorkUuid, string genreUuid)
        {
            await using var session = _driver.AsyncSession();
            await session.ExecuteWriteAsync(async tx =>
            {
                var parameters = new Dictionary<string, object?>
                {
                    { "cwUuid", creativeWorkUuid} ,
                    { "gUuid", genreUuid}
                };

                var query = $@"
                    MATCH (cw:{NeoLabels.CreativeWork} {{uuid: $cwUuid}})-[r:{NeoLabels.InGenre}]->(g:{NeoLabels.Genre} {{uuid: $gUuid}})
                    DELETE r";
                await tx.RunAsync(query, parameters);
            });
        }


        // ----- Helpers ----- //
    }
}

using kiss_graph_api.Domain;
using kiss_graph_api.Domain.Relationship;
using kiss_graph_api.Domain.Enums;
using Neo4j.Driver;

namespace kiss_graph_api.Services {
    public class GraphRelationshipService
    {
        private readonly IAsyncSession _session;

        public GraphRelationshipService(IAsyncSession session)
        {
            _session = session;
        }

        /// <summary>
        ///  ACTED_IN
        /// </summary>
        /// <param name="actor"></param>
        /// <param name="work"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public async Task LinkActorToWork(Person actor, CreativeWork work)
        {

            if (work.Type is CreativeWorkType.Movie or CreativeWorkType.Game)
            {
                var query = @"
                    MATCH (p:Person {id: $personId}), (w:CreativeWork {id: $workId})
                    MERGE (p)-[:ACTED_IN]->(w)";
                await _session.RunAsync(query, new { personId = actor.Uuid, workId = work.Uuid });
            }
            else
            {
                throw new InvalidOperationException($"Cannot link actor to work of type '{work.Type}'. Only movies and games are supported.");
            }
        }

        /// <summary>
        /// APPEARED_IN
        /// </summary>
        /// <param name="character"></param>
        /// <param name="work"></param>
        /// <returns></returns>
        public async Task LinkCharacterToWork(Character character, CreativeWork work)
        {
            var query = @"
                MATCH (c:Character {id: $characterId}), (w:CreativeWork {id: $workId})
                MERGE (c)-[:APPEARED_IN]->(w)";
            await _session.RunAsync(query, new { characterId = character.Uuid, workId = work.Uuid });
        }

        /// <summary>
        /// PORTRAYED
        /// </summary>
        /// <param name="person"></param>
        /// <param name="character"></param>
        /// <returns></returns>
        public async Task LinkActorToCharacter(Person person, Character character)
        {
            var query = @"
                MATCH (p:Person {id: $personId}), (c:Character {id: $characterId})
                MERGE (p)-[:PORTRAYED]->(c)";
            await _session.RunAsync(query, new { characterId = character.Uuid, perosnId = person.Uuid });
        }

        /// <summary>
        /// HAS_GENRE
        /// </summary>
        /// <param name="work"></param>
        /// <param name="genre"></param>
        /// <returns></returns>
        public async Task LinkCreativeWorkToGenre(CreativeWork work, Genre genre)
        {
            var query = @"
                MATCH (cw:CreativeWork {id: $workId}), (g:Genre {id: $genreId})
                MERGE (cw)-[:HAS_GENRE]->(g)";
            await _session.RunAsync(query, new { workId = work.Uuid, genreId = genre.Uuid });
        }

        /// <summary>
        /// IN_FRANCHISE
        /// </summary>
        /// <param name="work"></param>
        /// <param name="franchise"></param>
        /// <returns></returns>
        public async Task LinkCreativeWorkToFranchise(CreativeWork work, Franchise franchise)
        {
            var query = @"
                MATCH (cw:CreativeWork {id: $workId}), (f:Franchise {id: $franchiseId})
                MERGE (cw)-[:IN_FRANCHISE]->(f)";
            await _session.RunAsync(query, new { workId = work.Uuid, franchiseId = franchise.Uuid });
        }

        /// <summary>
        /// IN_FRANCHISE
        /// </summary>
        /// <param name="work"></param>
        /// <param name="franchise"></param>
        /// <returns></returns>
        public async Task LinkCreativeWorkToFranchise(Character character, Franchise franchise)
        {
            var query = @"
                MATCH (c:Character {id: $characterId}), (f:Franchise {id: $franchiseId})
                MERGE (c)-[:IN_FRANCHISE]->(f)";
            await _session.RunAsync(query, new { characterId = character.Uuid, franchiseId = franchise.Uuid });
        }
    }
}

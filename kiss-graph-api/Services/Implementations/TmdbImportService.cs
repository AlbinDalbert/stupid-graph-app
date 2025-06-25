using kiss_graph_api.Clients.Interfaces;
using kiss_graph_api.DTOs;
using kiss_graph_api.Repositories.Interfaces;
using kiss_graph_api.Services.Interfaces;

namespace kiss_graph_api.Services.Implementations
{
    public class TmdbImportService : ITmdbImportService
    {
        private readonly ITmdbClient _tmdbClient;
        private readonly IMovieRepository _movieRepo;
        private readonly IPersonRepository _personRepo;
        private readonly ICharacterRepository _characterRepo;
        private readonly IActedInRepository _actedInRepo;
        private readonly IPortrayedRepository _portrayedRepo;
        private readonly IAppearsInRepository _appearsInRepo;
        private readonly ILogger<TmdbImportService> _logger;

        public TmdbImportService(
            ITmdbClient tmdbClient,
            IMovieRepository movieRepo,
            IPersonRepository personRepo,
            ICharacterRepository characterRepo,
            IActedInRepository actedInRepo,
            IPortrayedRepository portrayedRepo,
            IAppearsInRepository appearsInRepo,
            ILogger<TmdbImportService> logger)
        {
            _tmdbClient = tmdbClient;
            _movieRepo = movieRepo;
            _personRepo = personRepo;
            _characterRepo = characterRepo;
            _actedInRepo = actedInRepo;
            _portrayedRepo = portrayedRepo;
            _appearsInRepo = appearsInRepo;
            _logger = logger;
        }

        public async Task ImportMoviesByIdsAsync(IEnumerable<int> movieIds)
        {
            _logger.LogInformation("Starting TMDB import process...");

            foreach (var movieId in movieIds)
            {
                try
                {
                    // Step 1: Get Movie Details from TMDB
                    var tmdbMovie = await _tmdbClient.GetMovieByIdAsync(movieId);
                    if (tmdbMovie == null)
                    {
                        _logger.LogWarning("Could not find movie with TMDB ID {MovieId}. Skipping.", movieId);
                        continue; // Skip to the next movie ID
                    }

                    // Step 2: Create/Update the :CreativeWork node in your DB
                    // You'll need to create a CreateOrUpdateFromTmdbAsync method in your repo that uses MERGE
                    var ourCreativeWork = await _movieRepo.CreateOrUpdateFromTmdbAsync(tmdbMovie);
                    _logger.LogInformation("Imported/Updated CreativeWork: {Title}", tmdbMovie.Title);

                    // Step 3: Get Credits from TMDB
                    //var tmdbCredits = await _tmdbClient.GetCreditsForMovieAsync(movieId);
                    //if (tmdbCredits == null) continue;

                    // Step 4: Process the Cast
                    // Taking top 15 actors to keep it manageable. Remove .Take(15) to get everyone.
                    //foreach (var castMember in tmdbCredits.Cast.Where(c => !string.IsNullOrEmpty(c.Character)).Take(15))
                    //{
                    //    // Create/Update the Person
                    //    var ourPerson = await _personRepo.CreateOrUpdateFromTmdbAsync(castMember.Id);
                    //    // Create/Update the Character
                    //    var ourCharacter = await _characterRepo.CreateOrUpdateByNameAsync(castMember.Character);

                    //    // Create the relationships
                    //    await _actedInRepo.CreateLink(ourPerson.Uuid, ourCreativeWork.Uuid, new ActedInDto { Role = /* Determine role, e.g., Supporting */ });
                    //    await _portrayedRepo.CreateLink(ourPerson.Uuid, ourCharacter.Uuid);
                    //    await _appearsInRepo.CreateLink(ourCharacter.Uuid, ourCreativeWork.Uuid);
                    //}

                    // Step 5: (To Do) Process Genres, Franchise, Directors (from tmdbCredits.Crew) in a similar way

                    _logger.LogInformation("Successfully processed TMDB ID {MovieId}", movieId);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "An error occurred while processing TMDB ID {MovieId}", movieId);
                    // Continue to the next movie even if one fails
                }
            }
            _logger.LogInformation("TMDB import process finished.");
        }
    }
}
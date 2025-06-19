using kiss_graph_api.DTOs;
using kiss_graph_api.Services.Interfaces;
using kiss_graph_api.Repositories.Interfaces;
using kiss_graph_api.Exceptions;

namespace kiss_graph_api.Services.Implementations
{
    public class MovieService : IMovieService
    {
        private readonly IMovieRepository _repository;
        private readonly ICWInFranchiseRepository _inFranchiseRepository;
        private readonly ICInFranchiseRepository _characterInFranchiseRepository;
        private readonly ICharacterRepository _characterRepository;
        private readonly IInGenreRepository _inGenreRepository;
        private readonly ILogger<MovieService> _logger;

        public MovieService(
            IMovieRepository repository,
            ICWInFranchiseRepository inFranchiseRepository, 
            ICInFranchiseRepository cInFranchiseRepository, 
            ICharacterRepository characterRepository,
            IInGenreRepository inGenreRepository,
            ILogger<MovieService> logger
        )
        {
            _repository = repository;
            _inFranchiseRepository = inFranchiseRepository;
            _characterInFranchiseRepository = cInFranchiseRepository;
            _characterRepository = characterRepository;
            _inGenreRepository = inGenreRepository;
            _logger = logger;   
        }
        public async Task<IEnumerable<MovieDto>> GetAllCreativeWorksAsync()
        {
            _logger.LogInformation("Service: Getting all Creative Works");
            return await _repository.GetAllAsync();
        }
        public async Task<MovieDto> GetCreativeWorkByUuidAsync(string uuid)
        {
            _logger.LogInformation("Service: Creative Works By Uuid");
            var work = await _repository.GetByUuidAsync(uuid);

            if (work == null)
            {
                throw new NotFoundException($"CreativeWork with UUID '{uuid}' not found.");
            }

            return work;
        }

        public async Task<MovieDto> CreateCreativeWorkAsync(CreateMovieDto movie)
        {
            _logger.LogInformation("Service: Add Creative Work");
            return await _repository.CreateAsync(movie);
        }

        public async Task<MovieDto> UpdateCreativeWorkAsync(string uuid, UpdateMovieDto movie)
        {
            _logger.LogInformation("Service: Update Creative Work");
            var work = await _repository.UpdateAsync(uuid, movie);

            if (work == null)
            {
                throw new NotFoundException($"CreativeWork with UUID '{uuid}' not found.");
            }

            return work;
        }

        public async Task DeleteCreativeWorkAsync(string uuid)
        {
            _logger.LogInformation("Service: Delete Creative Work");
            await _repository.DeleteAsync(uuid);
        }

        public async Task AssignToFranchise(string movieUuid, string franchiseUuid)
        {
            _logger.LogInformation("Service: Add Creative Work To Franchise");
            await _inFranchiseRepository.CreateLink(movieUuid, franchiseUuid);
            var characters = await _characterRepository.GetAllFromCreativeWorkAsync(movieUuid);
            _logger.LogInformation($"Service: Adding characters {characters.ToList().Count}");
            foreach (var character in characters) {
                if (character.Uuid == null) { continue; }
                await _characterInFranchiseRepository.CreateLink(character.Uuid, franchiseUuid);
            }
        }

        public async Task RemoveFromFranchise(string movieUuid, string franchiseUuid)
        {
            _logger.LogInformation("Service: Delete Creative Work From Franchise");
            await _inFranchiseRepository.DeleteLink(movieUuid, franchiseUuid);
        }

        public async Task AssignGenre(string movieUuid, string genreUuid)
        {
            _logger.LogInformation("Service: Assign Genre");
            await _inGenreRepository.CreateLink(movieUuid, genreUuid);
        }

        public async Task RemoveGenre(string movieUuid, string genreUuid)
        {
            _logger.LogInformation("Service: Remove Genre");
            await _inGenreRepository.DeleteLink(movieUuid, genreUuid);
        }
    }
}
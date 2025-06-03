using kiss_graph_api.DTOs;
using kiss_graph_api.Services.Interfaces;
using kiss_graph_api.Repositories.Interfaces;
using kiss_graph_api.Exceptions;

namespace kiss_graph_api.Services.Implementations
{
    public class MovieService : IMovieService
    {
        private readonly IMovieRepository _repository;
        private readonly ILogger<MovieService> _logger;

        public MovieService(IMovieRepository repository, ILogger<MovieService> logger)
        {
            _repository = repository;
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
    }
}
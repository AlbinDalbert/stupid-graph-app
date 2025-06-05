using kiss_graph_api.DTOs;
using kiss_graph_api.Repositories.Interfaces;
using kiss_graph_api.Services.Interfaces;

namespace kiss_graph_api.Services.Implementations
{
    public class GenreService : IGenreService
    {

        private readonly IGenreRepository _repository;
        private readonly ILogger<GenreService> _logger;

        public GenreService(IGenreRepository repository, ILogger<GenreService> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public async Task<GenreDto> CreateGenreAsync(CreateGenreDto dto)
        {
            _logger.LogInformation("Service: Create genre");
            return await _repository.CreateAsync(dto);
        }

        public async Task DeleteGenreAsync(string uuid)
        {
            _logger.LogInformation("Service: Delete genre");
            await _repository.DeleteAsync(uuid);
        }

        public async Task<IEnumerable<GenreDto>> GetAllGenreAsync()
        {
            _logger.LogInformation("Service: Getting all Genres");
            return await _repository.GetAllAsync();
        }

        public async Task<GenreDto?> GetGenreByUuidAsync(string uuid)
        {
            _logger.LogInformation("Servie: Get genre by uuid");
            return await _repository.GetByUuidAsync(uuid);
        }

        public async Task<GenreDto?> UpdateGenreAsync(string uuid, UpdateGenreDto dto)
        {
            _logger.LogInformation("Service: Update Genre");
            return await _repository.UpdateAsync(uuid, dto);
        }
    }
}
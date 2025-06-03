using kiss_graph_api.DTOs;
using kiss_graph_api.Repositories.Interfaces;
using kiss_graph_api.Services.Interfaces;

namespace kiss_graph_api.Services.Implementations
{
    public class CharacterService : ICharacterService
    {

        private readonly ICharacterRepository _repository;
        private readonly ILogger<CharacterService> _logger;

        public CharacterService(ICharacterRepository repository, ILogger<CharacterService> logger)
        {
            _repository = repository;
            _logger = logger;
        }
        public async Task<CharacterDto> CreateCharacterAsync(CreateCharacterDto characterDto)
        {
            _logger.LogInformation($"Service: Create Character: {characterDto.Name}");
            return await _repository.CreateAsync(characterDto);
        }

        public async Task DeletePersonAsync(string uuid)
        {
            _logger.LogInformation("Service: Deleting character");
            await _repository.DeleteAsync(uuid);
        }

        public async Task<IEnumerable<CharacterDto>> GetAllPersonAsync()
        {
            _logger.LogInformation("Service: get all characters");
            return await _repository.GetAllAsync();
        }

        public async Task<CharacterDto?> GetCharacterByUuidAsync(string uuid)
        {
            _logger.LogInformation("Service: Get character by uuid");
            return await _repository.GetByUuidAsync(uuid);
        }

        public async Task<CharacterDto> UpdateCharacterAsync(string uuid, UpdateCharacterDto characterDto)
        {
            _logger.LogInformation("Service: update character");
            return await _repository.UpdateAsync(uuid, characterDto);
        }
    }
}
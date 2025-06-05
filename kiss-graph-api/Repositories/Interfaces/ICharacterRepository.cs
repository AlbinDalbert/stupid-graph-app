using kiss_graph_api.DTOs;

namespace kiss_graph_api.Repositories.Interfaces
{
    public interface ICharacterRepository
    {
        Task<IEnumerable<CharacterDto>> GetAllAsync();
        Task<CharacterDto?> GetByUuidAsync(string uuid);
        Task<CharacterDto> CreateAsync(CreateCharacterDto characterDto);
        Task<CharacterDto?> UpdateAsync(string uuid, UpdateCharacterDto characterDto);
        Task DeleteAsync(string uuid);
    }
}
using kiss_graph_api.DTOs;

namespace kiss_graph_api.Repositories.Interfaces
{
    public interface IGenreRepository
    {
        Task<IEnumerable<GenreDto>> GetAllAsync();
        Task<GenreDto?> GetByUuidAsync(string uuid);
        Task<GenreDto> CreateAsync(CreateGenreDto dto);
        Task<GenreDto?> UpdateAsync(string uuid, UpdateGenreDto dto);
        Task DeleteAsync(string uuid);
    }
}
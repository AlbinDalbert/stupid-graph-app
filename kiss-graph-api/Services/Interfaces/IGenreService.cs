using kiss_graph_api.DTOs;

namespace kiss_graph_api.Services.Interfaces
{
    public interface IGenreService
    {
        // Basic creative node operations
        Task<IEnumerable<GenreDto>> GetAllGenreAsync();
        Task<GenreDto?> GetGenreByUuidAsync(string uuid);
        Task<GenreDto> CreateGenreAsync(CreateGenreDto dto);
        Task<GenreDto?> UpdateGenreAsync(string uuid, UpdateGenreDto dto);
        Task DeleteGenreAsync(string uuid);
    }
}
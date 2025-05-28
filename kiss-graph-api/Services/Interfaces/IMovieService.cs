using kiss_graph_api.DTOs;

namespace kiss_graph_api.Services.Interfaces
{
    public interface IMovieService
    {
        // Basic creative node operations
        Task<IEnumerable<MovieDto>> GetAllCreativeWorksAsync();
        Task<MovieDto> GetCreativeWorkByUuidAsync(string uuid);
        Task<MovieDto> CreateCreativeWorkAsync(CreateMovieDto movie);
        Task<MovieDto> UpdateCreativeWorkAsync(string uuid, UpdateMovieDto movie);
        Task DeleteCreativeWorkAsync(string uuid);
    }
}
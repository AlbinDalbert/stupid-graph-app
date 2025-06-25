using kiss_graph_api.DTOs;

namespace kiss_graph_api.Services.Interfaces
{
    public interface ITmdbImportService
    {
        Task ImportMoviesByIdsAsync(IEnumerable<int> movieIds);
    }
}
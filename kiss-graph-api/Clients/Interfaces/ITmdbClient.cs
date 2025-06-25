using kiss_graph_api.Clients.Models;

namespace kiss_graph_api.Clients.Interfaces
{
    public interface ITmdbClient
    {
        Task<TmdbMovieDetailDto?> GetMovieByIdAsync(int movieId);
        Task<TmdbPersonDetailDto?> GetPersonByIdAsync(int personId);
        Task<TmdbMovieCreditsDto?> GetCreditsForMovieAsync(int movieId);
        Task<TmdbPersonMovieCreditsDto?> GetMovieCreditsForPersonAsync(int personId);
        Task<TmdbPersonTvCreditsDto?> GetTvCreditsForPersonAsync(int personId);
        Task<TmdbMovieDiscoveryResponse?> DiscoverPopularMoviesAsync(int page = 1);
    }
}

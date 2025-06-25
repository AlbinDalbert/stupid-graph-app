using kiss_graph_api.Clients.Interfaces;
using kiss_graph_api.Clients.Models;

namespace kiss_graph_api.Clients
{
    public class TmdbClient : ITmdbClient
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;

        public TmdbClient(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClient = httpClientFactory.CreateClient();

            // Read settings from appsettings.json
            _apiKey = configuration["Tmdb:ApiKey"];
            var baseUrl = configuration["Tmdb:BaseUrl"];

            if (string.IsNullOrEmpty(_apiKey) || string.IsNullOrEmpty(baseUrl))
            {
                throw new InvalidOperationException("TMDB API Key or BaseUrl is not configured in appsettings.json.");
            }

            // Configure the HttpClient instance
            _httpClient.BaseAddress = new Uri(baseUrl);
            _httpClient.DefaultRequestHeaders.Add("accept", "application/json");
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_apiKey}");
        }

        public async Task<TmdbMovieDetailDto?> GetMovieByIdAsync(int movieId)
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<TmdbMovieDetailDto>($"/movie/{movieId}?language=en-US");
            }
            catch (HttpRequestException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return null; // It's normal for an ID not to be found, so we return null instead of letting it crash.
            }
        }

        public async Task<TmdbPersonDetailDto?> GetPersonByIdAsync(int personId)
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<TmdbPersonDetailDto>($"/person/{personId}?language=en-US");
            }
            catch (HttpRequestException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return null;
            }
        }

        public async Task<TmdbMovieCreditsDto?> GetCreditsForMovieAsync(int movieId)
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<TmdbMovieCreditsDto>($"/movie/{movieId}/credits?language=en-US");
            }
            catch (HttpRequestException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return null;
            }
        }

        public async Task<TmdbPersonMovieCreditsDto?> GetMovieCreditsForPersonAsync(int personId)
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<TmdbPersonMovieCreditsDto>($"/person/{personId}/movie_credits?language=en-US");
            }
            catch (HttpRequestException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return null;
            }
        }

        public async Task<TmdbPersonTvCreditsDto?> GetTvCreditsForPersonAsync(int personId)
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<TmdbPersonTvCreditsDto>($"/person/{personId}/tv_credits?language=en-US");
            }
            catch (HttpRequestException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return null;
            }
        }

        public async Task<TmdbMovieDiscoveryResponse?> DiscoverPopularMoviesAsync(int page = 1)
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<TmdbMovieDiscoveryResponse>($"/discover/movie?language=en-US&sort_by=popularity.desc&page={page}");
            }
            catch (HttpRequestException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return null;
            }
        }

    }
}

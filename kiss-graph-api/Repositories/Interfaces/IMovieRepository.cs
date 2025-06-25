using kiss_graph_api.Clients.Models;
using kiss_graph_api.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace kiss_graph_api.Repositories.Interfaces
{
    public interface IMovieRepository
    {
        Task<IEnumerable<MovieDto>> GetAllAsync();
        Task<MovieDto?> GetByUuidAsync(string uuid);
        Task<MovieDto> CreateAsync(CreateMovieDto movie);
        Task<MovieDto?> UpdateAsync(string uuid, UpdateMovieDto movie);
        Task DeleteAsync(string uuid);
        Task<MovieDto> CreateOrUpdateFromTmdbAsync(TmdbMovieDetailDto tmdbMovie);
    }
}
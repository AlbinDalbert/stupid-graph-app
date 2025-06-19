using kiss_graph_api.DTOs;

namespace kiss_graph_api.Repositories.Interfaces
{
    public interface IInGenreRepository
    {
        Task CreateLink(string movieUuid, string genreUuid);
        Task DeleteLink(string movieUuid, string genreUuid);
    }
}
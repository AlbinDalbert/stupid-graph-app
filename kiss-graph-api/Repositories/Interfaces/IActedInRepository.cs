using kiss_graph_api.DTOs;

namespace kiss_graph_api.Repositories.Interfaces
{
    public interface IActedInRepository
    {
        Task<ActedInSummaryDto> CreateLink(string personUuid, string movieUuid, ActedInDto dto);
        Task DeleteLink(string personUuid, string movieUuid);
    }
}
using kiss_graph_api.DTOs;

namespace kiss_graph_api.Repositories.Interfaces
{
    public interface IAppearsInRepository
    {
        Task<AppearsInSummaryDto> CreateLink(string characterUuid, string creativeWorkUuid, AppearsInDto dto);
        Task DeleteLink(string characterUuid, string creativeWorkUuid);
    }
}

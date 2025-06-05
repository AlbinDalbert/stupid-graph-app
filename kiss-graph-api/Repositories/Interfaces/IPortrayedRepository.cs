using kiss_graph_api.DTOs;

namespace kiss_graph_api.Repositories.Interfaces
{
    public interface IPortrayedRepository
    {
        Task<PortrayedSummaryDto> CreateLink(string personUuid, string characterUuid, PortrayedDto dto);
        Task DeleteLink(string personUuid, string characterUuid);
    }
}

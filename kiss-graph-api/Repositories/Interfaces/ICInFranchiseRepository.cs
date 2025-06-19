using kiss_graph_api.DTOs;

namespace kiss_graph_api.Repositories.Interfaces
{
    public interface ICInFranchiseRepository
    {
        Task<CInFranchiseSummaryDto> CreateLink(string characterUuid, string franchiseUuid);
        Task DeleteLink(string characterUuid, string franchiseUuid);
    }
}

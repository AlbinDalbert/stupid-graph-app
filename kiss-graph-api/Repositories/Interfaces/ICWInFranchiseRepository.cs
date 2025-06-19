using kiss_graph_api.DTOs;

namespace kiss_graph_api.Repositories.Interfaces
{
    public interface ICWInFranchiseRepository
    {
        Task<CWInFranchiseSummaryDto> CreateLink(string creativeWorkUuid, string franchiseUuid);
        Task DeleteLink(string creativeWorkUuid, string franchiseUuid);
    }
}

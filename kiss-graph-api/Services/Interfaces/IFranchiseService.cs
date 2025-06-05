using kiss_graph_api.DTOs;

namespace kiss_graph_api.Services.Interfaces
{
    public interface IFranchiseService
    {
        // Basic creative node operations
        Task<IEnumerable<FranchiseDto>> GetAllFranchisesAsync();
        Task<FranchiseDto> GetFranchiseByUuidAsync(string uuid);
        Task<FranchiseDto> CreateFranchiseAsync(CreateFranchiseDto movie);
        Task<FranchiseDto> UpdateFranchiseAsync(string uuid, UpdateFranchiseDto movie);
        Task DeleteFranchiseAsync(string uuid);
    }
}